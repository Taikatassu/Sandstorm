using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureTools
{
    public static Texture2D HeightMapToTexture(float[,] heightMap)
    {
        int textureSizeX = heightMap.GetLength(0);
        int textureSizeY = heightMap.GetLength(1);

        Color[] pixels = new Color[textureSizeX * textureSizeY];

        for (int y = 0; y < textureSizeY; y++)
        {
            for (int x = 0; x < textureSizeX; x++)
            {
                int index = y * textureSizeX + x;
                float heightValue = heightMap[x, y];
                pixels[index] = new Color(heightValue, heightValue, heightValue, 1f);
            }
        }

        return CreateTextureFromColorArray(pixels, textureSizeX, textureSizeY);
    }

    public static Texture2D CreateTextureFromColorArray(Color[] pixelColors, int sizeX, int sizeY)
    {
        Texture2D texture = new Texture2D(sizeX, sizeY);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(pixelColors);
        texture.Apply();
        return texture;
    }
}
