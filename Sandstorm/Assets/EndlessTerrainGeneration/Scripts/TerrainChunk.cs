using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TerrainChunk
{
    Vector2Int coordinates;
    GameObject meshObject;
    bool chunkState = false;
    Action<Vector2Int, bool> chunkStateChangeCallback;

    public TerrainChunk(Vector2Int coordinates, Action<Vector2Int, bool> chunkStateChangeCallback)
    {
        this.coordinates = coordinates;
        this.chunkStateChangeCallback = chunkStateChangeCallback;
    }

    public void SetChunkState(bool enabled)
    {
        chunkState = enabled;
        UpdateChunk();
    }

    public void SetChunkMesh(GameObject meshObject)
    {
        this.meshObject = meshObject;
        UpdateChunk();
    }

    public void Clean()
    {
        if (meshObject != null)
        {
            GameObject.Destroy(meshObject);
        }
    }

    private void UpdateChunk()
    {
        if (meshObject != null)
        {
            if (meshObject.activeSelf != chunkState)
            {
                meshObject.SetActive(chunkState);

                // Send out an event when the chunk mesh active state changes
                chunkStateChangeCallback(coordinates, chunkState);
            }
        }
    }
}
