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

        int triangleIndex;

        public MeshGenerationData(Vector2Int coordinates, int chunkSize, Action<MeshData> callback,
                        int meshWidth, int meshHeight)
        {
            this.coordinates = coordinates;
            this.chunkSize = chunkSize;
            this.callback = callback;
            vertices = new Vector3[meshWidth * meshHeight];
            uvs = new Vector2[meshWidth * meshHeight];
            triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        }

        public void AddTriangle(int a, int b, int c)
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
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
        MeshGenerationData meshData = GenerateMesh(coordinates, chunkSize, heightMap, callback);
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
        int width = heightMap.GetLength(0);
        int length = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (length - 1) / 2f;

        int meshSimplificationIncrement = (levelOfDetail == 0)
            ? 1
            : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

        MeshGenerationData meshGenData = new MeshGenerationData(coordinates, chunkSize, callback,
                                                                           verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        for (int x = 0; x < width; x += meshSimplificationIncrement)
        {
            for (int y = 0; y < length; y += meshSimplificationIncrement)
            {
                meshGenData.vertices[vertexIndex]
                    = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier,
                                  topLeftZ + y - length);

                meshGenData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)length);

                if (x < width - 1 && y < length - 1)
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
}
