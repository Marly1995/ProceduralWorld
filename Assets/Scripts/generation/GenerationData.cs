using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationData
{
    public int seed;

    public int gridSize;

    public float noiseScale;

    public int sheets;
    public float persistance;
    public float lacunarity;

    public float meshHeightmultiplier;
    public AnimationCurve meshHeightCurve;
    
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

    public SeamLessTerrainData[] regions;
    float[,] falloffMap;
}