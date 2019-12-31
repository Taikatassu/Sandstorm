using UnityEngine;
using System;

public interface IHeightMapGenerator
{
    void RequestHeightMap(Vector2Int coordinates, int chunkSize, Action<HeightMapData> callback);
}
