using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class DefaultTerrainMeshGenerator : MonoBehaviour, ITerrainMeshGenerator
{
    [Tooltip("Transform under which the terrain mesh objects are spawned")]
    [SerializeField] private Transform parent = null;
    [Tooltip("A multiplier used for the vertical position of the mesh vertices when placing them " +
             "according to the height map values")]
    [SerializeField] private float heightMultiplier = 30;
    [Tooltip("A curve multiplier used for the vertical position of the mesh vertices when placing them " +
             "according to the height map values")]
    [SerializeField] private AnimationCurve heightCurve = null;
    [Tooltip("Material attached to a terrain mesh when it is generated")]
    [SerializeField] private Material terrainMaterial = null;

    private Queue<ThreadInfo<MeshGenerationData>> chunkMeshThreadInfoQueue
        = new Queue<ThreadInfo<MeshGenerationData>>();

    private class MeshGenerationData
    {
        public Vector2Int coordinates;
        public int chunkSize;
        public Action<MeshData> callback;

        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;

        Vector3[] boarderVertices;
        int[] boarderTriangles;

        int triangleIndex = 0;
        int boarderTriangleIndex = 0;

        public MeshGenerationData(Vector2Int coordinates, int chunkSize, Action<MeshData> callback, int verticesPerLine)
        {
            this.coordinates = coordinates;
            this.chunkSize = chunkSize;
            this.callback = callback;

            vertices = new Vector3[verticesPerLine * verticesPerLine];
            triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];
            uvs = new Vector2[verticesPerLine * verticesPerLine];

            boarderVertices = new Vector3[verticesPerLine * 4 + 4];
            boarderTriangles = new int[24 * verticesPerLine];

        }

        public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
        {
            if (vertexIndex < 0)
            {
                boarderVertices[-vertexIndex - 1] = vertexPosition;
            }
            else
            {
                vertices[vertexIndex] = vertexPosition;
                uvs[vertexIndex] = uv;
            }
        }

        public void AddTriangle(int a, int b, int c)
        {
            if (a < 0 || b < 0 || c < 0)
            {
                boarderTriangles[boarderTriangleIndex] = a;
                boarderTriangles[boarderTriangleIndex + 1] = b;
                boarderTriangles[boarderTriangleIndex + 2] = c;
                boarderTriangleIndex += 3;
            }
            else
            {
                triangles[triangleIndex] = a;
                triangles[triangleIndex + 1] = b;
                triangles[triangleIndex + 2] = c;
                triangleIndex += 3;
            }
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            return mesh;
        }
    }

    private void Update()
    {
        // Check for any finished chunk meshes
        if (chunkMeshThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < chunkMeshThreadInfoQueue.Count; i++)
            {
                ThreadInfo<MeshGenerationData> threadInfo = chunkMeshThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    public void RequestMesh(Vector2Int coordinates, int chunkSize, float[,] heightMap,
                                    Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshGenerationThread(coordinates, chunkSize, heightMap, callback);
        };

        new Thread(threadStart).Start();
    }

    private void MeshGenerationThread(Vector2Int coordinates, int chunkSize, float[,] heightMap,
                                         Action<MeshData> callback)
    {
        // TODO: Try to get GenerateMeshAlt() working here without seams / gaps between mesh chunks
        MeshGenerationData meshData = GenerateMeshAlt(coordinates, chunkSize, heightMap, callback);
        lock (chunkMeshThreadInfoQueue)
        {
            // Store the generated height map to the queue
            chunkMeshThreadInfoQueue.Enqueue(new ThreadInfo<MeshGenerationData>(OnChunkMeshGenerated, meshData));
        }
    }

    private MeshGenerationData GenerateMesh(Vector2Int coordinates, int chunkSize, float[,] heightMap,
                                       Action<MeshData> callback)
    {
        int levelOfDetail = 0;
        int meshSize = heightMap.GetLength(0);
        float topLeftX = (meshSize - 1) / -2f;
        float topLeftZ = (meshSize - 1) / 2f;

        int meshSimplificationIncrement = (levelOfDetail == 0)
            ? 1
            : levelOfDetail * 2;
        int verticesPerLine = (meshSize - 1) / meshSimplificationIncrement + 1;

        MeshGenerationData meshGenData = new MeshGenerationData(coordinates, chunkSize, callback, verticesPerLine);
        int vertexIndex = 0;

        for (int x = 0; x < meshSize; x += meshSimplificationIncrement)
        {
            for (int y = 0; y < meshSize; y += meshSimplificationIncrement)
            {
                float height = heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier;
                meshGenData.vertices[vertexIndex] = new Vector3(topLeftX + x, height, topLeftZ + y - meshSize);
                meshGenData.uvs[vertexIndex] = new Vector2(x / (float)meshSize, y / (float)meshSize);

                if (x < meshSize - 1 && y < meshSize - 1)
                {
                    meshGenData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshGenData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshGenData;
    }

    private void OnChunkMeshGenerated(MeshGenerationData meshGenData)
    {
        meshGenData.callback(new MeshData(meshGenData.coordinates, GenerateMeshObject(meshGenData)));
    }

    private GameObject GenerateMeshObject(MeshGenerationData meshData)
    {
        Mesh mesh = meshData.CreateMesh();
        GameObject meshObject = new GameObject("TerrainMesh");
        meshObject.AddComponent<MeshFilter>().mesh = mesh;
        meshObject.AddComponent<MeshCollider>().sharedMesh = mesh;
        meshObject.AddComponent<MeshRenderer>().sharedMaterial = terrainMaterial;

        Transform meshObjectTransform = meshObject.transform;
        meshObjectTransform.SetParent(parent);
        meshObjectTransform.localPosition = TerrainUtility.CalculateChunkWorldPosition(meshData.coordinates,
                                                                                       meshData.chunkSize);

        return meshObject;
    }

    private MeshGenerationData GenerateMeshAlt(Vector2Int coordinates, int chunkSize, float[,] heightMap,
                                              Action<MeshData> callback)
    {
        int levelOfDetail = 0;
        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;

        int boarderedSize = heightMap.GetLength(0);
        int meshSize = boarderedSize * meshSimplificationIncrement;
        int meshSizeUnsimplified = boarderedSize;

        float topLeftX = (meshSizeUnsimplified - 1) / -2f;
        float topLeftZ = (meshSizeUnsimplified - 1) / 2f;

        int verticesPerLine = (meshSize - 1) / meshSimplificationIncrement + 1;

        MeshGenerationData meshGenData = new MeshGenerationData(coordinates, chunkSize, callback, verticesPerLine);

        int[,] vertexIndicesMap = new int[boarderedSize, boarderedSize];
        int meshVertexIndex = 0;
        int boarderVertexIndex = -1;

        for (int x = 0; x < boarderedSize; x += meshSimplificationIncrement)
        {
            for (int y = 0; y < boarderedSize; y += meshSimplificationIncrement)
            {
                bool isBoarderVertex = (y == 0 || y == boarderedSize - 1
                    || x == 0 || x == boarderedSize - 1);

                if (isBoarderVertex)
                {
                    vertexIndicesMap[x, y] = boarderVertexIndex;
                    boarderVertexIndex--;
                }
                else
                {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }

        for (int x = 0; x < boarderedSize; x += meshSimplificationIncrement)
        {
            for (int y = 0; y < boarderedSize; y += meshSimplificationIncrement)
            {
                int vertexIndex = vertexIndicesMap[x, y];
                Vector2 percent = new Vector2((x - meshSimplificationIncrement) / (float)meshSize,
                    (y - meshSimplificationIncrement) / (float)meshSize);
                float height = heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier;

                Vector3 vertexPosition =
                    new Vector3(topLeftX + percent.x * meshSizeUnsimplified, height,
                    topLeftZ + percent.y * meshSizeUnsimplified - meshSizeUnsimplified);

                meshGenData.AddVertex(vertexPosition, percent, vertexIndex);

                if (x < boarderedSize - 1 && y < boarderedSize - 1)
                {
                    int a = vertexIndicesMap[x, y];
                    int b = vertexIndicesMap[x + meshSimplificationIncrement, y];
                    int c = vertexIndicesMap[x, y + meshSimplificationIncrement];
                    int d = vertexIndicesMap[x + meshSimplificationIncrement, y + meshSimplificationIncrement];

                    //meshGenData.AddTriangle(a, d, c);
                    //meshGenData.AddTriangle(d, a, b);
                    meshGenData.AddTriangle(a, c, d);
                    meshGenData.AddTriangle(d, b, a);
                }

                vertexIndex++;
            }
        }

        //meshData.FinalizeMesh();

        return meshGenData;
    }
}
