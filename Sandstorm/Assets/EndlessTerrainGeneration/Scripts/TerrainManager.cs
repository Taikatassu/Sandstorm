using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [Tooltip("65535 vertices (~240x240) is the max supported mesh size in Unity")]
    [SerializeField] private int chunkSize = 240;
    public int ChunkSize { get { return chunkSize; } }
    //[Tooltip("Use this event to for example toggle prop-objects on a given chunk")]
    public Vector2IntBoolUnityEvent OnChunkVisibilityChange;

    private ITerrainMeshGenerator meshGenerator;
    private IHeightMapGenerator heightMapGenerator;
    private List<TerrainChunk> activeChunks = new List<TerrainChunk>();
    public Dictionary<Vector2Int, TerrainChunk> chunkDictionary { get; private set; }
        = new Dictionary<Vector2Int, TerrainChunk>();

    private void Awake()
    {
        FindAndValidateReferences();
    }

    public void DrawChunks(Vector2Int[] chunkCoordinates, bool disableOtherChunks)
    {
        List<TerrainChunk> newActiveChunks = new List<TerrainChunk>();

        foreach (var coord in chunkCoordinates)
        {
            if (chunkDictionary.ContainsKey(coord))
            {
                // Enable existing chunk
                TerrainChunk chunk = chunkDictionary[coord];
                chunk.SetChunkState(true);
                newActiveChunks.Add(chunk);
            }
            else
            {
                // Create new chunk and request a height map for it
                TerrainChunk newChunk = new TerrainChunk(coord, BroadcastChunkVisibilityChange);
                newChunk.SetChunkState(true);
                chunkDictionary.Add(coord, newChunk);
                newActiveChunks.Add(newChunk);
                heightMapGenerator.RequestHeightMap(coord, chunkSize, OnHeightMapReceived);
            }
        }

        foreach (var previouslyActiveChunk in activeChunks)
        {
            if (!newActiveChunks.Contains(previouslyActiveChunk))
            {
                if (disableOtherChunks)
                {
                    previouslyActiveChunk.SetChunkState(false);
                }
                else
                {
                    newActiveChunks.Add(previouslyActiveChunk);
                }
            }
        }

        activeChunks = newActiveChunks;
    }

    // Clears all existing chunks (data and game objects) from the chunkDictionary
    public void CleanUpChunks(Vector2Int[] exceptions)
    {
        Vector2Int[] existingChunks = new Vector2Int[chunkDictionary.Keys.Count];
        chunkDictionary.Keys.CopyTo(existingChunks, 0);
        int counter = 0;

        List<Vector2Int> chunksNotToClean = new List<Vector2Int>();
        for (int i = 0; i < exceptions.Length; i++)
        {
            chunksNotToClean.Add(exceptions[i]);
        }

        foreach (var coordinate in existingChunks)
        {
            if (!chunksNotToClean.Contains(coordinate))
            {
                chunkDictionary[coordinate].Clean();
                chunkDictionary.Remove(coordinate);
                counter++;
            }
        }
    }

    private void FindAndValidateReferences()
    {
        meshGenerator = GetComponent<ITerrainMeshGenerator>();
        if (meshGenerator == null)
            Debug.LogWarning("ITerrainMeshGenerator not found! Please add a component that implements " +
                "the ITerrainMeshGenerator interface to this game object.", gameObject);

        heightMapGenerator = GetComponent<IHeightMapGenerator>();
        if (meshGenerator == null)
            Debug.LogWarning("IHeightMapGenerator not found! Please add a component that implements " +
                "the IHeightMapGenerator interface to this game object.", gameObject);
    }

    private void OnHeightMapReceived(HeightMapData heightMapData)
    {
        if (chunkDictionary.ContainsKey(heightMapData.coordinates))
        {
            meshGenerator.RequestMesh(heightMapData.coordinates,
                chunkSize, heightMapData.heightMap, OnMeshReceived);
        }
        else
        {
            Debug.LogWarning("Height map received for a chunk that does not exist in chunk dictionary." +
                " (Request height map only once corresponding TerrainChunk has been added to the dictionary!)");
        }
    }

    private void OnMeshReceived(MeshData meshData)
    {
        if (chunkDictionary.ContainsKey(meshData.coordinates))
        {
            chunkDictionary[meshData.coordinates].SetChunkMesh(meshData.meshObject);
        }
        else
        {
            Debug.LogWarning("Terrain mesh received for a chunk that does not exist in chunk dictionary." +
                " (Request mesh only once corresponding TerrainChunk has been added to the dictionary!)");
        }

    }

    private void BroadcastChunkVisibilityChange(Vector2Int chunkCoordinate, bool isVisible)
    {
        OnChunkVisibilityChange.Invoke(chunkCoordinate, isVisible);
    }

}
