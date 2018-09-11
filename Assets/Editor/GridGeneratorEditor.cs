using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridGenerator))]
public class GridGeneratorEditor : Editor
{
    private bool ShowAdvanced = false;

    public override void OnInspectorGUI()
    {
        GridGenerator gridGenerator = target as GridGenerator;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate Single Grid"))
        {
            gridGenerator.GenerateSingleGrid();
        }
        if (GUILayout.Button("Generate World Grid"))
        {
            gridGenerator.GenerateWorld();
        }
        ShowAdvanced = EditorGUILayout.Foldout(ShowAdvanced, "Advanced");
        if (ShowAdvanced)
        {
            if (GUILayout.Button("Destroy All"))
            {
                DestroyConfirm(gridGenerator);
            }
        }
    }

    private static void DestroyConfirm(GridGenerator gridGenerator)
    {
        if (EditorUtility.DisplayDialog("Destroy all grid game objects?", "Destroy all quad game objects?\nThis action cannot be undone!", "Destroy All", "Cancel"))
        {
            gridGenerator.DestroyAll();
        }
    }
}
