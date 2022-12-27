using UnityEngine;
using UnityEditor;
using UnityQuickSheet;

[CustomEditor(typeof(ButtonMapping))]
public class InputMappingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Update"))
        {
            ButtonMapping t = (ButtonMapping)target;
            t.Update();
        }
        base.OnInspectorGUI();
    }
}