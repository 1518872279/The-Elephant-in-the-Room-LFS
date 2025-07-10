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
        EditorGUILayout.LabelField("Manual Controls", EditorStyles.boldLabel);
        
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
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Toggle Storm", GUILayout.Height(30)))
        {
            stormController.ToggleStorm();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Current Status", EditorStyles.boldLabel);
        
        bool isActive = stormController.IsStormActive;
        string statusText = isActive ? "Storm is ACTIVE" : "Storm is INACTIVE";
        Color statusColor = isActive ? Color.green : Color.red;
        
        GUI.color = statusColor;
        EditorGUILayout.LabelField(statusText, EditorStyles.boldLabel);
        GUI.color = Color.white;
    }
} 