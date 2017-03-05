using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class sphereMapping : MonoBehaviour {

    public static Mesh[] getSphere(int divs, int gridSize, MapData[] heightMap, float heightMultiplier, AnimationCurve _heightCurve)
    {
    float radius = 5;

    return Generate(divs, gridSize, radius, heightMap, heightMultiplier, _heightCurve);
    }

    public static Mesh[] Generate(int divs, int gridSize, float radius, MapData[] heightMap, float heightMultiplier, AnimationCurve _heightCurve)
    {
        Mesh[] meshes = GenerateVerts(divs, gridSize, radius, heightMap, heightMultiplier, _heightCurve);

        for (int i = 0; i < meshes.Length; i++)
        {
            Mesh temp = new Mesh();
            temp = GenerateTris(gridSize, meshes[i].vertices, divs);
            meshes[i] = temp;
            meshes[i].RecalculateNormals();
        }
        for (int i = 0; i < meshes.Length; i++)
        {
            if (i >= divs * 1 && i < divs * 3)
            {
                meshes[i].triangles = meshes[i].triangles.Reverse().ToArray();
            }
            if (i >= divs * 5 && i < divs * 6)
            {
                meshes[i].triangles = meshes[i].triangles.Reverse().ToArray();
            }
        }

        return meshes;
    }

    private static Mesh[] GenerateVerts(int divs, int gridSize, float radius, MapData[] heightMap, float heightMultiplier, AnimationCurve _heightCurve)
    {
        Mesh[] meshes = new Mesh[divs*6];
        int inc = 0;
        switch (divs)
        {
            case 1:
                inc = 1;
                break;
            case 4:
                inc = 2;
                break;
            case 16:
                inc = 4;
                break;
        }
        int index = 0;
        // front
        for (int x = 0; x < inc; x++)
        {
            for (int y = 0; y < inc; y++)
            {
                meshes[index] = XyPlane(inc, gridSize, radius, heightMap[0].heightMap, heightMultiplier, _heightCurve, x * gridSize, y * gridSize, gridSize * (int)Mathf.Pow(divs, 0.5f));
                index++;                    
            }               
        }
        // right
        for (int x = 0; x < inc; x++)
        {
            for (int y = 0; y < inc; y++)
            {
                meshes[index] = ZyPlane(inc, gridSize, radius, heightMap[1].heightMap, heightMultiplier, _heightCurve, x * gridSize, y * gridSize, 0);
                index++;
            }
        }
        // back
        for (int x = 0; x < inc; x++)
        {
            for (int y = 0; y < inc; y++)
            {
                meshes[index] = XyPlane(inc, gridSize, radius, heightMap[2].heightMap, heightMultiplier, _heightCurve, x * gridSize, y * gridSize, 0);
                index++;
            }
        }
        // left
        for (int x = 0; x < inc; x++)
        {
            for (int y = 0; y < inc; y++)
            {
                meshes[index] = ZyPlane(inc, gridSize, radius, heightMap[3].heightMap, heightMultiplier, _heightCurve, x * gridSize, y * gridSize, gridSize * (int)Mathf.Pow(divs, 0.5f));
                index++;
            }
        }
        // bot
        for (int x = 0; x < inc; x++)
        {
            for (int y = 0; y < inc; y++)
            {
                meshes[index] = XzPlane(inc, gridSize, radius, heightMap[4].heightMap, heightMultiplier, _heightCurve, x * gridSize, y * gridSize, 0);
                index++;
            }
        }
        //top
        for (int x = 0; x < inc; x++)
        {
            for (int y = 0; y < inc; y++)
            {
                meshes[index] = XzPlane(inc, gridSize, radius, heightMap[5].heightMap, heightMultiplier, _heightCurve, x * gridSize, y * gridSize, gridSize * (int)Mathf.Pow(divs, 0.5f));
                index++;
            }
        }
        return meshes;
    }

    private static Mesh XyPlane(int inc, int gridSize, float radius, float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int yStart, int xStart, int z)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(gridSize+1)*(gridSize+1)];
        Vector3[] normals = new Vector3[vertices.Length];
        int i = 0;
        for (int y = 0; y <= gridSize; y++)
        {
            for (int x = 0; x <= gridSize; x++)
            {
                SetVertex(inc, gridSize, radius, heightMap, heightMultiplier, _heightCurve, normals, vertices, i++, x + xStart, y + yStart, z, y + yStart, x + xStart);
            }
        }        
        mesh.vertices = vertices;
        mesh.normals = normals;
        return mesh;
    }

    private static Mesh XzPlane(int inc, int gridSize, float radius, float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int zStart, int xStart, int y)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(gridSize + 1) * (gridSize + 1)];
        Vector3[] normals = new Vector3[vertices.Length];
        int i = 0;
        for (int z = 0; z <= gridSize; z++)
        {
            for (int x = 0; x <= gridSize; x++)
            {
                SetVertex(inc, gridSize, radius, heightMap, heightMultiplier, _heightCurve, normals, vertices, i++, x + xStart, y, z + zStart, z + zStart, x + xStart);
            }
        }
        mesh.vertices = vertices;
        mesh.normals = normals;
        return mesh;
    }

    private static Mesh ZyPlane(int inc, int gridSize, float radius, float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int yStart, int zStart, int x)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(gridSize + 1) * (gridSize + 1)];
        Vector3[] normals = new Vector3[vertices.Length];
        int i = 0;
        for (int z = 0; z <= gridSize; z++)
        {
            for (int y = 0; y <= gridSize; y++)
            {
                SetVertex(inc, gridSize, radius, heightMap, heightMultiplier, _heightCurve, normals, vertices, i++, x, y + yStart, z + zStart, z + zStart, y + yStart);
            }
        }
        mesh.vertices = vertices;
        mesh.normals = normals;
        return mesh;
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