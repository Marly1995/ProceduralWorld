using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {

    public Renderer textureRenderer;
    public MeshFilter[] meshFilter = new MeshFilter[6];
    public MeshRenderer[] meshRenderer = new MeshRenderer[6];

    public Terrain terrain;

    public Shader shader;

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

    public void DrawSphere(SegmentData[] segments, int divs)
    {
        for (int i = 0; i < segments.Length; i++)
        {
            meshFilter[i].sharedMesh = segments[i].mesh;
            meshRenderer[i].sharedMaterial = new Material(shader);
            meshRenderer[i].sharedMaterial.SetTexture("_MainTex", segments[i].texture);
        }
		PopulatWorld world = FindObjectOfType<PopulatWorld> ();
		world.Populate (segments);
	}

    public void DrawTerrain(TerrainData terrainData)
    {
        terrain.terrainData = terrainData;
    }
}
