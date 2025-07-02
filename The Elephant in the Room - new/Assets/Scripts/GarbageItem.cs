using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GarbageItem : MonoBehaviour
{
    public float interactDistance = 2f;
    private Camera cam;

    void Start() => cam = Camera.main;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance)
                && hit.collider.gameObject == gameObject)
            {
                // Notify controller and clean
                if (GarbageCleanupController.Instance != null)
                {
                    GarbageCleanupController.Instance.ItemCleaned();
                }
                Destroy(gameObject);
            }
        }
    }
} 