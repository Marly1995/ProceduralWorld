using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;

public static class NoiseGeneration
{
    public enum NormailizeMode { Local, Global };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int sheets, float persistance, float lacunarity, Vector2 offset)//, NormailizeMode normalizeMode)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random rndGen = new System.Random(seed);

        Vector2[] sheetOffsets = new Vector2[sheets];

        float maxPossibleHeight = 0;
        float amplitude = 1.0f;
        float frequency = 1.0f;

        for (int i = 0; i < sheets; i++)
        {
            float offsetX = rndGen.Next(-100000, 100000) + offset.x;
            float offsetY = rndGen.Next(-100000, 100000) - offset.y;
            sheetOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2.0f;
        float halfHeight = mapHeight / 2.0f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1.0f;
                frequency = 1.0f;
                float noiseHeight = 0.0f;

                for (int i = 0; i < sheets; i++)
                {
                    float sampleX = (x - halfWidth + sheetOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + sheetOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                //if (normalizeMode == NormailizeMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }
        }
        return noiseMap;
    }

    public static float[,] GenerateLibNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int sheets, float persistance, float lacunarity, Vector2 offset, float voronoi, float perlin, float ridged, float billow)
    {
        Const rmfConst = new Const(ridged);
        RidgedMultifractal rmf = new RidgedMultifractal(scale, lacunarity, sheets, seed, QualityMode.High);

        Const billConst = new Const(billow);
        Billow bill = new Billow(scale, lacunarity, persistance, sheets, seed, QualityMode.High);

        Const vorConst = new Const(voronoi);
        Voronoi vor = new Voronoi(scale, 1f, seed, true);

        Const perlinConst = new Const(perlin);
        Perlin per = new Perlin(scale, lacunarity, persistance, sheets, seed, QualityMode.High);
        
        var gen1 = new Add(new Multiply(per, perlinConst), new Multiply(vor, vorConst));
        var gen2 = new Add(new Multiply(rmf, rmfConst), new Multiply(bill, billConst));
        var finalGenerator = new Add(gen1, gen2);

        Noise2D noiseGenerator = new Noise2D(mapWidth, mapHeight, finalGenerator);

        noiseGenerator.GeneratePlanar(0f, 1f, 0f, 1f);

        return noiseGenerator.GetData();
    }
}
