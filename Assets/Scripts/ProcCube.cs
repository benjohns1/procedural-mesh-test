using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProcCube : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;

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

    private void Awake()
    {
        Init(options);
    }

    public void Init(Options opts)
    {
        StartCoroutine(Generate(opts));
    }

    private IEnumerator Generate(Options opts)
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = opts.name;

        int xSize = opts.size.x;
        int ySize = opts.size.y;
        int zSize = opts.size.z;

        int cornerVertices = 8;
        int edgeVertices = (xSize + ySize + zSize - 3) * 4;
        int faceVertices = (
            (xSize - 1) * (ySize - 1) +
            (xSize - 1) * (zSize - 1) +
            (ySize - 1) * (zSize - 1)) * 2;
        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];

        int v = 0;
        for (int y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[v++] = new Vector3(x, 0, 0);
            }
            for (int z = 1; z <= zSize; z++)
            {
                vertices[v++] = new Vector3(xSize, 0, z);
            }
            for (int x = xSize - 1; x >= 0; x--)
            {
                vertices[v++] = new Vector3(x, 0, zSize);
            }
            for (int z = zSize - 1; z > 0; z--)
            {
                vertices[v++] = new Vector3(0, 0, z);
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

        yield return null;
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }

        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Debug.Log(vertices[i]);
            Gizmos.DrawSphere(vertices[i], 1f);
        }
    }
}
