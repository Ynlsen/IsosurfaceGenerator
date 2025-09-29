using Godot;

public partial class UiManager : Control
{
	[Export] public SpinBox GridSizeInput;
	[Export] public OptionButton AlgorithmInput;
	[Export] public SpinBox IsoLevelInput;
	[Export] public CheckBox ThresholdInput;
	[Export] public SpinBox FrequencyInput;
	[Export] public SpinBox SeedInput;

	private IsosurfaceGenerator _isosurfaceGenerator;

	public void Initialize(int GridSize, IsosurfaceGenerator.Algorithms Algorithm, float IsoLevel, bool ThresholdDensityVisualization, float Frequency, float Seed, IsosurfaceGenerator IsosurfaceGenerator)
	{
		GridSizeInput.Value = GridSize;
		AlgorithmInput.Selected = (int)Algorithm;
		IsoLevelInput.Value = IsoLevel;
		ThresholdInput.ButtonPressed = ThresholdDensityVisualization;
		FrequencyInput.Value = Frequency;
		SeedInput.Value = Seed;
		_isosurfaceGenerator = IsosurfaceGenerator;
	}

	public void OnGenerateButtonPressed()
	{
		_isosurfaceGenerator.SetSettings(
			(int)GridSizeInput.Value,
			(IsosurfaceGenerator.Algorithms)AlgorithmInput.Selected,
			(float)IsoLevelInput.Value,
			ThresholdInput.ButtonPressed,
			(float)FrequencyInput.Value,
			(int)SeedInput.Value
		);

		_isosurfaceGenerator.Generate();
	}
}
