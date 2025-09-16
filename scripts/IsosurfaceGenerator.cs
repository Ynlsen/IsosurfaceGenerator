using Godot;

public partial class IsosurfaceGenerator : Node3D
{
    [Export] public int GridSize = 10;

    private FastNoiseLite _noise;

    public override void _Ready()
    {
        _noise = new FastNoiseLite();
        _noise.Frequency = 0.1f;

        GenerateMesh();
    }

    private void GenerateMesh()
    {
        float[,,] density = SampleDensity();

        ArrayMesh mesh = Polygonize(density);
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
        
    }
}
