using System.Collections.Generic;
using Godot;

public partial class IsosurfaceGenerator : Node3D
{
	[Export] public int GridSize = 10;
	[Export] public float Isolevel = 0f;

	private FastNoiseLite _noise;
	
	private const int NumCorners = 8;
	private const int NumEdges = 12;
	private const int MaxTrianglesPerCube = 5;

	public override void _Ready()
	{
		_noise = new FastNoiseLite();
		_noise.Frequency = 0.05f;

		GenerateMesh();
	}

	private void GenerateMesh()
	{
		float[,,] density = SampleDensity();

		ArrayMesh mesh = Polygonize(density);

		var meshInstance = new MeshInstance3D();
		meshInstance.Mesh = mesh;
		AddChild(meshInstance);
	}

	private float[,,] SampleDensity()
	{
		var density = new float[GridSize, GridSize, GridSize];

		for (int i = 0; i < GridSize; i++)
		{
			for (int j = 0; j < GridSize; j++)
			{
				for (int k = 0; k < GridSize; k++)
				{
					density[i, j, k] = _noise.GetNoise3D(i, j, k);
				}
			}
		}

		return density;
	}

	private ArrayMesh Polygonize(float[,,] density)
	{
		var surfaceTool = new SurfaceTool();
		surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		var cornerPositions = new Vector3[NumCorners];
		var cornerDensities = new float[NumCorners];
		var edgeVertices = new Vector3[NumEdges];

		for (int i = 0; i < GridSize - 1; i++)
		{
			for (int j = 0; j < GridSize - 1; j++)
			{
				for (int k = 0; k < GridSize - 1; k++)
				{
					int cubeIndex = 0;
					for (int l = 0; l < NumCorners; l++)
					{
						Vector3 offset = MarchingCubesData.CornerOffsetTable[l];
						int ci = i + (int)offset.X;
						int cj = j + (int)offset.Y;
						int ck = k + (int)offset.Z;

						cornerPositions[l] = new Vector3(ci, cj, ck);
						cornerDensities[l] = density[ci, cj, ck];

						if (cornerDensities[l] < 0f)
						{
							cubeIndex |= 1 << l;
						}
					}

					int edges = MarchingCubesData.EdgeTable[cubeIndex];
					if (edges == 0)
					{
						continue;
					}

					for (int m = 0; m < NumEdges; m++)
					{
						if ((edges & (1 << m)) != 0)
						{
							int v0 = MarchingCubesData.EdgeConnectionTable[m, 0];
							int v1 = MarchingCubesData.EdgeConnectionTable[m, 1];
							edgeVertices[m] = (cornerPositions[v0] + cornerPositions[v1]) * 0.5f;
						}
					}

					for (int n = 0; n < MaxTrianglesPerCube * 3; n += 3)
					{
						int a = MarchingCubesData.TriangulationTable[cubeIndex, n];
						if (a == -1)
						{
							break;
						}
						int b = MarchingCubesData.TriangulationTable[cubeIndex, n + 1];
						int c = MarchingCubesData.TriangulationTable[cubeIndex, n + 2];

						surfaceTool.AddVertex(edgeVertices[c]);
						surfaceTool.AddVertex(edgeVertices[b]);
						surfaceTool.AddVertex(edgeVertices[a]);
					}

				}
			}
		}

		surfaceTool.GenerateNormals();

		return surfaceTool.Commit();
	}
}
