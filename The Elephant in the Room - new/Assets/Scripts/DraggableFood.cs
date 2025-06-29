using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DraggableFood : MonoBehaviour
{
    private Camera cam;
    private Vector3 offset;
    private float zDepth;
    private bool isDragging = false;

    [Header("Pan Bounds")]
    [Tooltip("Collider of the pan to clamp dropped items inside its bounds")]
    public Collider panCollider;

    void Start()
    {
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("DraggableFood: Camera.main is null! Make sure there's a camera tagged as 'MainCamera' in the scene.");
        }
    }

    void OnMouseDown()
    {
        if (cam == null) return;
        
        isDragging = true;
        zDepth = cam.WorldToScreenPoint(transform.position).z;
        Vector3 screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDepth);
        offset = transform.position - cam.ScreenToWorldPoint(screenPoint);
        
        // Disable physics during drag
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
    }

    void OnMouseDrag()
    {
        if (cam == null || !isDragging) return;
        
        Vector3 curScreen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDepth);
        Vector3 curWorld = cam.ScreenToWorldPoint(curScreen) + offset;
        transform.position = curWorld;
    }

    void OnMouseUp()
    {
        isDragging = false;
        
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true; // Keep kinematic after drop
        
        if (panCollider != null)
        {
            // Clamp position to inside the pan collider
            Vector3 clampedPos = panCollider.ClosestPoint(transform.position);
            transform.position = clampedPos;
            
            // Parent to pan for static positioning
            transform.SetParent(panCollider.transform);
        }
    }
} 