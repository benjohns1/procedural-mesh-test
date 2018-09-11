using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuadGenerator))]
public class QuadGeneratorEditor : Editor
{
    private bool ShowAdvanced = false;

    public override void OnInspectorGUI()
    {
        QuadGenerator quadGenerator = target as QuadGenerator;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate Quad"))
        {
            quadGenerator.GenerateQuad();
        }
        ShowAdvanced = EditorGUILayout.Foldout(ShowAdvanced, "Advanced");
        if (ShowAdvanced)
        {
            if (GUILayout.Button("Destroy All"))
            {
                DestroyConfirm(quadGenerator);
            }
        }
    }

    private static void DestroyConfirm(QuadGenerator quadGenerator)
    {
        if (EditorUtility.DisplayDialog("Destroy all quad game objects?", "Destroy all quad game objects?\nThis action cannot be undone!", "Destroy All", "Cancel"))
        {
            quadGenerator.DestroyAll();
        }
    }
}
