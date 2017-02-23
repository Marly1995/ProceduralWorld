using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator
{
    public static float length = 500.0f;
    public static float height = 500.0f;

    public static Texture2D GrassTexture;
    public static Texture2D RockTexture;

    public static TerrainData GenerateTerrain(float[,] heightMap, float heightMultiplier)
    {
        TerrainData terrainData = new TerrainData();
        terrainData.alphamapResolution = 129;

        // heights
        terrainData.heightmapResolution = 129;
        terrainData.SetHeights(0, 0, heightMap);
        terrainData.size = new Vector3(length, height * heightMultiplier, length);

        // textures
        SplatPrototype Grass = new SplatPrototype();
        SplatPrototype Rocks = new SplatPrototype();

        Grass.texture = GrassTexture;
        Grass.tileSize = new Vector2(4f, 4f);
        Rocks.texture = RockTexture;
        Rocks.tileSize = new Vector2(4f, 4f);

        terrainData.splatPrototypes = new SplatPrototype[] { Grass, Rocks };
        terrainData.RefreshPrototypes();
        terrainData.SetAlphamaps(0, 0, MakeSplatMap(terrainData));

        return terrainData;
    }

    public static float[,,] MakeSplatMap(TerrainData terrainData)
    {
        float[,,] splatmap = new float[129, 129, 2];

        for (int x = 0; x < 129; x++)
        {
            for (int z = 0; z < 129; z++)
            {
                float normX = (float)x / 128.0f;
                float normZ = (float)z / 128.0f;

                float steepness = terrainData.GetSteepness(normX, normZ) / 90.0f;

                splatmap[z, x, 0] = 1.0f - steepness;
                splatmap[z, x, 1] = steepness;
            }
        }
        return splatmap;
    }
}
