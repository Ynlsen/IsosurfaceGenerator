using Godot;

public interface IsosurfaceAlgorithm
{
    ArrayMesh Polygonize(float[,,] density, float IsoLevel);
}
