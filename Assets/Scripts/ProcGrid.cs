using UnityEngine;

public class ProcGrid : MonoBehaviour
{
    public bool persist;

    public GridGenerator.GridOptions gridOptions;

    public void Init(GridGenerator.GridOptions _gridOptions)
    {
        gridOptions = _gridOptions;
    }

    private void OnValidate()
    {
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        if (mesh == null)
        {
            return;
        }
        mesh = GridGenerator.GenerateMesh(gridOptions, mesh);
    }
}
