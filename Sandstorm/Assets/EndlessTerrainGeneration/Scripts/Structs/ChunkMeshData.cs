using UnityEngine;

public struct MeshData
{
    public readonly Vector2Int coordinates;
    public readonly GameObject meshObject;      // TODO: Change this to use mesh instead of a game object!

    public MeshData(Vector2Int coordinate, GameObject meshObject)
    {
        this.coordinates = coordinate;
        this.meshObject = meshObject;
    }
}
