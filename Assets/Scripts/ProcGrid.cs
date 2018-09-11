using UnityEngine;

public class ProcGrid : MonoBehaviour
{
    public bool persist;

    public GridGenerator.GridOptions gridOptions;

    public void Init(GridGenerator.GridOptions _gridOptions)
    {
        gridOptions = _gridOptions;
    }
}
