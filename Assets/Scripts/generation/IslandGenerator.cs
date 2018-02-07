using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public class IslandGenerator : MonoBehaviour {

    public LPWAsset.LowPolyWaterScript waterScript;
    public Material sea;
    public enum DrawMode { NoiseMap, ColorMap, Mesh, Sphere };
    public DrawMode drawMode;

    public float startTime;
    public float endTime;

    public const int mapChunkSize = 241;

	[Range(0,2)]
	public int divisions;
	[Range(4,250)]
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
    public bool makeIslands;
	[Range(0, 10)]
	public float islandFallof_a;
	[Range(0, 10)]
	public float islandFalloff_b;

    [Range(0, 10)]
    public float falloff_a;
    [Range(0, 10)]
    public float falloff_b;

    [Range(0, 1)]
	public float falloffHeight;
	[Range(4, 250)]
	public int halfIslandSize;

	public bool autoUpdate;
    public bool flat_shading;

    public bool randomColors;
    public bool randomize;

    public TerrainType[] regions;
	float[,] falloffMap;

    public ColorManager colMan;

    private void Start()
    {
        
        startTime = Time.deltaTime;
        int i = ((int)Math.Pow(4, divisions));
        i = (int)Mathf.Pow(i, 0.5f);
        falloffMap = FalloffGenerator.GenerateFalloff((gridSize * i) + 1, falloff_a, falloff_b);

        MapData[] mapData = new MapData[6];
        for (int j = 0; j < 6; j++)
        {
            mapData[j] = GenerateMapData(new Vector2(0, (j * gridSize * i) + 1), (gridSize * i) + 1);
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap) { display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData[0].heightMap)); }
        else if (drawMode == DrawMode.ColorMap) { display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData[0].colorMap, mapChunkSize, mapChunkSize)); }
        //else if (drawMode == DrawMode.Mesh) { display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData[0].heightMap, meshHeightmultiplier, meshHeightCurve, previewLOD), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize)); }
        else if (drawMode == DrawMode.Sphere) { display.DrawSphere(sphereMapping.getSphere(i, gridSize, mapData, meshHeightmultiplier, meshHeightCurve, regions, flat_shading), i); }

        endTime = Time.deltaTime;
        //Debug.Log(endTime - startTime);
    }

    private void Update()
    {
    }
    public void DrawMapInEditor()
    {
        if (randomColors)
        {
            RandomizeColors();
        }
        if (randomize)
        {
            seed = UnityEngine.Random.Range(-10000, 10000);
        }
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
        else if (drawMode == DrawMode.Sphere) { display.DrawSphere(sphereMapping.getSphere(i, gridSize, mapData, meshHeightmultiplier, meshHeightCurve, regions, flat_shading), i); }
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
		float[,] fallMap = FalloffGenerator.GenerateFalloff((halfIslandSize*2)+1, islandFallof_a, islandFalloff_b);
        List<Vector2> islands = new List<Vector2>();
        List<float> heights = new List<float>();

        Color[] colorMap = new Color[Size * Size];
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                if (falloff)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }
                float currentHeight = noiseMap[x, y];
                if (currentHeight >= falloffHeight &&
                    checkIslandOverlap(islands, x, y))
                {
                    islands.Add(new Vector2(x, y));
                    heights.Add(currentHeight);
                }
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colorMap[y * Size + x] = regions[i].color;
                        break;
                    }
                }
            }
        }
        if (makeIslands)
        {
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
        
        return new global::MapData(noiseMap, colorMap);
	}

    public void RandomizeColors()
    {
        int coln = UnityEngine.Random.Range(0, 6);
        Color col = colMan.colors[coln];
        Color water = colMan.compliments[coln];
        col.a = 1.0f;
        for (int i = 0; i < regions.Length; i++)
        {
            col.r += UnityEngine.Random.Range(0.2f, 0.4f)-0.4f;
            col.g += UnityEngine.Random.Range(0.2f, 0.4f)-0.4f;
            col.b += UnityEngine.Random.Range(0.2f, 0.4f)-0.4f;
            col.a -= 0.1f;
            regions[i].color = col;
            regions[i].iceColor = regions[i].color;
        }
        sea.SetColor("_Color", water);
        sea.SetColor("_DeepColor", water);
        waterScript.material = sea;
    }

    public void RandomizeHeights()
    {
        //float half = meshHeightmultiplier / 2;
        //meshHeightmultiplier = UnityEngine.Random.Range(meshHeightmultiplier-half, meshHeightmultiplier+half);
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
    [Range(0, 1)]
    public float height;
    public Color color;
    public Color iceColor;
    public float slope;
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
    public Mesh collider;
    public Texture2D texture;
	public MapData MapData;


	public SegmentData(Mesh mesh, Mesh collider, Texture2D texture, Color[] colorMap, float[,] heightMap)
    {
        this.mesh = mesh;
        this.texture = texture;
		this.MapData = new MapData(heightMap, colorMap);
        this.collider = collider;
    }
}