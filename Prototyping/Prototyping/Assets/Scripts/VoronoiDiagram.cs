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

    public static Texture2D GenerateColoredTexture(int textureSize, int cellCount,
        Vector2Int[] seeds)
    {
        Color[] cellColors = new Color[seeds.Length];
        for (int i = 0; i < cellCount; i++)
        {
            cellColors[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }

        Color[] pixels = new Color[textureSize * textureSize];

        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                int index = x * textureSize + y;
                pixels[index] = cellColors[GetClosestSeedIndex(new Vector2Int(x, y), seeds)];
            }
        }

        return CreateTextureFromColorArray(pixels, textureSize);
    }

    //public static Texture2D GenerateTextureByDistance(int textureSize, int cellCount,
    //    Vector2Int[] seeds, float maxDistanceFromSeedPoints)
    //{
    //    Color[] pixels = new Color[textureSize * textureSize];
    //    float[] distances = new float[textureSize * textureSize];

    //    for (int x = 0; x < textureSize; x++)
    //    {
    //        for (int y = 0; y < textureSize; y++)
    //        {
    //            int index = x * textureSize + y;
    //            distances[index] = Vector2.Distance(new Vector2Int(x, y), 
    //                seeds[GetClosestSeedIndex(new Vector2Int(x, y), seeds)]);
    //        }
    //    }
        
    //    Debug.Log("maxDistance:" + GetMaxDistance(distances));
    //    for (int i = 0; i < distances.Length; i++)
    //    {
    //        float colorValue = distances[i] / maxDistanceFromSeedPoints;
    //        pixels[i] = new Color(colorValue, colorValue, colorValue, 1f);
    //    }

    //    return CreateTextureFromColorArray(pixels, textureSize);
    //}

    public static Texture2D GenerateTextureByDistance(int textureSize, int cellCount,
        Vector2Int[] seeds, float maxDistanceFromSeedPoints)
    {
        float[,] heightMap = GenerateHeightMapByDistance(textureSize, cellCount, seeds, maxDistanceFromSeedPoints);

        Color[] pixels = new Color[textureSize * textureSize];

        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                int index = x * textureSize + y;
                float heightValue = heightMap[x, y];
                pixels[index] = new Color(heightValue, heightValue, heightValue, 1f);
            }
        }

        return CreateTextureFromColorArray(pixels, textureSize);
    }

    public static float[,] GenerateHeightMapByDistance(int mapSize, int cellCount,
        Vector2Int[] seeds, float maxDistanceFromSeedPoints)
    {
        float[,] heightMap = new float[mapSize, mapSize];
        float[,] distances = new float[mapSize, mapSize];
        
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                distances[x, mapSize - 1 - y] = Vector2.Distance(new Vector2Int(x, y),
                    seeds[GetClosestSeedIndex(new Vector2Int(x, y), seeds)]);
            }
        }

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                float heightValue = distances[x, y] / maxDistanceFromSeedPoints;
                heightMap[x, y] = heightValue;
            }
        }

        return heightMap;
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

    private static Texture2D CreateTextureFromColorArray(Color[] pixelColors, int textureSize)
    {
        Texture2D texture = new Texture2D(textureSize, textureSize);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(pixelColors);
        texture.Apply();
        return texture;
    }


}
