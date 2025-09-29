using Godot;

public partial class IsosurfaceGenerator : Node3D
{
	[Export] public int GridSize = 10;
	[Export] public Algorithms Algorithm = Algorithms.MarchingCubes;
	[Export] public float IsoLevel = 0f;
	[Export] public bool ThresholdDensityVisualization = true;
	[Export] public float Frequency = 0.05f;
	[Export] public int Seed = 0;

	[Export] public PackedScene GridVisualizerScene;
	[Export] public PackedScene UiScene;

	public enum Algorithms{
		MarchingCubes
	}

	private FastNoiseLite _noise;

	private MeshInstance3D _meshInstance3D;
	private GridDensityVisualizer _gridDensityVisualizer;
	private UiManager _uiManager;
	
	private const int NumCorners = 8;
	private const int NumEdges = 12;
	private const int MaxTrianglesPerCube = 5;

	public override void _Ready()
	{
		_noise = new FastNoiseLite();

		Generate();
		SpawnUi();
	}

	public void Generate()
	{
		_noise.Frequency = Frequency;
		_noise.Seed = Seed;

		GenerateMesh();
		SpawnVisualizer();
	}

	private void GenerateMesh()
	{
		RemoveChild(_meshInstance3D);

		float[,,] density = SampleDensity();

		ArrayMesh mesh = Polygonize(density);

		_meshInstance3D = new MeshInstance3D();
		_meshInstance3D.Mesh = mesh;

		AddChild(_meshInstance3D);
	}

	private void SpawnVisualizer()
	{
		RemoveChild(_gridDensityVisualizer);

		_gridDensityVisualizer = GridVisualizerScene.Instantiate<GridDensityVisualizer>();
		_gridDensityVisualizer.Initialize(GridSize, ThresholdDensityVisualization, IsoLevel, _noise);

		AddChild(_gridDensityVisualizer);
	}

	private void SpawnUi()
	{
		RemoveChild(_uiManager);

		_uiManager = UiScene.Instantiate<UiManager>();
		_uiManager.Initialize(GridSize, Algorithm, IsoLevel, ThresholdDensityVisualization, Frequency, Seed, this);

		AddChild(_uiManager);	
	}

	public void SetSettings(int GridSize, Algorithms Algorithm, float IsoLevel, bool ThresholdDensityVisualization, float Frequency, int Seed)
	{
		this.GridSize = GridSize;
		this.Algorithm = Algorithm;
		this.IsoLevel = IsoLevel;
		this.ThresholdDensityVisualization = ThresholdDensityVisualization;
		this.Frequency = Frequency;
		this.Seed = Seed;
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

						if (cornerDensities[l] < IsoLevel)
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
