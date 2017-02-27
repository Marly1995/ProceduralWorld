using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
public class sphereMapping : MonoBehaviour
{
    public int xSize, ySize, zSize;
    private Vector3[] vertices;
    private Mesh mesh;

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {     
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "ProceduralMesh";
        GenerateVerts();
        GenerateTris();
    }

    private void GenerateVerts()
    {
        int cornerVerts = 8;
        int edgeVerts = (xSize + ySize + zSize - 3) * 4;
        int faceVerts = (
            (xSize - 1) * (ySize - 1) +
            (xSize - 1) * (zSize - 1) +
            (ySize - 1) * (zSize - 1)) * 2;
        vertices = new Vector3[cornerVerts + edgeVerts + faceVerts];
        int i = 0;
        // rings 
        for (int y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[i++] = new Vector3(x, y, 0);
            }
            for (int z = 1; z <= zSize; z++)
            {
                vertices[i++] = new Vector3(xSize, y, z);
            }
            for (int x = xSize - 1; x >= 0; x--)
            {
                vertices[i++] = new Vector3(x, y, zSize);
            }
            for (int z = zSize - 1; z > 0; z--)
            {
                vertices[i++] = new Vector3(0, y, z);
            }
        }
        // holes
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
            {
                vertices[i++] = new Vector3(x, ySize, z);
            }
        }
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
            {
                vertices[i++] = new Vector3(x, 0, z);
            }
        }
        mesh.vertices = vertices;
    }

    private void GenerateTris()
    {

    }

    private static void MakeQuad(int i, int v1, int v14, int v23, int v5)
    {
        triangles[i] = v1;
        triangles[i + 1] = triangles[i + 4] = v14;
        triangles[i + 2] = triangles[i + 3] = v23;
        triangles[i + 5] = v5;
    }

    private void OnDrawGizmos()
    {
        if(vertices == null)
        {
            return;
        }

        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}
