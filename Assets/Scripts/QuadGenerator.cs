using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadGenerator : MonoBehaviour
{
    public string initialName = "Quad";

    public Transform parent;

    public Material initialMaterial;

    public Vector2 initialSize = new Vector2(1, 1);

    public TransformDisplay initialTransform;

    [System.Serializable]
    public class TransformDisplay
    {
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public Vector3 scale = Vector3.one;
    }

    public void GenerateQuad()
    {
        GameObject quadGO = new GameObject(initialName, typeof(MeshFilter), typeof(MeshRenderer), typeof(Quad));
        quadGO.transform.position = initialTransform.position;
        quadGO.transform.rotation = Quaternion.Euler(initialTransform.rotation);
        quadGO.transform.localScale = initialTransform.scale;
        if (parent != null)
        {
            quadGO.transform.SetParent(parent);
        }

        Quad quad = quadGO.GetComponent<Quad>();
        quad.size = initialSize;
        quadGO.GetComponent<MeshRenderer>().material = initialMaterial;
        quadGO.GetComponent<MeshFilter>().sharedMesh = new Mesh
        {
            vertices = GenerateVertices(quad.size),
            triangles = new int[]
            {
                0, 1, 2,
                2, 3, 0,
            },
            uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
            },
            normals = new Vector3[]
            {
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
            }
        };
    }

    public void DestroyAll()
    {
        List<GameObject> quadGOs = GameObject.FindObjectsOfType<Quad>().Aggregate(new List<GameObject>(), (acc, q) =>
        {
            if (!q.persist)
            {
                acc.Add(q.gameObject);
            }
            return acc;
        });
        if (Application.isPlaying)
        {
            quadGOs.ForEach(go => Object.Destroy(go));
        }
        else
        {
            quadGOs.ForEach(go => Object.DestroyImmediate(go));
        }
    }

    public static Vector3[] GenerateVertices(Vector2 size)
    {
        return new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, size.y),
            new Vector3(size.x, 0, size.y),
            new Vector3(size.x, 0, 0),
        };
    }
}
