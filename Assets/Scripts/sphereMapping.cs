﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class sphereMapping : MonoBehaviour {

    public static Mesh getSphere(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve)
    {
    int gridSize = 50;
    float radius = 5;

    return Generate(gridSize, radius, heightMap, heightMultiplier, _heightCurve);
    }

    public static Mesh Generate(int gridSize, float radius, float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve)
    {
        Mesh mesh = new Mesh();

        Mesh vertMesh = GenerateVerts(gridSize, radius, heightMap, heightMultiplier, _heightCurve);
        Mesh triMesh = GenerateTris(gridSize, vertMesh.vertices);

        mesh.vertices = vertMesh.vertices;
        mesh.normals = vertMesh.normals;
        mesh.subMeshCount = 3;
        mesh.SetTriangles(triMesh.GetTriangles(0), 0);
        mesh.SetTriangles(triMesh.GetTriangles(1), 1);
        mesh.SetTriangles(triMesh.GetTriangles(2), 2);
        return mesh;
    }

    private static Mesh GenerateVerts(int gridSize, float radius, float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve)
    {
        int cornerVerts = 8;
        int edgeVerts = (gridSize + gridSize + gridSize - 3) * 4;
        int faceVerts = (
            (gridSize - 1) * (gridSize - 1) +
            (gridSize - 1) * (gridSize - 1) +
            (gridSize - 1) * (gridSize - 1)) * 2;
        Vector3[] vertices = new Vector3[cornerVerts + edgeVerts + faceVerts];
        Vector3[] normals = new Vector3[vertices.Length];
        int i = 0;
        // rings 
        for (int y = 0; y <= gridSize; y++)
        {
            for (int x = 0; x <= gridSize; x++)
            {
                SetVertex(gridSize, radius, heightMap, heightMultiplier, _heightCurve, normals, vertices, i++, x, y, 0);
            }
            for (int z = 1; z <= gridSize; z++)
            {
                SetVertex(gridSize, radius, heightMap, heightMultiplier, _heightCurve, normals, vertices, i++, gridSize, y, z);
            }
            for (int x = gridSize - 1; x >= 0; x--)
            {
                SetVertex(gridSize, radius, heightMap, heightMultiplier, _heightCurve, normals, vertices, i++, x, y, gridSize);
            }
            for (int z = gridSize - 1; z > 0; z--)
            {
                SetVertex(gridSize, radius, heightMap, heightMultiplier, _heightCurve, normals, vertices, i++, 0, y, z);
            }
        }
        // holes
        for (int z = 1; z < gridSize; z++)
        {
            for (int x = 1; x < gridSize; x++)
            {
                SetVertex(gridSize, radius, heightMap, heightMultiplier, _heightCurve, normals, vertices, i++, x, gridSize, z);
            }
        }
        for (int z = 1; z < gridSize; z++)
        {
            for (int x = 1; x < gridSize; x++)
            {
                SetVertex(gridSize, radius, heightMap, heightMultiplier, _heightCurve, normals, vertices, i++, x, 0, z);
            }
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.normals = normals;
        return mesh;
    }

    private static void SetVertex(int gridSize, float radius, float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, Vector3[] normals, Vector3[] vertices, int i, int x, int y, int z)
    {
        Vector3 v = new Vector3(x, y, z) * 2.0f / gridSize - Vector3.one;
        float x2 = v.x * v.x;
        float y2 = v.y * v.y;
        float z2 = v.z * v.z;
        Vector3 s;
        s.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
        s.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
        s.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);
        normals[i] = s.normalized;
        vertices[i] = normals[i] * (radius + _heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier);
    }

    private static Mesh GenerateTris(int gridSize, Vector3[] vertices)
    {
        int[] trianglesZ = new int[(gridSize * gridSize) * 12];
        int[] trianglesX = new int[(gridSize * gridSize) * 12];
        int[] trianglesY = new int[(gridSize * gridSize) * 12];
        int ring = (gridSize + gridSize) * 2;
        int tZ = 0, tX = 0, tY = 0, v = 0;

        for (int y = 0; y < gridSize; y++, v++)
        {
            for (int q = 0; q < gridSize; q++, v++)
            {
                tZ = MakeQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < gridSize; q++, v++)
            {
                tX = MakeQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < gridSize; q++, v++)
            {
                tZ = MakeQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < gridSize - 1; q++, v++)
            {
                tX = MakeQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            tX = MakeQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);
        }

        tY = CreateBoxTop(gridSize, trianglesY, tY, ring);        
        tY = CreateBoxBot(gridSize, vertices, trianglesY, tY, ring);
        
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.subMeshCount = 3;
        mesh.SetTriangles(trianglesZ, 0);
        mesh.SetTriangles(trianglesX, 1);
        mesh.SetTriangles(trianglesY, 2);
        return mesh;
    }

    private static int CreateBoxTop(int gridSize, int[] triangles, int t, int ring)
    {
        int v = ring * gridSize;
        for (int x = 0; x < gridSize - 1; x++, v++)
        {
            t = MakeQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
        }
        t = MakeQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

        int vMin = ring * (gridSize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for (int z = 1; z < gridSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = MakeQuad(triangles, t, vMin, vMid, vMin - 1, vMid + gridSize - 1);
            for (int x = 1; x < gridSize - 1; x++, vMid++)
            {
                t = MakeQuad(
                    triangles, t,
                    vMid, vMid + 1, vMid + gridSize - 1, vMid + gridSize);
            }
            t = MakeQuad(triangles, t, vMid, vMax, vMid + gridSize - 1, vMax + 1);
        }

        int vTop = vMin - 2;
        t = MakeQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
        for (int x = 1; x < gridSize - 1; x++, vTop--, vMid++)
        {
            t = MakeQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
        }
        t = MakeQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

        return t;
    }

    private static int CreateBoxBot(int gridSize, Vector3[] vertices, int[] triangles, int t, int ring)
    {
        int v = 1;
        int vMid = vertices.Length - (gridSize - 1) * (gridSize - 1);
        t = MakeQuad(triangles, t, ring - 1, vMid, 0, 1);
        for (int x = 1; x < gridSize - 1; x++, v++, vMid++)
        {
            t = MakeQuad(triangles, t, vMid, vMid + 1, v, v + 1);
        }
        t = MakeQuad(triangles, t, vMid, v + 2, v, v + 1);
                
        int vMin = ring - 2;
        vMid -= gridSize - 2;
        int vMax = v + 2;

        for (int z = 1; z < gridSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = MakeQuad(triangles, t, vMin, vMid + gridSize - 1, vMin + 1, vMid);
            for (int x = 1; x < gridSize - 1; x++, vMid++)
            {
                t = MakeQuad(
                    triangles, t,
                    vMid + gridSize - 1, vMid + gridSize, vMid, vMid + 1);
            }
            t = MakeQuad(triangles, t, vMid + gridSize - 1, vMax + 1, vMid, vMax);
        }

        int vTop = vMin - 1;
        t = MakeQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
        for (int x = 1; x < gridSize - 1; x++, vTop--, vMid++)
        {
            t = MakeQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
        }
        t = MakeQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);

        return t;
    }

    private static int MakeQuad(int[] triangles, int i, int v1, int v23, int v14, int v5)
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