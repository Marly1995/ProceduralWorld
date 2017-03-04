﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {

    public Renderer textureRenderer;
    public MeshFilter[] meshFilter = new MeshFilter[6];
    public MeshRenderer[] meshRenderer = new MeshRenderer[6];

    public Terrain terrain;

    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        //meshFilter.sharedMesh = meshData.CreateMesh();
        //meshRenderer.sharedMaterial.mainTexture = texture;
    }

    public void DrawSphere(Mesh[] mesh, int divs)
    {
        for (int i = 0; i < divs*6; i++)
        {
            meshFilter[i].sharedMesh = mesh[i];
            //meshRenderer.sharedMaterial.mainTexture = texture;
        }
    }

    public void DrawTerrain(TerrainData terrainData)
    {
        terrain.terrainData = terrainData;
    }
}
