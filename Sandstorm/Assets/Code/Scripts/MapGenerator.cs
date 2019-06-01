using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public DrawType drawType;

    [Header("Generation")]
    public int randomSeed;

    [Header("Voronoi")]
    public int cellCount;
    public float maxDistanceFromSeedPoint;
    public AnimationCurve heightCurve;

    [Header("Perlin")]
    public float noiseScale;
    public int octaves;
    public float persistance;
    public float lacunarity;

    [Header("Terrain")]
    public int chunkSize;
    [Range(0, 6)]
    public int levelOfDetail;
    public float heightMultiplier;
    public Material material;

    [Header("Object Placement")]
    public GameObject[] objectsToPlace;
    public float placementRaycastStartHeight;
    public float finalPlacementHeightOffset;

    private Dictionary<Vector2Int, Vector2Int[]> voronoiSeedsDictionary;
    private Vector2[] octaveOffsets;

    private List<GameObject> terrainDisplays = new List<GameObject>();
    private List<GameObject> textureDisplays = new List<GameObject>();

    public enum DrawType
    {
        TextureOnly,
        MeshOnly,
        TextureAndMesh
    }

    public void Generate()
    {
        ClearDisplayObjects();

        if (chunkSize <= 0)
        {
            Debug.Log("Invalid chunk size!");
            return;
        }

        if (voronoiSeedsDictionary == null)
        {
            ResetSeeds();
        }

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);
                Vector2Int[] seeds = GenerateSeeds(coordinates);

                switch (drawType)
                {
                    case DrawType.TextureOnly:
                        GenerateTexture(coordinates, seeds);
                        break;
                    case DrawType.MeshOnly:
                        GenerateMesh(coordinates, seeds);
                        break;
                    case DrawType.TextureAndMesh:
                        GenerateTexture(coordinates, seeds);
                        GenerateMesh(coordinates, seeds);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void ResetSeeds()
    {
        Random.InitState(randomSeed);
        voronoiSeedsDictionary = new Dictionary<Vector2Int, Vector2Int[]>();
        Vector2Int[] seeds = GenerateSeeds(new Vector2Int(0, 0));

        octaveOffsets = PerlinNoise.GenerateOctaveOffsets(octaves);

        Debug.Log("Seeds reset.");
    }

    private void ClearDisplayObjects()
    {
        if (textureDisplays != null)
        {
            for (int i = 0; i < textureDisplays.Count; i++)
            {
                if (textureDisplays[i] != null)
                    DestroyImmediate(textureDisplays[i]);
            }
        }

        if (terrainDisplays != null)
        {
            for (int i = 0; i < terrainDisplays.Count; i++)
            {
                if (terrainDisplays[i] != null)
                    DestroyImmediate(terrainDisplays[i]);
            }
        }

        textureDisplays = new List<GameObject>();
        terrainDisplays = new List<GameObject>();
    }

    private Vector2Int[] GenerateSeeds(Vector2Int coordinates)
    {
        List<Vector2Int> nearbySeeds = new List<Vector2Int>();

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                Vector2Int coordinateToCheck = new Vector2Int(coordinates.x + x, coordinates.y + y);
                Vector2Int[] chunkSeeds;
                if (voronoiSeedsDictionary.ContainsKey(coordinateToCheck))
                {
                    chunkSeeds = voronoiSeedsDictionary[coordinateToCheck];
                }
                else
                {
                    chunkSeeds = VoronoiDiagram.GenerateSeeds(chunkSize, cellCount);
                    voronoiSeedsDictionary.Add(coordinateToCheck, chunkSeeds);
                }

                Vector2Int[] offsetSeeds = new Vector2Int[chunkSeeds.Length];
                for (int i = 0; i < offsetSeeds.Length; i++)
                {
                    offsetSeeds[i] = new Vector2Int(chunkSeeds[i].x + x * chunkSize, chunkSeeds[i].y + y * chunkSize);
                }

                nearbySeeds.AddRange(offsetSeeds);
            }
        }

        return nearbySeeds.ToArray();
    }

    private void GenerateTexture(Vector2Int coordinates, Vector2Int[] seeds)
    {
        GameObject textureDisplay = new GameObject("TextureDisplay");
        textureDisplays.Add(textureDisplay);
        Transform textureTransform = textureDisplay.transform;
        textureTransform.SetParent(transform);
        textureTransform.localPosition = new Vector3(coordinates.x * chunkSize, 20f, coordinates.y * chunkSize);
        textureTransform.localEulerAngles = new Vector3(-90, 0, 0);
        textureTransform.localScale = new Vector3(100, 100, 1);
        SpriteRenderer spriteRenderer = textureDisplay.AddComponent<SpriteRenderer>();

        Vector2 perlinOffset = new Vector2(coordinates.x * chunkSize, coordinates.y * chunkSize);
        Texture2D texture = VoronoiDiagram.GenerateTextureByDistance(chunkSize + 1,
            cellCount, seeds, maxDistanceFromSeedPoint);
        //Texture2D texture = PerlinNoise.GenerateTexture(chunkSize + 1, noiseScale,
        //    octaveOffsets, persistance, lacunarity, perlinOffset);
        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, chunkSize, chunkSize), Vector2.one * 0.5f);
    }

    private void GenerateMesh(Vector2Int coordinates, Vector2Int[] seeds)
    {
        GameObject terrainDisplay = new GameObject("TerrainDisplay");
        MeshFilter meshFilter = terrainDisplay.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = terrainDisplay.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = terrainDisplay.AddComponent<MeshCollider>();
        terrainDisplays.Add(terrainDisplay);

        Transform terrainTransform = terrainDisplay.transform;
        terrainTransform.SetParent(transform);
        terrainTransform.localPosition = new Vector3(coordinates.x * chunkSize, 0, coordinates.y * chunkSize);

        MeshData meshData = MeshGenerator.GenerateTerrainMesh(
            VoronoiDiagram.GenerateHeightMapByDistance(chunkSize + 1, cellCount, seeds,
            maxDistanceFromSeedPoint), heightMultiplier, heightCurve, levelOfDetail);

        DrawMesh(meshData, material, terrainDisplay);
        PlaceObjects(objectsToPlace, placementRaycastStartHeight, finalPlacementHeightOffset);
    }

    private void DrawMesh(MeshData meshData, Material material, GameObject terrainDisplay)
    {
        Mesh mesh = meshData.CreateMesh();
        terrainDisplay.GetComponent<MeshFilter>().sharedMesh = mesh;
        terrainDisplay.GetComponent<MeshRenderer>().sharedMaterial = material;
        terrainDisplay.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void PlaceObjects(GameObject[] objects, float raycastStartHeight, float finalPlacementHeightOffset)
    {
        if (objects == null || objects.Length == 0) return;

        foreach (var o in objects)
        {
            o.SetActive(false);
            Transform objectTransform = o.transform;
            Vector3 originalObjectPosition = objectTransform.position;
            Vector3 rayOrigin = new Vector3(originalObjectPosition.x, raycastStartHeight, originalObjectPosition.z);
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, Vector3.down, out hit))
            {
                objectTransform.position = new Vector3(originalObjectPosition.x,
                    hit.point.y + finalPlacementHeightOffset, originalObjectPosition.z);
            }

            o.SetActive(true);
        }
    }
}
