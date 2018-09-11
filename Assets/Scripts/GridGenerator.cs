using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public string initialName = "ProcGrid";

    public Transform parent;

    public Material initialMaterial;

    public TransformDisplay initialTransform;

    public GridOptions gridOptions = new GridOptions();

    [System.Serializable]
    public class GridOptions
    {
        public IntVector2 gridSize = new IntVector2(10, 10);
        public Vector2 gridUnitSize = Vector2.one;
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

    [System.Serializable]
    public class TransformDisplay
    {
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public Vector3 scale = Vector3.one;
    }

    public void GenerateGrid()
    {
        GameObject go = new GameObject(initialName, typeof(MeshFilter), typeof(MeshRenderer), typeof(ProcGrid));
        go.GetComponent<ProcGrid>().Init(gridOptions);
        go.transform.position = initialTransform.position;
        go.transform.rotation = Quaternion.Euler(initialTransform.rotation);
        go.transform.localScale = initialTransform.scale;
        if (parent != null)
        {
            go.transform.SetParent(parent);
        }
        go.GetComponent<MeshRenderer>().material = initialMaterial;

        go.GetComponent<MeshFilter>().sharedMesh = GenerateMesh(gridOptions);
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
                vertices[index] = new Vector3(j * gridSizeX, 0, i * gridSizeY);
                uvs[index] = new Vector2((float)j / rows, (float)i / cols);
                tangents[index] = staticTangent;
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.tangents = tangents;
        return mesh; ;
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
