using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StormController))]
public class StormControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        StormController stormController = (StormController)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Manual Control", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Activate Storm", GUILayout.Height(30)))
        {
            stormController.ActivateStorm();
        }
        
        if (GUILayout.Button("Deactivate Storm", GUILayout.Height(30)))
        {
            stormController.DeactivateStorm();
        }
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Use these buttons to manually test the storm system in Play mode.", MessageType.Info);
    }
} 