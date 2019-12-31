using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrainController : MonoBehaviour
{

    [Tooltip("Transforms around which new terrain is spawned, according to the other variables below")]
    [SerializeField] private Transform[] viewers;
    [Header("Terrain spawning")]
    [Tooltip("What is the range (from a viewer) within which new terrain should be spawned")]
    public float viewDistance = 150f;
    [Tooltip("How far should the viewer move for a new terrain spawning to be concidered")]
    public float minViewerMoveDistanceToUpdateTerrain = 50f;
    [Tooltip("Should spawned chunks be disabled when the viewer moves further away from them than the view distance")]
    public bool disableChunksOutsideOfViewDistance = true;
    [Header("Terrain clean up")]
    [Tooltip("How far away from the viewer should a terrain chunk be at minimum to be destroyed at clean up")]
    public float minCleanUpDistance = 800f;
    [Tooltip("How far should the player move for terrain clean up to be considered")]
    public float minViewerMoveDistanceToCleanUpTerrain = 400f;
    [Tooltip("Should far away terrain chunks be destroyed when the viewer moves far enough from them")]
    public bool enableCleanUp = true;

    private TerrainManager terrainManager;
    private int chunkSize;
    private float sqrViewDistance;
    private float sqrMinViewerMoveDistanceToUpdateTerrain;
    private float sqrMinCleanUpDistance;
    private float sqrMinViewerMoveDistanceToCleanUpTerrain;
    private Vector3[] viewerPositionsAtPreviousTerrainUpdate;
    private Vector3[] viewerPositionsAtPreviousTerrainCleanUp;
    private Vector2Int[][] viewerChunks;
    private readonly int maxChunkVisibilityRadiusCheckStep = 100; // To prevent any accidential infinite while-loops

    private void Awake()
    {
        FindAndValidateReferences();
    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        TryUpdateTerrain();
        TryCleanUpTerrain();
    }

    public void SetViewers(Transform[] viewers)
    {
        this.viewers = viewers;
        Initialize();
    }

    private void FindAndValidateReferences()
    {
        terrainManager = GetComponent<TerrainManager>();

        if (terrainManager == null)
            Debug.LogError("TerrainManager not found! Please add a TerrainManager component to this game object.",
                gameObject);
    }

    private void Initialize()
    {
        chunkSize = terrainManager.ChunkSize;

        sqrViewDistance = viewDistance * viewDistance;
        sqrMinViewerMoveDistanceToUpdateTerrain
            = minViewerMoveDistanceToUpdateTerrain * minViewerMoveDistanceToUpdateTerrain;

        sqrMinCleanUpDistance = minCleanUpDistance * minCleanUpDistance;
        sqrMinViewerMoveDistanceToCleanUpTerrain
            = minViewerMoveDistanceToCleanUpTerrain * minViewerMoveDistanceToCleanUpTerrain;

        if (minViewerMoveDistanceToCleanUpTerrain < minViewerMoveDistanceToUpdateTerrain)
        {

        }
        else if(minCleanUpDistance < viewDistance)
        {
            Debug.LogWarning("minCleanUpDistance < viewDistance! " +
                "(Cleanup will remove terrain chunks withing view distance.)");
        }

        viewerChunks = new Vector2Int[viewers.Length][];
        for (int i = 0; i < viewerChunks.Length; i++)
        {
            viewerChunks[i] = new Vector2Int[0];
        }

        viewerPositionsAtPreviousTerrainUpdate = new Vector3[viewers.Length];
        for (int i = 0; i < viewerPositionsAtPreviousTerrainUpdate.Length; i++)
        {
            viewerPositionsAtPreviousTerrainUpdate[i] = viewers[i].position;
        }

        viewerPositionsAtPreviousTerrainCleanUp = new Vector3[viewers.Length];
        for (int i = 0; i < viewerPositionsAtPreviousTerrainCleanUp.Length; i++)
        {
            viewerPositionsAtPreviousTerrainCleanUp[i] = viewers[i].position;
        }

        TryUpdateTerrain(true);
        TryCleanUpTerrain(true);
    }

    private void TryUpdateTerrain(bool forceUpdate = false)
    {
        List<Vector2Int> chunksToEnable = new List<Vector2Int>();
        bool newChunksToBeEnabled = false;

        for (int i = 0; i < viewers.Length; i++)
        {
            if (forceUpdate || IsDistanceBetweenPointsOverThreshold(viewerPositionsAtPreviousTerrainUpdate[i],
                viewers[i].position, sqrMinViewerMoveDistanceToUpdateTerrain))
            {
                newChunksToBeEnabled = true;
                viewerPositionsAtPreviousTerrainUpdate[i] = viewers[i].position;
                Vector2Int[] chunksInViewDistance = CalculateChunksInRange(viewers[i].position, sqrViewDistance);
                viewerChunks[i] = chunksInViewDistance;
                chunksToEnable.AddRange(chunksInViewDistance);
            }
            else
            {
                // Keep the previously enabled chunks active if not updating
                // E.g. when updating only the chunks of a single viewer, 
                //      the other viewers' chunks also need to stay enabled
                chunksToEnable.AddRange(viewerChunks[i]);
            }
        }

        if (newChunksToBeEnabled)
        {
            terrainManager.DrawChunks(chunksToEnable.ToArray(), disableChunksOutsideOfViewDistance);
        }
    }

    private void TryCleanUpTerrain(bool forceCleanup = false)
    {
        List<Vector2Int> chunksNotToCleanUp = new List<Vector2Int>();
        bool atLeastOneViewerMovedEnoughForCleanUp = false;
        
        for (int i = 0; i < viewers.Length; i++)
        {
            if (forceCleanup || IsDistanceBetweenPointsOverThreshold(viewerPositionsAtPreviousTerrainCleanUp[i],
                viewers[i].position, sqrMinViewerMoveDistanceToCleanUpTerrain))
            {
                atLeastOneViewerMovedEnoughForCleanUp = true;
                viewerPositionsAtPreviousTerrainCleanUp[i] = viewers[i].position;
                Vector2Int[] chunksWithinRange = CalculateChunksInRange(viewers[i].position, sqrMinCleanUpDistance);
                chunksNotToCleanUp.AddRange(chunksWithinRange);
            }
            else
            {
                // Keep the previously enabled chunks active if not updating
                // E.g. when updating only the chunks of a single viewer, 
                //      the other viewers' chunks also need to stay enabled
                chunksNotToCleanUp.AddRange(viewerChunks[i]);
            }
        }

        if (atLeastOneViewerMovedEnoughForCleanUp)
        {
            terrainManager.CleanUpChunks(chunksNotToCleanUp.ToArray());
        }
    }

    private bool IsDistanceBetweenPointsOverThreshold(Vector3 oldPosition, Vector3 newPosition, float sqrThresholdDistance)
    {
        float sqrTraveledDistance = Vector3.SqrMagnitude(newPosition - oldPosition);

        return (sqrTraveledDistance >= sqrThresholdDistance);
    }

    private Vector2Int[] CalculateChunksInRange(Vector3 viewerPosition, float sqrRange)
    {
        // TODO: Implement threading to this method?

        List<Vector2Int> visibleChunks = new List<Vector2Int>();
        Vector2Int centerChunk = TerrainUtility.CalculateChunkCoordinates(viewerPosition, chunkSize);
        visibleChunks.Add(centerChunk);

        int ringCounter = 1; // Ring 0 would include only the center chunk
        bool radiusAtLeastPartiallyVisible;

        do
        {
            List<Vector2Int> chunksVisibleAtRadius = GetChunksInRangeAtRing(viewerPosition, centerChunk, sqrRange,
                                                                            ringCounter);
            visibleChunks.AddRange(chunksVisibleAtRadius);
            radiusAtLeastPartiallyVisible = (chunksVisibleAtRadius.Count > 0);
            ringCounter++;

        } while (radiusAtLeastPartiallyVisible && ringCounter <= maxChunkVisibilityRadiusCheckStep);

        return visibleChunks.ToArray();
    }

    // Calculates which chunks at given "chunk ring" are within the given range
    // e.g. ring 2 would mean all chunks which are exactly one chunk away from the center chunk
    private List<Vector2Int> GetChunksInRangeAtRing(Vector3 viewerPosition, Vector2Int centerCoordinate, float sqrRange,
                                                    int ringNumber)
    {
        List<Vector2Int> visibleChunks = new List<Vector2Int>();

        for (int x = -ringNumber; x <= ringNumber; x++)
        {
            for (int y = -ringNumber; y <= ringNumber; y++)
            {
                // Only check the tiles at the edge of the given radius
                if (Mathf.Abs(x) == ringNumber || Mathf.Abs(y) == ringNumber)
                {
                    Vector2Int coordinateToCheck = new Vector2Int(centerCoordinate.x + x, centerCoordinate.y + y);
                    float sqrViewerDistanceToChunk = TerrainUtility.CalculateSqrHorizontalDistanceToChunk(viewerPosition,
                        coordinateToCheck, chunkSize);

                    if (sqrViewerDistanceToChunk <= sqrRange)
                    {
                        visibleChunks.Add(coordinateToCheck);
                    }
                }
            }
        }

        return visibleChunks;
    }

}
