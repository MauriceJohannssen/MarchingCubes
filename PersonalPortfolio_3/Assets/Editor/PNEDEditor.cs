using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PNED))]
public class PNEDEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PNED pned = (PNED) target;
        if (GUILayout.Button("Create environment")) pned.GenerateEnvironment();
        if (GUILayout.Button("Delete environment")) pned.DeleteEnvironment();
    }
}
