using UnityEngine;

public struct HeightMapData
{
    public readonly Vector2Int coordinates;
    public readonly float[,] heightMap;

    public HeightMapData(Vector2Int coordinate, float[,] heightMap)
    {
        this.coordinates = coordinate;
        this.heightMap = heightMap;
    }
}