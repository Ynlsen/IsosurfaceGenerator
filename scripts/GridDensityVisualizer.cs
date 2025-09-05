using Godot;

public partial class GridDensityVisualizer : Node3D
{
	[Export] public int GridSize = 10;
	[Export] public float Radius = 4.0f;

	private ShaderMaterial _billboardMaterial;

	public override void _Ready()
	{
		var shader = GD.Load<Shader>("res://shaders/BillboardPoint.gdshader");
		_billboardMaterial = new ShaderMaterial { Shader = shader };


		var multiMesh = new MultiMesh();
		multiMesh.Mesh = new QuadMesh { Size = new Vector2(0.1f, 0.1f) };
		multiMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
		multiMesh.UseCustomData = true;
		multiMesh.InstanceCount = GridSize * GridSize * GridSize;

		var multiMeshInstance = new MultiMeshInstance3D();
		multiMeshInstance.Multimesh = multiMesh;
		multiMeshInstance.MaterialOverride = _billboardMaterial;
		AddChild(multiMeshInstance);

		int counter = 0;
		for (int i = 0; i < GridSize; i++)
		{
			for (int j = 0; j < GridSize; j++)
			{
				for (int k = 0; k < GridSize; k++)
				{
					var pos = new Vector3(i, j, k);
					var transform = new Transform3D(Basis.Identity, pos);

					float density = 1f;

					multiMesh.SetInstanceTransform(counter, transform);
					multiMesh.SetInstanceCustomData(counter, new Color(density, 0, 0));

					counter++;
				}
			}
		}
	}

}
