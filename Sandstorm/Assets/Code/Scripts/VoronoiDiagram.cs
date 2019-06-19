using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoronoiDiagram
{
    public static Vector2Int[] GenerateSeeds(int areaSize, int cellCount)
    {
        Vector2Int[] seeds = new Vector2Int[cellCount];
        for (int i = 0; i < cellCount; i++)
        {
            seeds[i] = new Vector2Int(Random.Range(0, areaSize), Random.Range(0, areaSize));
        }

        return seeds;
    }

    public static float[,] GenerateHeightMap(int mapSize, int cellCount,
        Vector2Int[] seeds, float maxDistanceFromSeedPoints)
    {
        float[,] heightMap = new float[mapSize, mapSize];
        float[,] distances = new float[mapSize, mapSize];

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                distances[x, mapSize - 1 - y] = Vector2.Distance(new Vector2Int(x, y),
                    seeds[GetClosestSeedIndex(new Vector2Int(x, y), seeds)]);
            }
        }

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float heightValue = distances[x, y] / maxDistanceFromSeedPoints;
                heightMap[x, y] = heightValue;
            }
        }

        return heightMap;
    }

    private static int GetClosestSeedIndex(Vector2Int pixelPosition, Vector2Int[] seeds)
    {
        float sqrShortestDistance = float.MaxValue;
        int index = 0;
        for (int i = 0; i < seeds.Length; i++)
        {
            float sqrDistance = Vector2.SqrMagnitude(pixelPosition - seeds[i]);
            if (sqrDistance < sqrShortestDistance)
            {
                sqrShortestDistance = sqrDistance;
                index = i;
            }
        }

        return index;
    }
}
