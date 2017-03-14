using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public class IslandGenerator : MonoBehaviour {

    public enum DrawMode { NoiseMap, ColorMap, Mesh, Sphere };
    public DrawMode drawMode;

    public const int mapChunkSize = 241;

	[Range(0,2)]
	public int divisions;
	[Range(10,250)]
	public int gridSize;
	
    public float noiseScale;

    public int sheets;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightmultiplier;
    public AnimationCurve meshHeightCurve;

	public bool falloff;
	[Range(0, 10)]
	public float falloff_a;
	[Range(0, 10)]
	public float falloff_b;
	[Range(0, 1)]
	public float falloffHeight;
	[Range(10, 100)]
	public int halfIslandSize;
	public int islandNumber = 5;

	public bool autoUpdate;

    public TerrainType[] regions;
	float[,] falloffMap;

    Queue<MapThreadInfo<MapData>> mapDataThreadQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadQueue = new Queue<MapThreadInfo<MeshData>>();

    public void DrawMapInEditor()
    {		
		MapData[] mapData = new MapData[6];
        int i = ((int)Mathf.Pow(4, divisions));
        for (int j = 0; j < 6; j++)
        {
			int k = (int)Mathf.Pow(i, 0.5f);
            mapData[j] = GenerateMapData(new Vector2(0, (j*gridSize*k)+1), (gridSize * k)+1);
        }		

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap) { display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData[0].heightMap)); }
        else if (drawMode == DrawMode.ColorMap) { display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData[0].colorMap, mapChunkSize, mapChunkSize)); }
        //else if (drawMode == DrawMode.Mesh) { display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData[0].heightMap, meshHeightmultiplier, meshHeightCurve, previewLOD), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize)); }
        else if (drawMode == DrawMode.Sphere) { display.DrawSphere(sphereMapping.getSphere(i, gridSize, mapData, meshHeightmultiplier, meshHeightCurve, regions), i); }
    }

    public void RequestMapData(Vector2 centre, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(centre, callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 centre, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(centre, 5);
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

    bool checkIslandOverlap(List<Vector2> islands, int x, int y)
    {
        for (int i = 0; i < islands.Count; i++)
        {
            if (x < (islands[i].x + halfIslandSize*2) &&
                x > (islands[i].x - halfIslandSize*2))
            {
                if (y < (islands[i].y + halfIslandSize * 2) &&
                    y > (islands[i].y - halfIslandSize * 2))
                {
                    return false;
                }
            }
        }
        return true;
    }

    MapData GenerateMapData(Vector2 centre, int Size)
    {
        float[,] noiseMap = NoiseGeneration.GenerateNoiseMap(Size, Size, seed, noiseScale, sheets, persistance, lacunarity, centre + offset);
        float[,] islandMap = new float[Size, Size];
		float[,] fallMap = FalloffGenerator.GenerateFalloff((halfIslandSize*2)+1, falloff_a, falloff_b);
        List<Vector2> islands = new List<Vector2>();
        List<float> heights = new List<float>();

		Color[] colorMap = new Color[Size * Size];
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
				if(falloff)
				{
					noiseMap[x, y] = Mathf.Clamp01(noiseMap[x,y] - falloffMap[x, y]);
				}
                float currentHeight = noiseMap[x, y];
                if (currentHeight >= falloffHeight)
                {
                    islands.Add(new Vector2(x, y));
                    heights.Add(currentHeight);                      
                }                        
				for (int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colorMap[y * Size + x] = regions[i].color;
                        break;
                    }
                }
            }
        }


		for (int i = 0; i < islands.Count; i++)
		{
			int x = (int)islands[i].x;
			int y = (int)islands[i].y;
            for (int o = -halfIslandSize; o < halfIslandSize; o++)
			{
				for (int p = -halfIslandSize; p < halfIslandSize; p++)
				{
					if (y + o > 0 && y + o < Size && x + p > 0 && x + p < Size)
					{						
						noiseMap[x + p, y + o] = Mathf.Clamp01(noiseMap[x + p, y + o] - fallMap[p + halfIslandSize, o + halfIslandSize]);
                        islandMap[x + p, y + o] = noiseMap[x + p, y + o];

                    }
				}
			}
		}
        return new global::MapData(islandMap, colorMap);
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
		int i = ((int)Math.Pow(4, divisions));
		i = (int)Mathf.Pow(i, 0.5f);
		falloffMap = FalloffGenerator.GenerateFalloff((gridSize * i) + 1, falloff_a, falloff_b);

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
    public float[,] heightMap;
    public Color[] colorMap;
    
    public MapData (float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}

public struct SegmentData
{
    public Mesh mesh;
    public Texture2D texture;

    public SegmentData(Mesh mesh, Texture2D texture)
    {
        this.mesh = mesh;
        this.texture = texture;
    }
}