using Godot;

public partial class GridDensityVisualizer : Node3D
{
	private int _gridSize;
	private bool _thresholdDensityVisualization;
	private float _isoLevel;
	private FastNoiseLite _noise;

	private ShaderMaterial _billboardMaterial;

	public void Initialize(int GridSize, bool ThresholdDensityVisualization, float IsoLevel, FastNoiseLite Noise)
	{
		_gridSize = GridSize;
		_thresholdDensityVisualization = ThresholdDensityVisualization;
		_isoLevel = IsoLevel;
		_noise = Noise;
	}

	public override void _Ready()
	{
		var shader = GD.Load<Shader>("res://shaders/BillboardPoint.gdshader");
		_billboardMaterial = new ShaderMaterial { Shader = shader };

		// Using MultiMesh for GPU instancing to significantly improve performance
		var multiMesh = new MultiMesh
		{
			Mesh = new QuadMesh { Size = new Vector2(0.1f, 0.1f) },
			TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
			UseCustomData = true,
			InstanceCount = _gridSize * _gridSize * _gridSize
		};

		var multiMeshInstance = new MultiMeshInstance3D
		{
			Multimesh = multiMesh,
			MaterialOverride = _billboardMaterial

		};
		AddChild(multiMeshInstance);

		int counter = 0;
		var transform = new Transform3D();

		for (int i = 0; i < _gridSize; i++)
		{
			for (int j = 0; j < _gridSize; j++)
			{
				for (int k = 0; k < _gridSize; k++)
				{
					transform.Origin = new Vector3(i, j, k);

					float density = _noise.GetNoise3D(i, j, k);

					float value;
					if (_thresholdDensityVisualization)
					{
						value = density < _isoLevel ? 0f : 1f;
					}
					else
					{
						value = (density + 1f) * 0.5f;
					}

					multiMesh.SetInstanceTransform(counter, transform);
					multiMesh.SetInstanceCustomData(counter, new Color(value, 0, 0));

					counter++;
				}
			}
		}
	}

}
