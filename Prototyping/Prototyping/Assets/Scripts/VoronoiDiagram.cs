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

    //public static Texture2D GenerateColoredTexture(int textureSize, int cellCount,
    //    Vector2Int[] seeds)
    //{
    //    Color[] cellColors = new Color[seeds.Length];
    //    for (int i = 0; i < cellCount; i++)
    //    {
    //        cellColors[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
    //    }

    //    Color[] pixels = new Color[textureSize * textureSize];

    //    for (int x = 0; x < textureSize; x++)
    //    {
    //        for (int y = 0; y < textureSize; y++)
    //        {
    //            int index = x * textureSize + y;
    //            pixels[index] = cellColors[GetClosestSeedIndex(new Vector2Int(x, y), seeds)];
    //        }
    //    }

    //    return TextureTools.CreateTextureFromColorArray(pixels, textureSize, textureSize);
    //}

    public static float[,] GenerateHeightMapByDistance(int mapSize, int cellCount,
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

    public static Texture2D GenerateTextureByDistance(int textureSize, int cellCount,
        Vector2Int[] seeds, float maxDistanceFromSeedPoints)
    {
        return TextureTools.HeightMapToTexture(
            GenerateHeightMapByDistance(textureSize, cellCount, seeds, maxDistanceFromSeedPoints));
    }

    private static float GetMaxDistance(float[] distances)
    {
        float maxDistance = float.MinValue;
        for (int i = 0; i < distances.Length; i++)
        {
            if (distances[i] > maxDistance)
            {
                maxDistance = distances[i];
            }
        }

        return maxDistance;
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
