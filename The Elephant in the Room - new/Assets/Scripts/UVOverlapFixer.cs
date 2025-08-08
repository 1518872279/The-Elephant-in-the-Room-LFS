using UnityEngine;
using System.Collections.Generic;

public class UVOverlapFixer : MonoBehaviour
{
    [Header("UV Overlap Fix Settings")]
    [Tooltip("Objects to fix UV overlap for")]
    public GameObject[] objectsToFix;
    
    [Tooltip("Minimum distance between objects")]
    public float minDistance = 1f;
    
    [Tooltip("Random rotation range in degrees")]
    public float rotationRange = 5f;
    
    [Tooltip("Random scale variation")]
    public float scaleVariation = 0.1f;
    
    [Header("Auto Fix")]
    [Tooltip("Automatically fix UV overlap on start")]
    public bool autoFixOnStart = true;
    
    void Start()
    {
        if (autoFixOnStart)
        {
            FixUVOverlap();
        }
    }
    
    [ContextMenu("Fix UV Overlap")]
    public void FixUVOverlap()
    {
        if (objectsToFix == null || objectsToFix.Length == 0)
        {
            Debug.LogWarning("No objects assigned to fix UV overlap");
            return;
        }
        
        List<Vector3> positions = new List<Vector3>();
        
        foreach (GameObject obj in objectsToFix)
        {
            if (obj == null) continue;
            
            Vector3 newPosition = FindSafePosition(obj.transform.position, positions);
            obj.transform.position = newPosition;
            positions.Add(newPosition);
            
            // Add random rotation
            Vector3 randomRotation = new Vector3(
                Random.Range(-rotationRange, rotationRange),
                Random.Range(0, 360),
                Random.Range(-rotationRange, rotationRange)
            );
            obj.transform.rotation = Quaternion.Euler(randomRotation);
            
            // Add random scale variation
            float scaleFactor = 1f + Random.Range(-scaleVariation, scaleVariation);
            obj.transform.localScale *= scaleFactor;
            
            Debug.Log($"Fixed UV overlap for: {obj.name}");
        }
        
        Debug.Log($"Fixed UV overlap for {positions.Count} objects");
    }
    
    private Vector3 FindSafePosition(Vector3 originalPosition, List<Vector3> existingPositions)
    {
        Vector3 newPosition = originalPosition;
        int attempts = 0;
        const int maxAttempts = 100;
        
        while (attempts < maxAttempts)
        {
            bool isSafe = true;
            
            foreach (Vector3 existingPos in existingPositions)
            {
                if (Vector3.Distance(newPosition, existingPos) < minDistance)
                {
                    isSafe = false;
                    break;
                }
            }
            
            if (isSafe)
            {
                return newPosition;
            }
            
            // Try a new random position
            newPosition = originalPosition + new Vector3(
                Random.Range(-minDistance, minDistance),
                0,
                Random.Range(-minDistance, minDistance)
            );
            
            attempts++;
        }
        
        Debug.LogWarning($"Could not find safe position for object at {originalPosition}");
        return originalPosition;
    }
    
    [ContextMenu("Reset All Objects")]
    public void ResetAllObjects()
    {
        foreach (GameObject obj in objectsToFix)
        {
            if (obj == null) continue;
            
            obj.transform.localScale = Vector3.one;
            obj.transform.rotation = Quaternion.identity;
            
            Debug.Log($"Reset: {obj.name}");
        }
    }
} 