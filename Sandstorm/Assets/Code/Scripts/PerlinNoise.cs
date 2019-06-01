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
            float offsetX = Random.Range(-10000, 10000);
            float offsetY = Random.Range(-10000, 10000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        return octaveOffsets;
    }

    public static float[,] GenerateNoiseMap(int mapSize, float scale, Vector2[] octaveOffsets,
        float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapSize, mapSize];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfMapSize = mapSize / 2f;

        float normalizedScale = scale * mapSize;

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float amplitude = 1;
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

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

                noiseMap[x, mapSize - 1 - y] = noiseHeight;
            }
        }

        // TODO: Fix this!
        // Requires global height normalization
        // https://www.youtube.com/watch?v=4olmeStiBsE 4:10->

        //for (int y = 0; y < mapSize; y++)
        //{
        //    for (int x = 0; x < mapSize; x++)
        //    {
        //        noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
        //    }
        //}

        return noiseMap;
    }

    public static Texture2D GenerateTexture(int textureSize, float noiseScale,
        Vector2[] octaveOffsets, float persistance, float lacunarity, Vector2 offset)
    {
        return TextureTools.HeightMapToTexture(
            GenerateNoiseMap(textureSize, noiseScale, octaveOffsets, persistance, lacunarity, offset));
    }
}
