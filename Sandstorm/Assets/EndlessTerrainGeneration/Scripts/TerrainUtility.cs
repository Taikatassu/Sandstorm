using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainUtility
{
    public static Vector3 CalculateChunkWorldPosition(Vector2Int coordinates, int chunkSize)
    {
        Vector3 worldPosition = new Vector3(coordinates.x * chunkSize, 0f, coordinates.y * chunkSize);
        return worldPosition;
    }

    public static Vector2Int CalculateChunkCoordinates(Vector3 worldPosition, int chunkSize)
    {
        Vector2Int chunkCoordinate = new Vector2Int(Mathf.RoundToInt(worldPosition.x / chunkSize),
            Mathf.RoundToInt(worldPosition.z / chunkSize));

        return chunkCoordinate;
    }

    public static Bounds CreateChunkBounds(Vector2Int coordinates, int chunkSize)
    {
        return new Bounds(CalculateChunkWorldPosition(coordinates, chunkSize), Vector3.one * chunkSize);
    }

    public static float CalculateSqrHorizontalDistanceToChunk(Vector3 point, Vector2Int chunkCoordinates, int chunkSize)
    {
        Vector3 chunkWorldPosition = CalculateChunkWorldPosition(chunkCoordinates, chunkSize);

        float xDistance = Mathf.Max(Mathf.Abs(point.x - chunkWorldPosition.x) - (float)(chunkSize / 2f), 0f);
        float zDistance = Mathf.Max(Mathf.Abs(point.z - chunkWorldPosition.z) - (float)(chunkSize / 2f), 0f);

        return xDistance * xDistance + zDistance * zDistance;
    }
}
