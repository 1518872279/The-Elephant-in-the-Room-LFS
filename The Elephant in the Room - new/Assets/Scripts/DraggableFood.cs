using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DraggableFood : MonoBehaviour
{
    private Camera cam;
    private Vector3 offset;
    private float zDepth;

    [Header("Pan Bounds")]
    [Tooltip("Collider of the pan to clamp dropped items inside its bounds")]
    public Collider panCollider;

    [Header("Optional Cooked Prefab")]
    [Tooltip("For items that change upon cooking (e.g., eggs)")]
    public GameObject cookedPrefab;

    void Start()
    {
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        zDepth = cam.WorldToScreenPoint(transform.position).z;
        Vector3 screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDepth);
        offset = transform.position - cam.ScreenToWorldPoint(screenPoint);
    }

    void OnMouseDrag()
    {
        Vector3 curScreen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDepth);
        Vector3 curWorld = cam.ScreenToWorldPoint(curScreen) + offset;
        transform.position = curWorld;
    }

    void OnMouseUp()
    {
        // Disable physics
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        // Calculate drop position on pan surface
        Vector3 finalPos = transform.position;
        if (panCollider != null)
        {
            Transform panT = panCollider.transform;
            Plane panPlane = new Plane(panT.up, panT.position);
            Ray downRay = new Ray(transform.position + panT.up * 5f, -panT.up);
            if (panPlane.Raycast(downRay, out float dist))
            {
                Vector3 surfacePoint = downRay.GetPoint(dist);
                finalPos = panCollider.ClosestPoint(surfacePoint);
            }
        }

        if (cookedPrefab != null)
        {
            finalPos.y += 0.01f;
            // Instantiate cooked version with correct orientation and destroy raw
            Quaternion spawnRot = Quaternion.FromToRotation(Vector3.up, panCollider.transform.up);
            Instantiate(cookedPrefab, finalPos, spawnRot, panCollider.transform);
            Destroy(gameObject);
        }
        else
        {
            // Place original on pan surface
            transform.position = finalPos;
            transform.SetParent(panCollider.transform);
        }
    }
}