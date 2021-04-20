using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MarchingCubes))]
public class MarchingCubesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MarchingCubes marchingCubes = (MarchingCubes) target;
        if (GUILayout.Button("Generate terrain")) marchingCubes.GenerateTerrain();
        if (GUILayout.Button("Delete terrain")) marchingCubes.Reset();
    }
}
