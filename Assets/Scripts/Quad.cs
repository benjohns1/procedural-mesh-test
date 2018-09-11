using UnityEngine;

public class Quad : MonoBehaviour
{
    public Vector2 size;

    public bool persist;

    private void OnValidate()
    {
        UpdateQuad();
    }

    private void UpdateQuad()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        if (mesh == null)
        {
            return;
        }
        mesh.vertices = QuadGenerator.GenerateVertices(size);
    }
}
