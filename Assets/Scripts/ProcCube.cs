﻿using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProcCube : MonoBehaviour
{
    public Options options;

    [Serializable]
    public class Options
    {
        public string name = "ProcCube";
        public IntVector3 size = new IntVector3(8, 8, 8);
    }

    [Serializable]
    public class IntVector3
    {
        public int x;
        public int y;
        public int z;
        public IntVector3(int _x, int _y, int _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
    }

    private void Start()
    {
        Generate(options);
    }

    private void OnValidate()
    {
        Generate(options);
    }

    public void Generate(Options opts)
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh = new Mesh();
        mesh.name = opts.name;
        mesh.vertices = CreateVertices(opts);
        int vCount = mesh.vertices.Length;
        mesh.triangles = CreateTriangles(opts, vCount);
    }

    private static int[] CreateTriangles(Options opts, int vCount)
    {
        int xSize = opts.size.x;
        int ySize = opts.size.y;
        int zSize = opts.size.z;
        int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
        int[] triangles = new int[quads * 6];

        int ring = (xSize + zSize) * 2;
        int t = 0, v = 0;

        for (int y = 0; y < ySize; y++, v++)
        {
            for (int q = 0; q < ring - 1; q++, v++)
            {
                t = SetQuad(ref triangles, t, v, v + 1, v + ring, v + ring + 1);
            }
            t = SetQuad(ref triangles, t, v, v - ring + 1, v + ring, v + 1);
        }
        t = CreateTopFace(ref triangles, t, ring, xSize, ySize, zSize);
        t = CreateBottomFace(ref triangles, t, ring, xSize, ySize, zSize, vCount);
        

        return triangles;
    }

    private static int CreateTopFace(ref int[] triangles, int t, int ring, int xSize, int ySize, int zSize)
    {
        // First row
        int v = ring * ySize;
        for (int x = 0; x < xSize - 1; x++, v++)
        {
            t = SetQuad(ref triangles, t, v, v + 1, v + ring - 1, v + ring);
        }
        t = SetQuad(ref triangles, t, v, v + 1, v + ring - 1, v + 2);

        // Middle rows
        int vMin = ring * (ySize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;
        for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(ref triangles, t, vMin, vMid, vMin - 1, vMid + xSize - 1);
            for (int x = 1; x < xSize - 1; x++, vMid++)
            {
                t = SetQuad(ref triangles, t, vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
            }
            t = SetQuad(ref triangles, t, vMid, vMax, vMid + xSize - 1, vMax + 1);
        }

        // Last row
        int vTop = vMin - 2;
        t = SetQuad(ref triangles, t, vMin, vMid, vMin - 1, vMin - 2);
        for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(ref triangles, t, vMid, vMid + 1, vTop, vTop - 1);
        }
        t = SetQuad(ref triangles, t, vMid, vTop - 2, vTop, vTop - 1);

        return t;
    }
    private static int CreateBottomFace(ref int[] triangles, int t, int ring, int xSize, int ySize, int zSize, int vCount)
    {
        // First row
        int v = 1;
        int vMid = vCount - (xSize - 1) * (zSize - 1);
        t = SetQuad(ref triangles, t, ring - 1, vMid, 0, 1);
        for (int x = 1; x < xSize - 1; x++, v++, vMid++)
        {
            t = SetQuad(ref triangles, t, vMid, vMid + 1, v, v + 1);
        }
        t = SetQuad(ref triangles, t, vMid, v + 2, v, v + 1);

        // Middle rows
        int vMin = ring - 2;
        vMid -= xSize - 2;
        int vMax = v + 2;

        for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(ref triangles, t, vMin, vMid + xSize - 1, vMin + 1, vMid);
            for (int x = 1; x < xSize - 1; x++, vMid++)
            {
                t = SetQuad(ref triangles, t, vMid + xSize - 1, vMid + xSize, vMid, vMid + 1);
            }
            t = SetQuad(ref triangles, t, vMid + xSize - 1, vMax + 1, vMid, vMax);
        }

        // Last row
        int vTop = vMin - 1;
        t = SetQuad(ref triangles, t, vTop + 1, vTop, vTop + 2, vMid);
        for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(ref triangles, t, vTop, vTop - 1, vMid, vMid + 1);
        }
        t = SetQuad(ref triangles, t, vTop, vTop - 1, vMid, vTop - 2);

        return t;
    }

    private static int SetQuad(ref int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;
        return i + 6;
    }


    private static Vector3[] CreateVertices(Options opts)
    {
        int xSize = opts.size.x;
        int ySize = opts.size.y;
        int zSize = opts.size.z;

        int cornerVertices = 8;
        int edgeVertices = (xSize + ySize + zSize - 3) * 4;
        int faceVertices = (
            (xSize - 1) * (ySize - 1) +
            (xSize - 1) * (zSize - 1) +
            (ySize - 1) * (zSize - 1)) * 2;
        Vector3[] vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];

        int v = 0;
        for (int y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[v++] = new Vector3(x, y, 0);
            }
            for (int z = 1; z <= zSize; z++)
            {
                vertices[v++] = new Vector3(xSize, y, z);
            }
            for (int x = xSize - 1; x >= 0; x--)
            {
                vertices[v++] = new Vector3(x, y, zSize);
            }
            for (int z = zSize - 1; z > 0; z--)
            {
                vertices[v++] = new Vector3(0, y, z);
            }
        }
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
            {
                vertices[v++] = new Vector3(x, ySize, z);
            }
        }
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
            {
                vertices[v++] = new Vector3(x, 0, z);
            }
        }

        return vertices;
    }
}
