using UnityEngine;
using UnityEngine.UI;
using TMPro;

//[RequireComponent(typeof(Camera))]
public class InteractionHintController : MonoBehaviour
{
    public Camera cam;
    public Image hintImage;
    public TextMeshProUGUI hintText; // New: TextMeshPro component for hints
    public float hintDistance = 3f;
    public LayerMask interactableLayers;

    [Header("Hint Sprites")]
    public Sprite defaultDot;
    public Sprite doorIcon;
    public Sprite handIcon;
    public Sprite bedIcon; // New: Bed icon

    [Header("Hint Messages")]
    [Tooltip("Message shown when near doors")]
    public string doorMessage = "Press E to enter";
    [Tooltip("Message shown when near pickable objects")]
    public string pickableMessage = "Press E to pick up";
    [Tooltip("Message shown when near bed")]
    public string bedMessage = "Press E to sleep";
    [Tooltip("Message shown for other interactables")]
    public string defaultMessage = "Press E to interact";

    void Start()
    {
        if (cam == null) cam = GetComponent<Camera>();
        hintImage.enabled = false;
        if (hintText != null) hintText.enabled = false;
    }

    void Update()
    {
        // Raycast from screen center
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, hintDistance, interactableLayers))
        {
            // Determine icon and message based on object
            if (hit.collider.CompareTag("Door"))
            {
                hintImage.sprite = doorIcon;
                SetHintText(doorMessage);
            }
            else if (hit.collider.CompareTag("Bed"))
            {
                hintImage.sprite = bedIcon;
                SetHintText(bedMessage);
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Pickable"))
            {
                hintImage.sprite = handIcon;
                SetHintText(pickableMessage);
            }
            else
            {
                hintImage.sprite = defaultDot;
                SetHintText(defaultMessage);
            }

            hintImage.enabled = true;
            if (hintText != null) hintText.enabled = true;
        }
        else
        {
            hintImage.enabled = false;
            if (hintText != null) hintText.enabled = false;
        }
    }

    private void SetHintText(string message)
    {
        if (hintText != null)
        {
            hintText.text = message;
        }
    }
} 