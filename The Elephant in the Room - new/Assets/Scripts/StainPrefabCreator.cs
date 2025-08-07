using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class StainPrefabCreator : MonoBehaviour
{
    [Header("Stain Settings")]
    [Tooltip("Health points for the stain")]
    public int stainHealth = 3;
    
    [Tooltip("Scale of the stain")]
    public Vector3 stainScale = Vector3.one * 0.3f;
    
    [Tooltip("Color of the stain")]
    public Color stainColor = new Color(0.6f, 0.4f, 0.2f); // Brown color
    
    [Header("Prefab Settings")]
    [Tooltip("Name for the created prefab")]
    public string prefabName = "StainPrefab";
    
    [Tooltip("Path to save the prefab")]
    public string prefabPath = "Assets/Prefabs/";

    /// <summary>
    /// Create a stain prefab with all necessary components
    /// </summary>
    [ContextMenu("Create Stain Prefab")]
    public void CreateStainPrefab()
    {
        // Create the GameObject
        GameObject stainObject = new GameObject("Stain");
        
        // Add MeshFilter and MeshRenderer
        MeshFilter meshFilter = stainObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = stainObject.AddComponent<MeshRenderer>();
        
        // Create a simple sphere mesh
        meshFilter.mesh = CreateSphereMesh();
        
        // Add Collider
        SphereCollider collider = stainObject.AddComponent<SphereCollider>();
        collider.radius = 0.5f;
        
        // Add Stain script
        Stain stain = stainObject.AddComponent<Stain>();
        stain.health = stainHealth;
        
        // Add AudioSource
        AudioSource audioSource = stainObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound
        
        // Set material
        Material material = new Material(Shader.Find("Standard"));
        material.color = stainColor;
        meshRenderer.material = material;
        
        // Set scale
        stainObject.transform.localScale = stainScale;
        
        // Set layer to Stain (Layer 11)
        stainObject.layer = 11;
        
        // Create prefab
        #if UNITY_EDITOR
        if (!AssetDatabase.IsValidFolder(prefabPath))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        string fullPath = prefabPath + prefabName + ".prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(stainObject, fullPath);
        
        if (prefab != null)
        {
            Debug.Log($"StainPrefabCreator: Created stain prefab at {fullPath}");
            
            // Select the created prefab
            Selection.activeObject = prefab;
        }
        else
        {
            Debug.LogError("StainPrefabCreator: Failed to create prefab!");
        }
        
        // Clean up the temporary GameObject
        DestroyImmediate(stainObject);
        #else
        Debug.LogWarning("StainPrefabCreator: Can only create prefabs in Editor mode");
        DestroyImmediate(stainObject);
        #endif
    }

    /// <summary>
    /// Create a simple sphere mesh for the stain
    /// </summary>
    private Mesh CreateSphereMesh()
    {
        // Create a simple sphere using Unity's built-in sphere
        GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Mesh sphereMesh = tempSphere.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(tempSphere);
        
        return sphereMesh;
    }

    /// <summary>
    /// Create a stain layer if it doesn't exist
    /// </summary>
    [ContextMenu("Create Stain Layer")]
    public void CreateStainLayer()
    {
        #if UNITY_EDITOR
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");
        
        // Check if Stain layer already exists
        bool layerExists = false;
        for (int i = 8; i < layers.arraySize; i++)
        {
            SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
            if (layerSP.stringValue == "Stain")
            {
                layerExists = true;
                break;
            }
        }
        
        if (!layerExists)
        {
            // Find first empty layer
            for (int i = 8; i < layers.arraySize; i++)
            {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
                if (string.IsNullOrEmpty(layerSP.stringValue))
                {
                    layerSP.stringValue = "Stain";
                    tagManager.ApplyModifiedProperties();
                    Debug.Log($"StainPrefabCreator: Created Stain layer at index {i}");
                    break;
                }
            }
        }
        else
        {
            Debug.Log("StainPrefabCreator: Stain layer already exists");
        }
        #else
        Debug.LogWarning("StainPrefabCreator: Can only create layers in Editor mode");
        #endif
    }

    /// <summary>
    /// Create both layer and prefab
    /// </summary>
    [ContextMenu("Setup Complete Stain System")]
    public void SetupCompleteStainSystem()
    {
        CreateStainLayer();
        CreateStainPrefab();
    }
} 