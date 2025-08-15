using UnityEngine;
using System.Collections.Generic;

public class TornadoController : MonoBehaviour
{
    [Header("Tornado Settings")]
    public float radius = 10f;                      // Radius of tornado effect
    public float maxForce = 50f;                    // Maximum force applied to objects
    public float upwardForce = 20f;                 // Upward force component
    public float rotationSpeed = 2f;                // Speed of rotation around center
    public float height = 20f;                      // Height of tornado effect
    public LayerMask affectedLayers = -1;           // Layers affected by tornado
    
    [Header("Visual Effects")]
    public bool showGizmos = true;                  // Show tornado radius in scene view
    public Color gizmoColor = Color.red;
    
    [Header("Object Management")]
    public float objectLifetime = 10f;              // How long objects stay in tornado
    public float ejectionForce = 15f;               // Force when objects are ejected
    
    private List<TornadoObject> affectedObjects = new List<TornadoObject>();
    private Vector3 tornadoCenter;
    
    [System.Serializable]
    public class TornadoObject
    {
        public Rigidbody rb;
        public float entryTime;
        public Vector3 originalPosition;
        public bool isEjected;
        
        public TornadoObject(Rigidbody rigidbody, Vector3 position)
        {
            rb = rigidbody;
            entryTime = Time.time;
            originalPosition = position;
            isEjected = false;
        }
    }
    
    void Start()
    {
        tornadoCenter = transform.position;
    }
    
    void Update()
    {
        tornadoCenter = transform.position;
        ApplyTornadoForces();
        ManageObjectLifetime();
    }
    
    void ApplyTornadoForces()
    {
        // Find all objects within tornado radius using multiple sphere casts
        Vector3 baseCenter = tornadoCenter;
        Vector3 topCenter = tornadoCenter + Vector3.up * height;
        
        // Use multiple sphere casts to approximate cylinder detection
        int numChecks = Mathf.CeilToInt(height / 2f); // Check every 2 units in height
        for (int i = 0; i <= numChecks; i++)
        {
            Vector3 checkPoint = Vector3.Lerp(baseCenter, topCenter, (float)i / numChecks);
            Collider[] colliders = Physics.OverlapSphere(checkPoint, radius, affectedLayers);
            
            foreach (Collider col in colliders)
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb != null && !rb.isKinematic)
                {
                    // Check if object is already being tracked
                    TornadoObject tornadoObj = affectedObjects.Find(obj => obj.rb == rb);
                    if (tornadoObj == null)
                    {
                        tornadoObj = new TornadoObject(rb, rb.position);
                        affectedObjects.Add(tornadoObj);
                    }
                    
                    ApplyTornadoPhysics(tornadoObj);
                }
            }
        }
    }
    
    void ApplyTornadoPhysics(TornadoObject tornadoObj)
    {
        if (tornadoObj.isEjected) return;
        
        Rigidbody rb = tornadoObj.rb;
        Vector3 objectPos = rb.position;
        
        // Calculate distance from tornado center
        Vector3 horizontalOffset = objectPos - tornadoCenter;
        horizontalOffset.y = 0; // Ignore height difference for horizontal calculations
        float distance = horizontalOffset.magnitude;
        
        if (distance > radius) return;
        
        // Calculate force based on distance from center
        float forceMultiplier = 1f - (distance / radius);
        forceMultiplier = Mathf.Clamp01(forceMultiplier);
        
        // Calculate rotation direction (tangent to circle)
        Vector3 rotationDirection = Vector3.Cross(Vector3.up, horizontalOffset.normalized);
        
        // Apply rotational force
        Vector3 rotationalForce = rotationDirection * maxForce * forceMultiplier * rotationSpeed;
        rb.AddForce(rotationalForce, ForceMode.Force);
        
        // Apply upward force
        float heightFactor = 1f - (objectPos.y - tornadoCenter.y) / height;
        heightFactor = Mathf.Clamp01(heightFactor);
        Vector3 upwardForceVector = Vector3.up * upwardForce * forceMultiplier * heightFactor;
        rb.AddForce(upwardForceVector, ForceMode.Force);
        
        // Add some randomness to make it more realistic
        Vector3 randomForce = Random.insideUnitSphere * maxForce * 0.1f * forceMultiplier;
        rb.AddForce(randomForce, ForceMode.Force);
    }
    
    void ManageObjectLifetime()
    {
        for (int i = affectedObjects.Count - 1; i >= 0; i--)
        {
            TornadoObject tornadoObj = affectedObjects[i];
            
            // Check if object still exists
            if (tornadoObj.rb == null)
            {
                affectedObjects.RemoveAt(i);
                continue;
            }
            
            // Check if object has been in tornado too long
            if (Time.time - tornadoObj.entryTime > objectLifetime && !tornadoObj.isEjected)
            {
                EjectObject(tornadoObj);
            }
            
            // Remove ejected objects after a delay
            if (tornadoObj.isEjected && Time.time - tornadoObj.entryTime > objectLifetime + 2f)
            {
                affectedObjects.RemoveAt(i);
            }
        }
    }
    
    void EjectObject(TornadoObject tornadoObj)
    {
        if (tornadoObj.isEjected) return;
        
        tornadoObj.isEjected = true;
        
        // Calculate ejection direction (away from tornado center)
        Vector3 ejectionDirection = (tornadoObj.rb.position - tornadoCenter).normalized;
        ejectionDirection.y = 0.5f; // Add some upward component
        
        // Apply ejection force
        tornadoObj.rb.AddForce(ejectionDirection * ejectionForce, ForceMode.Impulse);
        
        // Add some random rotation
        tornadoObj.rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
    }
    
    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        Gizmos.color = gizmoColor;
        
        // Draw tornado radius using multiple wire spheres to approximate cylinder
        Vector3 baseCenter = transform.position;
        Vector3 topCenter = transform.position + Vector3.up * height;
        int numRings = Mathf.CeilToInt(height / 2f);
        
        for (int i = 0; i <= numRings; i++)
        {
            Vector3 ringCenter = Vector3.Lerp(baseCenter, topCenter, (float)i / numRings);
            Gizmos.DrawWireSphere(ringCenter, radius);
        }
        
        // Draw vertical lines to connect the rings
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f * Mathf.Deg2Rad;
            Vector3 basePoint = baseCenter + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Vector3 topPoint = topCenter + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(basePoint, topPoint);
        }
        
        // Draw center point
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Draw affected objects
        Gizmos.color = Color.red;
        foreach (TornadoObject tornadoObj in affectedObjects)
        {
            if (tornadoObj.rb != null)
            {
                Gizmos.DrawLine(transform.position, tornadoObj.rb.position);
            }
        }
    }
    
    // Public methods for external control
    public void SetRadius(float newRadius)
    {
        radius = newRadius;
    }
    
    public void SetForce(float newForce)
    {
        maxForce = newForce;
    }
    
    public void SetRotationSpeed(float newSpeed)
    {
        rotationSpeed = newSpeed;
    }
    
    public int GetAffectedObjectCount()
    {
        return affectedObjects.Count;
    }
    
    public void ClearAllObjects()
    {
        affectedObjects.Clear();
    }
} 