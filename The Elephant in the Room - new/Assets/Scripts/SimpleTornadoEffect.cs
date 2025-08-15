using UnityEngine;
using System.Collections.Generic;

public class SimpleTornadoEffect : MonoBehaviour
{
    [Header("Tornado Settings")]
    public float rotationSpeed = 50f;                // Degrees per second
    public float upwardSpeed = 2f;                   // Units per second
    public float radius = 5f;                        // Radius of the spinning circle
    public float height = 10f;                       // Maximum height objects can reach
    public float floatDuration = 3f;                 // How long objects float at max height
    public float startDelayRange = 3f;               // Random delay range for starting (0 to this value)
    
    [Header("Objects to Spin")]
    public List<GameObject> objectsToSpin = new List<GameObject>();
    
    [Header("Visual Effects")]
    public bool showGizmos = true;
    public Color gizmoColor = Color.red;
    
    [Header("Animation")]
    public bool autoStart = true;
    public bool isSpinning = false;
    
    private List<SpinningObject> spinningObjects = new List<SpinningObject>();
    
    [System.Serializable]
    public class SpinningObject
    {
        public GameObject gameObject;
        public Vector3 originalPosition;
        public float currentAngle;
        public float currentHeight;
        public float heightOffset; // Random offset for variety
        public float startDelay; // Random delay before starting to move
        public float floatTime; // Time spent floating at max height
        public bool isFloating; // Whether object is currently floating
        
        public SpinningObject(GameObject obj, Vector3 center, float radius, float delayRange)
        {
            gameObject = obj;
            originalPosition = obj.transform.position;
            
            // Calculate initial angle based on position relative to center
            Vector3 offset = obj.transform.position - center;
            currentAngle = Mathf.Atan2(offset.z, offset.x) * Mathf.Rad2Deg;
            
            // Set random height offset for variety
            heightOffset = Random.Range(0f, 2f);
            currentHeight = obj.transform.position.y;
            
            // Set random start delay
            startDelay = Random.Range(0f, delayRange);
            floatTime = 0f;
            isFloating = false;
        }
    }
    
    void Start()
    {
        InitializeSpinningObjects();
        
        if (autoStart)
        {
            StartSpinning();
        }
    }
    
    void InitializeSpinningObjects()
    {
        spinningObjects.Clear();
        
        foreach (GameObject obj in objectsToSpin)
        {
            if (obj != null)
            {
                SpinningObject spinningObj = new SpinningObject(obj, transform.position, radius, startDelayRange);
                spinningObjects.Add(spinningObj);
            }
        }
    }
    
    void Update()
    {
        if (isSpinning)
        {
            UpdateSpinningObjects();
        }
    }
    
    void UpdateSpinningObjects()
    {
        foreach (SpinningObject spinningObj in spinningObjects)
        {
            if (spinningObj.gameObject == null) continue;
            
            // Check if object should start moving yet
            if (spinningObj.startDelay > 0)
            {
                spinningObj.startDelay -= Time.deltaTime;
                continue;
            }
            
            // Update angle
            spinningObj.currentAngle += rotationSpeed * Time.deltaTime;
            
            // Handle height and floating
            if (!spinningObj.isFloating)
            {
                // Update height
                spinningObj.currentHeight += upwardSpeed * Time.deltaTime;
                
                // Check if reached max height
                if (spinningObj.currentHeight >= height + spinningObj.heightOffset)
                {
                    spinningObj.currentHeight = height + spinningObj.heightOffset;
                    spinningObj.isFloating = true;
                    spinningObj.floatTime = 0f;
                }
            }
            else
            {
                // Object is floating at max height
                spinningObj.floatTime += Time.deltaTime;
                
                // After float duration, reset to bottom
                if (spinningObj.floatTime >= floatDuration)
                {
                    spinningObj.currentHeight = transform.position.y + spinningObj.heightOffset;
                    spinningObj.isFloating = false;
                    spinningObj.floatTime = 0f;
                }
            }
            
            // Calculate new position
            float radians = spinningObj.currentAngle * Mathf.Deg2Rad;
            float x = transform.position.x + Mathf.Cos(radians) * radius;
            float z = transform.position.z + Mathf.Sin(radians) * radius;
            float y = spinningObj.currentHeight;
            
            // Apply position
            spinningObj.gameObject.transform.position = new Vector3(x, y, z);
            
            // Optional: Add some rotation to the objects themselves
            spinningObj.gameObject.transform.Rotate(Vector3.up, rotationSpeed * 0.5f * Time.deltaTime);
        }
    }
    
    public void StartSpinning()
    {
        isSpinning = true;
        Debug.Log("Tornado spinning started!");
    }
    
    public void StopSpinning()
    {
        isSpinning = false;
        Debug.Log("Tornado spinning stopped!");
    }
    
    public void ToggleSpinning()
    {
        if (isSpinning)
            StopSpinning();
        else
            StartSpinning();
    }
    
    public void ResetObjects()
    {
        foreach (SpinningObject spinningObj in spinningObjects)
        {
            if (spinningObj.gameObject != null)
            {
                spinningObj.gameObject.transform.position = spinningObj.originalPosition;
                spinningObj.currentAngle = 0f;
                spinningObj.currentHeight = spinningObj.originalPosition.y;
                spinningObj.isFloating = false;
                spinningObj.floatTime = 0f;
                spinningObj.startDelay = Random.Range(0f, startDelayRange);
            }
        }
    }
    
    public void AddObject(GameObject obj)
    {
        if (obj != null && !objectsToSpin.Contains(obj))
        {
            objectsToSpin.Add(obj);
            SpinningObject spinningObj = new SpinningObject(obj, transform.position, radius, startDelayRange);
            spinningObjects.Add(spinningObj);
        }
    }
    
    public void RemoveObject(GameObject obj)
    {
        if (objectsToSpin.Contains(obj))
        {
            objectsToSpin.Remove(obj);
            spinningObjects.RemoveAll(so => so.gameObject == obj);
        }
    }
    
    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        Gizmos.color = gizmoColor;
        
        // Draw spinning circle
        Vector3 center = transform.position;
        int segments = 32;
        for (int i = 0; i < segments; i++)
        {
            float angle1 = (float)i / segments * 360f * Mathf.Deg2Rad;
            float angle2 = (float)(i + 1) / segments * 360f * Mathf.Deg2Rad;
            
            Vector3 point1 = center + new Vector3(Mathf.Cos(angle1) * radius, 0, Mathf.Sin(angle1) * radius);
            Vector3 point2 = center + new Vector3(Mathf.Cos(angle2) * radius, 0, Mathf.Sin(angle2) * radius);
            
            Gizmos.DrawLine(point1, point2);
        }
        
        // Draw height range
        Gizmos.DrawLine(center, center + Vector3.up * height);
        
        // Draw center point
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, 0.5f);
    }
    
    // Public properties for external control
    public bool IsSpinning => isSpinning;
    public int ObjectCount => spinningObjects.Count;
} 