using UnityEngine;
using System;

public class TextureDisplayMeshGenerator : MonoBehaviour, ITerrainMeshGenerator
{
    [Tooltip("Transform under which the terrain mesh objects are spawned")]
    [SerializeField] private Transform parent = null;
    [Tooltip("Ignores extra rows and columns from the generated height map " +
             "(this mesh generator does not need any extra height map rows or columns)")]
    [SerializeField] private int extraHeightMapSizeToIgnore = 1;

    public void RequestMesh(Vector2Int coordinates, int chunkSize, float[,] heightMap,
                                    Action<MeshData> callback)
    {
        // This script is only for testing purposes and therefore does not actually implement threading, 
        //      so just generate the chunk on current thread and return it instantly
        GameObject chunkMesh = GenerateTextureDisplay(coordinates, chunkSize, heightMap);
        callback(new MeshData(coordinates, chunkMesh));
    }

    private GameObject GenerateTextureDisplay(Vector2Int coordinates, int chunkSize, float[,] heightMap)
    {
        GameObject newChunk = new GameObject("TextureDisplay");

        Transform textureTransform = newChunk.transform;
        textureTransform.SetParent(parent);
        textureTransform.localPosition = TerrainUtility.CalculateChunkWorldPosition(coordinates, chunkSize);
        textureTransform.localEulerAngles = new Vector3(90, 0, 0);
        textureTransform.localScale = new Vector3(chunkSize, chunkSize, 1);

        float spriteSize = heightMap.GetLength(0) - extraHeightMapSizeToIgnore;
        SpriteRenderer spriteRenderer = newChunk.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Sprite.Create(HeightMapToTexture(heightMap),
            new Rect(0, 0, spriteSize, spriteSize), Vector2.one * 0.5f, 1f * chunkSize);

        return newChunk;
    }

    private Texture2D HeightMapToTexture(float[,] heightMap)
    {
        int textureSizeX = heightMap.GetLength(0);
        int textureSizeY = heightMap.GetLength(1);

        Color[] pixels = new Color[textureSizeX * textureSizeY];

        for (int x = 0; x < textureSizeX; x++)
        {
            for (int y = 0; y < textureSizeY; y++)
            {
                int index = y * textureSizeX + x;
                float colorValue = heightMap[x, y];
                pixels[index] = new Color(colorValue, colorValue, colorValue, 1f);
            }
        }

        return CreateTextureFromColorArray(pixels, textureSizeX, textureSizeY);
    }

    private Texture2D CreateTextureFromColorArray(Color[] pixelColors, int sizeX, int sizeY)
    {
        Texture2D texture = new Texture2D(sizeX, sizeY);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(pixelColors);
        texture.Apply();
        return texture;
    }
}
