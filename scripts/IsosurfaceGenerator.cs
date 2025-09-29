using Godot;

public partial class IsosurfaceGenerator : Node3D
{
	[Export] public int GridSize = 10;
	[Export] public Algorithms Algorithm = Algorithms.MarchingCubes;
	[Export] public float IsoLevel = 0f;
	[Export] public bool ShowVisualizer = true;
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
		if (_meshInstance3D != null && IsInstanceValid(_meshInstance3D))
		{
			RemoveChild(_meshInstance3D);
			_meshInstance3D.QueueFree();
		}

		float[,,] density = SampleDensity();

		IsosurfaceAlgorithm selectedAlgorithm;
		
		switch (Algorithm)
		{
			case Algorithms.MarchingCubes:
				selectedAlgorithm = new MarchingCubesGenerator();
				break;

			default:
				GD.PrintErr("Invalid Algorithm");
				return;
		}

		ArrayMesh mesh = selectedAlgorithm.Polygonize(density,IsoLevel);

		_meshInstance3D = new MeshInstance3D();
		_meshInstance3D.Mesh = mesh;

		AddChild(_meshInstance3D);
	}

	private void SpawnVisualizer()
	{
		if (_gridDensityVisualizer != null && IsInstanceValid(_gridDensityVisualizer))
		{
			RemoveChild(_gridDensityVisualizer);
			_gridDensityVisualizer.QueueFree();
		}

		if (!ShowVisualizer)
		{
			return;
		}

		_gridDensityVisualizer = GridVisualizerScene.Instantiate<GridDensityVisualizer>();
		_gridDensityVisualizer.Initialize(GridSize, ThresholdDensityVisualization, IsoLevel, _noise);

		AddChild(_gridDensityVisualizer);
	}

	private void SpawnUi()
	{
		if (_uiManager != null && IsInstanceValid(_uiManager))
		{
			RemoveChild(_uiManager);
			_uiManager.QueueFree();
		}

		_uiManager = UiScene.Instantiate<UiManager>();
		_uiManager.Initialize(GridSize, Algorithm, IsoLevel, ShowVisualizer, ThresholdDensityVisualization, Frequency, Seed, this);

		AddChild(_uiManager);	
	}

	public void SetSettings(int GridSize, Algorithms Algorithm, float IsoLevel, bool ShowVisualizer, bool ThresholdDensityVisualization, float Frequency, int Seed)
	{
		this.GridSize = GridSize;
		this.Algorithm = Algorithm;
		this.IsoLevel = IsoLevel;
		this.ShowVisualizer = ShowVisualizer;
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
}
