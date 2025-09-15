using Godot;

public partial class GridDensityVisualizer : Node3D
{
	[Export] public int GridSize = 10;

	private ShaderMaterial _billboardMaterial;

	public override void _Ready()
	{
		var shader = GD.Load<Shader>("res://shaders/BillboardPoint.gdshader");
		_billboardMaterial = new ShaderMaterial { Shader = shader };

		// Using MultiMesh for GPU instancing to significantly improve performance
		var multiMesh = new MultiMesh();
		multiMesh.Mesh = new QuadMesh { Size = new Vector2(0.1f, 0.1f) };
		multiMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
		multiMesh.UseCustomData = true;
		multiMesh.InstanceCount = GridSize * GridSize * GridSize;

		var multiMeshInstance = new MultiMeshInstance3D();
		multiMeshInstance.Multimesh = multiMesh;
		multiMeshInstance.MaterialOverride = _billboardMaterial;
		AddChild(multiMeshInstance);

		FastNoiseLite noise = new FastNoiseLite();
		noise.Frequency = 0.1f;

		int counter = 0;
		for (int i = 0; i < GridSize; i++)
		{
			for (int j = 0; j < GridSize; j++)
			{
				for (int k = 0; k < GridSize; k++)
				{
					var pos = new Vector3(i, j, k);
					var transform = new Transform3D(Basis.Identity, pos);

					float density = ((noise.GetNoise3D(i, j, k) + 1f) * 0.5f) > 0.5f ? 1f : 0f;

					multiMesh.SetInstanceTransform(counter, transform);
					multiMesh.SetInstanceCustomData(counter, new Color(density, 0, 0));

					counter++;
				}
			}
		}
	}

}
