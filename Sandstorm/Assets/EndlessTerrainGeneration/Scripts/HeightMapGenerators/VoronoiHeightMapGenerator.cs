using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;

public class VoronoiHeightMapGenerator : MonoBehaviour, IHeightMapGenerator
{
    [Tooltip("Used to normalize the height map values between 0 and 1.")]
    [SerializeField] private int maxDistanceFromSeedPoints = 80;
    [Tooltip("How many cells should a single height map chunk contain")]
    [SerializeField] private int cellCount = 16;
    [Tooltip("Adds extra rows and columns to the generated height map (Generating a mesh from the height map requires " +
             "an additional row and column, so neighbouring the vertices of adjecent meshes match)")]
    [SerializeField] private int extraHeightMapSize = 1;

    private System.Random masterRandom = new System.Random();
    private Dictionary<Vector2Int, Vector2Int[]> voronoiSeedsDictionary = new Dictionary<Vector2Int, Vector2Int[]>();
    private Queue<ThreadInfo<HeightMapData>> heightMapThreadInfoQueue = new Queue<ThreadInfo<HeightMapData>>();

    private void Update()
    {
        // Check for any finished height maps and send the height map to the given callback method
        if (heightMapThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < heightMapThreadInfoQueue.Count; i++)
            {
                ThreadInfo<HeightMapData> threadInfo = heightMapThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    public void RequestHeightMap(Vector2Int coordinates, int chunkSize, Action<HeightMapData> callback)
    {
        // Create and start a new thread for generating the height map
        ThreadStart threadStart = delegate
        {
            HeightMapGenerationThread(coordinates, chunkSize + extraHeightMapSize, callback, masterRandom);
        };

        new Thread(threadStart).Start();
    }

    private void HeightMapGenerationThread(Vector2Int coordinates, int chunkSize, Action<HeightMapData> callback,
                                         System.Random random)
    {
        HeightMapData heightMap = GenerateHeightMap(coordinates, chunkSize, random);
        lock (heightMapThreadInfoQueue)
        {
            // Store the generated height map to the queue
            heightMapThreadInfoQueue.Enqueue(new ThreadInfo<HeightMapData>(callback, heightMap));
        }
    }

    private HeightMapData GenerateHeightMap(Vector2Int coordinates, int chunkSize, System.Random random)
    {
        float[,] heightMap = new float[chunkSize, chunkSize];
        float[,] distances = new float[chunkSize, chunkSize];

        Vector2Int[] seeds = GetSeedsForChunkAndAdjecentCoordinates(coordinates, chunkSize - extraHeightMapSize, random);

        // Calculate distance to closest voronoi seed for each height map point to create voronoi pattern
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                distances[x, y]
                    = Vector2.Distance(new Vector2(x, y), GetClosestSeed(new Vector2Int(x, y), seeds));
            }
        }

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                float heightValue = distances[x, y] / maxDistanceFromSeedPoints;
                heightMap[x, y] = heightValue;
            }
        }

        return new HeightMapData(coordinates, heightMap);
    }

    private Vector2Int GetClosestSeed(Vector2Int position, Vector2Int[] seeds)
    {
        float sqrShortestDistance = float.MaxValue;
        int index = 0;
        for (int i = 0; i < seeds.Length; i++)
        {
            float sqrDistance = Vector2.SqrMagnitude(position - seeds[i]);
            if (sqrDistance < sqrShortestDistance)
            {
                sqrShortestDistance = sqrDistance;
                index = i;
            }
        }

        return seeds[index];
    }

    private Vector2Int[] GetSeedsForChunkAndAdjecentCoordinates(Vector2Int coordinates, int chunkSize,
                                                           System.Random random, int adjecencyRange = 1)
    {
        List<Vector2Int> nearbySeeds = new List<Vector2Int>();

        lock (voronoiSeedsDictionary)
        {
            // Check the chunk at given coordinates, and all the chunks around it
            //  - If adjecencyRange = 1, that means getting the seeds of 9 chunks in total
            for (int x = -adjecencyRange; x <= adjecencyRange; x++)
            {
                for (int y = -adjecencyRange; y <= adjecencyRange; y++)
                {
                    Vector2Int coordinateToCheck = new Vector2Int(coordinates.x + x, coordinates.y + y);
                    Vector2Int[] chunkSeeds;
                    if (voronoiSeedsDictionary.ContainsKey(coordinateToCheck))
                    {
                        // Use existing seeds
                        chunkSeeds = voronoiSeedsDictionary[coordinateToCheck];
                    }
                    else
                    {
                        // Generate new seeds
                        chunkSeeds = new Vector2Int[cellCount];
                        for (int i = 0; i < cellCount; i++)
                        {
                            chunkSeeds[i] = new Vector2Int(random.Next(0, chunkSize - 1), random.Next(0, chunkSize - 1));
                        }

                        voronoiSeedsDictionary.Add(coordinateToCheck, chunkSeeds);
                    }

                    // Offset the seed positions to reflect their position related to the central chunk
                    Vector2Int[] offsetSeeds = new Vector2Int[chunkSeeds.Length];
                    for (int i = 0; i < offsetSeeds.Length; i++)
                    {
                        offsetSeeds[i] = new Vector2Int(chunkSeeds[i].x + x * chunkSize, chunkSeeds[i].y + y * chunkSize);
                    }

                    nearbySeeds.AddRange(offsetSeeds);
                }
            }
        }

        return nearbySeeds.ToArray();
    }
}
