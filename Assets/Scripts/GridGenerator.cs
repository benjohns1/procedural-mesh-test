using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public string initialName = "ProcGrid";

    public Transform parent;

    public Material initialMaterial;

    public Vector3 initialOffset = new Vector3(-1000, -250, -1000);

    public IntVector2 worldSize = new IntVector2(20, 20);

    public GridOptions gridOptions = new GridOptions(100, 100);

    [System.Serializable]
    public struct GridOptions
    {
        public IntVector2 gridSize;
        public Vector2 gridUnitSize;
        public Vector2 materialScale;
        public Vector2 procOffset;
        public ProcLevel[] procLevels;
        public GridOptions(int xSize, int ySize)
        {
            gridSize = new IntVector2(xSize, ySize);
            gridUnitSize = Vector2.one;
            materialScale = new Vector2(10000f, 10000f);
            procOffset = Vector2.zero;
            procLevels = new ProcLevel[]
            {
                new ProcLevel
                {
                    perlinHeight = 1f,
                    perlinScale = new Vector2(0.1f, 0.1f)
                },
                new ProcLevel
                {
                    perlinHeight = 10f,
                    perlinScale = new Vector2(0.05f, 0.05f)
                },
                new ProcLevel
                {
                    perlinHeight = 50f,
                    perlinScale = new Vector2(0.01f, 0.01f)
                },
                new ProcLevel
                {
                    perlinHeight = 100f,
                    perlinScale = new Vector2(0.005f, 0.005f)
                },
                new ProcLevel
                {
                    perlinHeight = 500f,
                    perlinScale = new Vector2(0.001f, 0.001f)
                }
            };
        }

        [System.Serializable]
        public struct ProcLevel
        {
            public float perlinHeight;
            public Vector2 perlinScale;
        }
    }

    [System.Serializable]
    public class IntVector2
    {
        public int x;
        public int y;
        public IntVector2(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
        public static IntVector2 zero = new IntVector2(0, 0);
        public static IntVector2 one = new IntVector2(1, 1);
    }

    public void GenerateWorld()
    {
        for (int x = 0; x < worldSize.x; x++)
        {
            for (int y = 0; y < worldSize.y; y++)
            {
                string gridName = initialName + "(" + x + "," + y + ")";
                GridOptions opts = gridOptions;
                float offsetX = x * opts.gridUnitSize.x * opts.gridSize.x;
                float offsetY = y * opts.gridUnitSize.y * opts.gridSize.y;
                opts.procOffset.x += offsetX;
                opts.procOffset.y += offsetY;
                Vector3 offset = initialOffset;
                offset.x += offsetX;
                offset.z += offsetY;
                GenerateSingleGrid(gridName, opts, offset);
            }
        }
    }

    public void GenerateSingleGrid()
    {
        GenerateSingleGrid(initialName, gridOptions, initialOffset);
    }

    private void GenerateSingleGrid(string gridName, GridOptions opts, Vector3 offset)
    {
        GameObject go = new GameObject(gridName, typeof(MeshFilter), typeof(MeshRenderer), typeof(ProcGrid), typeof(MeshCollider));
        go.GetComponent<ProcGrid>().Init(opts);
        go.transform.position = offset;
        if (parent != null)
        {
            go.transform.SetParent(parent);
        }
        go.GetComponent<MeshRenderer>().material = initialMaterial;
        if (initialMaterial.mainTexture.wrapMode == TextureWrapMode.Repeat)
        {
            float uScale = Mathf.Ceil(opts.materialScale.x * opts.gridUnitSize.x / initialMaterial.mainTexture.width);
            float vScale = Mathf.Ceil(opts.materialScale.y * opts.gridUnitSize.y / initialMaterial.mainTexture.height);
            initialMaterial.mainTextureScale = new Vector2(uScale, vScale);
        }

        Mesh mesh = GenerateMesh(opts);
        go.GetComponent<MeshFilter>().sharedMesh = mesh;
        go.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public void DestroyAll()
    {
        List<GameObject> gos = GameObject.FindObjectsOfType<ProcGrid>().Aggregate(new List<GameObject>(), (acc, q) =>
        {
            if (!q.persist)
            {
                acc.Add(q.gameObject);
            }
            return acc;
        });
        if (Application.isPlaying)
        {
            gos.ForEach(go => Object.Destroy(go));
        }
        else
        {
            gos.ForEach(go => Object.DestroyImmediate(go));
        }
    }

    public static Mesh GenerateMesh(GridOptions opts, Mesh previousMesh = null)
    {
        Mesh mesh = (previousMesh == null) ? new Mesh() : previousMesh;
        mesh = GenerateMeshData(opts, mesh);
        mesh.triangles = GenerateTriangles(opts);
        mesh.RecalculateNormals();
        return mesh;
    }

    private static Mesh GenerateMeshData(GridOptions opts, Mesh mesh)
    {
        int cols = opts.gridSize.x;
        int rows = opts.gridSize.y;
        float gridSizeX = opts.gridUnitSize.x;
        float gridSizeY = opts.gridUnitSize.y;
        Vector3[] vertices = new Vector3[(cols + 1) * (rows + 1)];
        Vector2[] uvs = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 staticTangent = new Vector4(1f, 0f, 0f, -1f);

        for (int i = 0, index = 0; i <= cols; i++)
        {
            for (int j = 0; j <= rows; j++, index++)
            {
                float x = j * gridSizeX;
                float y = i * gridSizeY;
                vertices[index] = new Vector3(x, 0, y);
                uvs[index] = new Vector2((float)j / rows, (float)i / cols);
                tangents[index] = staticTangent;
            }
        }
        mesh.vertices = opts.procLevels.Length > 0 ? ApplyPerlinHeights(opts, vertices) : vertices;
        mesh.uv = uvs;
        mesh.tangents = tangents;
        return mesh;
    }

    private static Vector3[] ApplyPerlinHeights(GridOptions opts, Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            float x = vertices[i].x + opts.procOffset.x;
            float y = vertices[i].z + opts.procOffset.y;
            float height = vertices[i].y;

            foreach (GridOptions.ProcLevel level in opts.procLevels)
            {
                height += level.perlinHeight * Mathf.PerlinNoise(x * level.perlinScale.x, y * level.perlinScale.y);
            }

            vertices[i].y = height;
        }
        return vertices;
    }

    private static int[] GenerateTriangles(GridOptions opts)
    {
        int cols = opts.gridSize.x;
        int rows = opts.gridSize.y;
        int[] triangles = new int[cols * rows * 6];
        for (int ti = 0, vi = 0, y = 0; y < rows; y++, vi++)
        {
            for (int x = 0; x < cols; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + cols + 1;
                triangles[ti + 5] = vi + cols + 2;
            }
        }
        return triangles;
    }
}
