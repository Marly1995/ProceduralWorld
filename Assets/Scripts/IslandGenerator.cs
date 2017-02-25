using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public class IslandGenerator : MonoBehaviour {

    public enum DrawMode { NoiseMap, ColorMap, Mesh, Terrain };
    public DrawMode drawMode;

    public const int mapChunkSize = 241;
    [Range(0,6)]
    public int previewLOD;
    public float noiseScale;

    public int sheets;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightmultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainType[] regions;

    Queue<MapThreadInfo<MapData>> mapDataThreadQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadQueue = new Queue<MapThreadInfo<MeshData>>();

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData();

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap) { display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap)); }
        else if (drawMode == DrawMode.ColorMap) { display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize)); }
        else if (drawMode == DrawMode.Mesh) { display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightmultiplier, meshHeightCurve, previewLOD), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize)); }
        else if (drawMode == DrawMode.Terrain) { display.DrawTerrain(TerrainGenerator.GenerateTerrain(mapData.heightMap, meshHeightmultiplier)); }
    }

    public void RequestMapData(Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Action<MapData> callback)
    {
        MapData mapData = GenerateMapData();
        lock (mapDataThreadQueue)
        {
            mapDataThreadQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, lod, callback);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightmultiplier, meshHeightCurve, lod);
        lock(meshDataThreadQueue)
        {
            meshDataThreadQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    void Update()
    {
        if(mapDataThreadQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    MapData GenerateMapData()
    {
        float[,] noiseMap = NoiseGeneration.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, sheets, persistance, lacunarity, offset);


        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = regions[i].color;
                        break;
                    }
                }
            }
        }
        return new global::MapData(noiseMap, colorMap);
    }

    void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (sheets < 0)
        {
            sheets = 0;
        }

    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }

    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}


public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;
    
    public MapData (float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}