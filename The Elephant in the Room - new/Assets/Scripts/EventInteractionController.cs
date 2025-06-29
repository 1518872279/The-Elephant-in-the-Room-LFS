using UnityEngine;

[RequireComponent(typeof(Camera))]
public class EventInteractionController : MonoBehaviour
{
    [Header("Event Interaction Settings")]
    public LayerMask eventLayer;
    public float interactDistance = 3f;
    public Camera cam;

    void Start()
    {
        if (cam == null) cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, eventLayer))
            {
                var evtObj = hit.collider.GetComponent<EventObject>();
                if (evtObj)
                {
                    // Special handling for Breakfast event
                    if (evtObj.eventName == "Breakfast")
                    {
                        if (CookingMinigameController.Instance != null)
                        {
                            CookingMinigameController.Instance.StartMinigame();
                        }
                        else
                        {
                            Debug.LogWarning("CookingMinigameController not found in scene.");
                        }
                    }
                    else
                    {
                        // Regular event handling
                        bool started = TimeManager.Instance.TryStartEvent(evtObj.eventName);
                        Debug.Log("time procceed " + TimeManager.Instance.currentTime);
                        if (!started)
                            Debug.LogWarning($"Failed to trigger event '{evtObj.eventName}'.");
                    }
                }
            }
        }
    }
} 