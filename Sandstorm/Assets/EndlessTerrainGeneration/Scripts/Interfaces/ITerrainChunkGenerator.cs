using UnityEngine;
using System;

public interface ITerrainMeshGenerator
{
    void RequestMesh(Vector2Int coordinates, int chunkSize, float[,] heightMap, Action<MeshData> callback);
}
