using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(Camera))]
public class InteractionHintController : MonoBehaviour
{
    public Camera cam;
    public Image hintImage;
    public float hintDistance = 3f;
    public LayerMask interactableLayers;

    [Header("Hint Sprites")]
    public Sprite defaultDot;
    public Sprite doorIcon;
    public Sprite handIcon;

    void Start()
    {
        if (cam == null) cam = GetComponent<Camera>();
        hintImage.enabled = false;
    }

    void Update()
    {
        // Raycast from screen center
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, hintDistance, interactableLayers))
        {
            // Determine icon based on object
            if (hit.collider.CompareTag("Door"))
                hintImage.sprite = doorIcon;
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Pickable"))
                hintImage.sprite = handIcon;
            else
                hintImage.sprite = defaultDot;

            hintImage.enabled = true;
        }
        else
        {
            hintImage.enabled = false;
        }
    }
} 