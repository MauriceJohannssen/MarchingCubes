using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PerlinNoise))]
public class PerlinNoiseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PerlinNoise perlinNoise = (PerlinNoise) target;
        if(GUILayout.Button("Visualize scalar field")) perlinNoise.RebakeScalarField();
    }
}
