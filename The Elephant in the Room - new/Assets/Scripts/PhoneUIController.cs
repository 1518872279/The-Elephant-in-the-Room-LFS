using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//[RequireComponent(typeof(FirstPersonController))]
public class PhoneUIController : MonoBehaviour
{
    [Header("UI and Post-Process References")]
    public GameObject phonePanel;
    public Volume postProcessVolume;
    public FirstPersonController fpController;  // reference to player controller

    private DepthOfField dof;

    void Start()
    {
        // Start with UI and blur disabled
        phonePanel.SetActive(false);
        postProcessVolume.weight = 0f;

        // Cache the DepthOfField override
        if (!postProcessVolume.profile.TryGet(out dof))
            Debug.LogWarning("DepthOfField override not found on Volume Profile.");

        // Ensure controller reference
        if (fpController == null)
            fpController = GetComponent<FirstPersonController>();

        // Hide cursor initially
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            TogglePhone();
    }

    void TogglePhone()
    {
        bool isActive = !phonePanel.activeSelf;
        phonePanel.SetActive(isActive);
        postProcessVolume.weight = isActive ? 1f : 0f;

        if (isActive && dof != null)
        {
            // Focus very close so the background blurs
            dof.focusDistance.value = 0.1f;
        }

        // Pause movement and look
        fpController.enabled = !isActive;

        // Cursor lock/visibility
        if (isActive)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
} 