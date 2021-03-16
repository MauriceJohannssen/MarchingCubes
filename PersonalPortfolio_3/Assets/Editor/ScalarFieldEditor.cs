using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScalarField))]
public class ScalarFieldEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ScalarField scalarField = (ScalarField) target;
        if(GUILayout.Button("Bake scalar field")) scalarField.RebakeScalarField();
    }
}
