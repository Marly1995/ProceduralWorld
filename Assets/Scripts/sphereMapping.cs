﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class sphereMapping : MonoBehaviour {

    public static SegmentData[] getSphere(int divs, int gridSize, MapData[] heightMap, float heightMultiplier, AnimationCurve _heightCurve, TerrainType[] regions)
    {
    float radius = 20;

    return Generate(divs, gridSize, radius, heightMap, heightMultiplier, _heightCurve, regions);
    }

    public static SegmentData[] Generate(int divs, int gridSize, float radius, MapData[] heightMap, float heightMultiplier, AnimationCurve _heightCurve, TerrainType[] regions)
    {
        SegmentData[] segments = GenerateVerts(divs, gridSize, radius, heightMap, heightMultiplier, _heightCurve, regions);

        for (int i = 0; i < segments.Length; i++)
        {
            Mesh temp = new Mesh();
			temp.vertices = segments[i].mesh.vertices;
			temp.normals = segments[i].mesh.normals;			
			temp = GenerateTris(gridSize, segments[i].mesh.vertices, divs);
			temp.uv = segments[i].mesh.uv;
			segments[i].mesh = temp;
            segments[i].mesh.RecalculateNormals();
        }
        for (int i = 0; i < segments.Length; i++)
        {
            if (i >= divs * 1 && i < divs * 3)
            {
                segments[i].mesh.triangles = segments[i].mesh.triangles.Reverse().ToArray();
            }
            if (i >= divs * 5 && i < divs * 6)
            {
                segments[i].mesh.triangles = segments[i].mesh.triangles.Reverse().ToArray();
            }
        }

        return segments;
    }

    private static SegmentData[] GenerateVerts(int divs, int gridSize, float radius, MapData[] heightMap, float heightMultiplier, AnimationCurve _heightCurve, TerrainType[] regions)
    {
        SegmentData[] segments = new SegmentData[divs*6];
        int inc = (int)Mathf.Pow(divs, 0.5f);        
        int index = 0;
        // front
        for (int x = 0; x < inc; x++)
        {
            for (int y = 0; y < inc; y++)
            {
                segments[index] = XyPlane(inc, gridSize, radius, heightMap[0].heightMap, heightMultiplier, _heightCurve, regions, x * gridSize, y * gridSize, gridSize * (int)Mathf.Pow(divs, 0.5f));                
                index++;                    
            }               
        }
        // right
        for (int x = 0; x < inc; x++)
        {
            for (int y = 0; y < inc; y++)
            {
                segments[index] = ZyPlane(inc, gridSize, radius, heightMap[1].heightMap, heightMultiplier, _heightCurve, regions, x * gridSize, y * gridSize, 0);
                index++;
            }
        }
        // back
        for (int x = 0; x < inc; x++)
        {
            for (int y = 0; y < inc; y++)
            {
                segments[index] = XyPlane(inc, gridSize, radius, heightMap[2].heightMap, heightMultiplier, _heightCurve, regions, x * gridSize, y * gridSize, 0);
                index++;
            }
        }
        // left
        for (int x = 0; x < inc; x++)
        {
            for (int y = 0; y < inc; y++)
            {
                segments[index] = ZyPlane(inc, gridSize, radius, heightMap[3].heightMap, heightMultiplier, _heightCurve, regions, x * gridSize, y * gridSize, gridSize * (int)Mathf.Pow(divs, 0.5f));
                index++;
            }
        }
        // bot
        for (int x = 0; x < inc; x++)
        {
            for (int y = 0; y < inc; y++)
            {
                segments[index] = XzPlane(inc, gridSize, radius, heightMap[4].heightMap, heightMultiplier, _heightCurve, regions, x * gridSize, y * gridSize, 0);
                index++;
            }
        }
        //top
        for (int x = 0; x < inc; x++)
        {
            for (int y = 0; y < inc; y++)
            {
                segments[index] = XzPlane(inc, gridSize, radius, heightMap[5].heightMap, heightMultiplier, _heightCurve, regions, x * gridSize, y * gridSize, gridSize * (int)Mathf.Pow(divs, 0.5f));
                index++;
            }
        }
        return segments;
    }

    private static SegmentData XyPlane(int inc, int gridSize, float radius, float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, TerrainType[] regions, int yStart, int xStart, int z)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(gridSize+1)*(gridSize+1)];
        Vector3[] normals = new Vector3[vertices.Length];
		Vector2[] uvs = new Vector2[vertices.Length];
		int i = 0;
        for (int y = 0; y <= gridSize; y++)
        {
            for (int x = 0; x <= gridSize; x++)
            {
				uvs[i] = new Vector2(x / (float)gridSize, y / (float)gridSize);
				SetVertex(inc, gridSize, radius, heightMap, heightMultiplier, _heightCurve, normals, vertices, i++, x + xStart, y + yStart, z, y + yStart, x + xStart);				
			}
        }
        Color[] colorMap = new Color[gridSize * gridSize];
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                float currentHeight = heightMap[x+xStart, y+yStart];
                for (int j = 0; j < regions.Length; j++)
                {
                    if (currentHeight <= regions[j].height)
                    {
                        colorMap[y*gridSize +x] = regions[j].color;                     
                        break;                        
                    }
                }
            }
        }
        mesh.vertices = vertices;
        mesh.normals = normals;
		mesh.uv = uvs;		
		SegmentData segment = new SegmentData(mesh, TextureGenerator.TextureFromColorMap(colorMap, gridSize, gridSize));
        return segment;
    }

    private static SegmentData XzPlane(int inc, int gridSize, float radius, float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, TerrainType[] regions, int zStart, int xStart, int y)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(gridSize + 1) * (gridSize + 1)];
        Vector3[] normals = new Vector3[vertices.Length];
		Vector2[] uvs = new Vector2[vertices.Length];
		int i = 0;
        for (int z = 0; z <= gridSize; z++)
        {
            for (int x = 0; x <= gridSize; x++)
            {
				uvs[i] = new Vector2(x / (float)gridSize, z / (float)gridSize);
				SetVertex(inc, gridSize, radius, heightMap, heightMultiplier, _heightCurve, normals, vertices, i++, x + xStart, y, z + zStart, z + zStart, x + xStart);
            }
        }
        Color[] colorMap = new Color[gridSize * gridSize];
        int index = 0;
        for (int z = zStart; z < zStart + gridSize; z++)
        {
            for (int x = xStart; x < xStart + gridSize; x++)
            {
                float currentHeight = heightMap[x, z];
                for (int j = 0; j < regions.Length; j++)
                {
                    if (currentHeight <= regions[j].height)
                    {
                        colorMap[index] = regions[j].color;
                        index++;
                        break;
                    }
                }
            }
        }
        mesh.vertices = vertices;
        mesh.normals = normals;
		mesh.uv = uvs;
		SegmentData segment = new SegmentData(mesh, TextureGenerator.TextureFromColorMap(colorMap, gridSize, gridSize));
        return segment;
    }

    private static SegmentData ZyPlane(int inc, int gridSize, float radius, float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, TerrainType[] regions, int yStart, int zStart, int x)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(gridSize + 1) * (gridSize + 1)];
        Vector3[] normals = new Vector3[vertices.Length];
		Vector2[] uvs = new Vector2[vertices.Length];
		int i = 0;
        for (int z = 0; z <= gridSize; z++)
        {
            for (int y = 0; y <= gridSize; y++)
            {
				uvs[i] = new Vector2(y / (float)gridSize, z / (float)gridSize);
				SetVertex(inc, gridSize, radius, heightMap, heightMultiplier, _heightCurve, normals, vertices, i++, x, y + yStart, z + zStart, z + zStart, y + yStart);
            }
        }
        Color[] colorMap = new Color[gridSize * gridSize];
        int index = 0;
        for (int z = zStart; z < zStart + gridSize; z++)
        {
            for (int y = yStart; y < yStart + gridSize; y++)
            {
                float currentHeight = heightMap[y, z];
                for (int j = 0; j < regions.Length; j++)
                {
                    if (currentHeight <= regions[j].height)
                    {
                        colorMap[index] = regions[j].color;
                        index++;
                        break;
                    }
                }
            }
        }
        mesh.vertices = vertices;
        mesh.normals = normals;
		mesh.uv = uvs;
		SegmentData segment = new SegmentData(mesh, TextureGenerator.TextureFromColorMap(colorMap, gridSize, gridSize));
        return segment;
    }

    private static void SetVertex(int inc, int gridSize, float radius, float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, Vector3[] normals, Vector3[] vertices, int i, int x, int y, int z, int xy, int xz)
    {       
        Vector3 v = new Vector3(x, y, z) * 2.0f / (gridSize*inc) - Vector3.one;
        float x2 = v.x * v.x;
        float y2 = v.y * v.y;
        float z2 = v.z * v.z;
        Vector3 s;
        s.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
        s.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
        s.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);
		normals[i] = s.normalized;       
        vertices[i] = normals[i] * (radius + _heightCurve.Evaluate(heightMap[xz, xy]) * heightMultiplier);
    }

    private static Mesh GenerateTris(int gridSize, Vector3[] vertices, int divs)
    {
        Mesh mesh = new Mesh();		
		mesh.vertices = vertices;
        int[] triangles = new int[gridSize*gridSize*6];
        int index = 0;
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                index = MakeQuad(triangles, index, (y*(gridSize+1))+x, (y* (gridSize + 1)) +x+1, (y* (gridSize + 1)) + gridSize + x+1, (y * (gridSize + 1)) + gridSize + x+2);
            }            
        }
        mesh.triangles = triangles;
		return mesh;
    }

    private static int MakeQuad(int[] triangles, int i, int v1, int v14, int v23, int v5)
    {
        triangles[i] = v1;
        triangles[i + 1] = triangles[i + 4] = v14;
        triangles[i + 2] = triangles[i + 3] = v23;
        triangles[i + 5] = v5;
        return i + 6;
    }

    //private void OnDrawGizmos()
    //{
    //    if (vertices == null)
    //    {
    //        return;
    //    }

    //    Gizmos.color = Color.black;
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        Gizmos.color = Color.black;
    //        Gizmos.DrawSphere(vertices[i], 0.1f);
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawRay(vertices[i], normals[i]);
    //    }
    //}
}