using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinNoise
{
    public static Vector2[] GenerateOctaveOffsets(int octaves)
    {
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = Random.Range(-100000, 100000);
            float offsetY = Random.Range(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        return octaveOffsets;
    }

    public static float[,] GenerateHeightMap(int mapSize, float scale, Vector2[] octaveOffsets,
        float persistance, float lacunarity, Vector2 offset, float maxHeightNormalizationMultiplier)
    {
        float[,] noiseMap = new float[mapSize, mapSize];

        float maxPossibleHeight = 0;
        float amplitude = 1;

        for (int i = 0; i < octaveOffsets.Length; i++)
        {
            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float halfMapSize = mapSize / 2f;

        float normalizedScale = scale * mapSize;

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaveOffsets.Length; i++)
                {
                    float sampleX = (float)(x - halfMapSize + offset.x + octaveOffsets[i].x) / normalizedScale * frequency;
                    float sampleY = (float)(y - halfMapSize + offset.y + octaveOffsets[i].y) / normalizedScale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                noiseMap[x, mapSize - 1 - y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float normalizedHeight = (noiseMap[x, y] + 1) / (2f * maxPossibleHeight * maxHeightNormalizationMultiplier);
                noiseMap[x, y] = normalizedHeight;
            }
        }

        return noiseMap;
    }
}
