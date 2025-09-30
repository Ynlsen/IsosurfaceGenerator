using System;
using Godot;

public partial class UiManager : Control
{
	[Export] public SpinBox GridSizeInput;
	[Export] public OptionButton AlgorithmInput;
	[Export] public SpinBox IsoLevelInput;
	[Export] public CheckBox VisualizerInput;
	[Export] public CheckBox ThresholdInput;
	[Export] public SpinBox FrequencyInput;
	[Export] public CheckBox RandomSeedInput;
	[Export] public SpinBox SeedInput;

	private IsosurfaceGenerator _isosurfaceGenerator;

	public void Initialize(int gridSize, IsosurfaceGenerator.Algorithms algorithm, float isoLevel, bool showVisualizer, bool thresholdDensityVisualization, float frequency, bool randomSeed, float seed, IsosurfaceGenerator isosurfaceGenerator)
	{
		GridSizeInput.Value = gridSize;
		AlgorithmInput.Selected = (int)algorithm;
		IsoLevelInput.Value = isoLevel;
		VisualizerInput.ButtonPressed = showVisualizer;
		ThresholdInput.ButtonPressed = thresholdDensityVisualization;
		FrequencyInput.Value = frequency;
		RandomSeedInput.ButtonPressed = randomSeed;
		SeedInput.Value = seed;
		_isosurfaceGenerator = isosurfaceGenerator;
	}

	public void OnGenerateButtonPressed()
	{
		_isosurfaceGenerator.SetSettings(
			(int)GridSizeInput.Value,
			(IsosurfaceGenerator.Algorithms)AlgorithmInput.Selected,
			(float)IsoLevelInput.Value,
			VisualizerInput.ButtonPressed,
			ThresholdInput.ButtonPressed,
			(float)FrequencyInput.Value,
			RandomSeedInput.ButtonPressed,
			(int)SeedInput.Value
		);

		_isosurfaceGenerator.Generate();
	}

	public void OnVisualizerInputToggled(bool ToggledOn)
	{
		ThresholdInput.Disabled = !ToggledOn;
	}

	public void OnRandomSeedInputToggled(bool ToggledOn)
	{
		SeedInput.Editable = !ToggledOn; 
	}
}
