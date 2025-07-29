using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

//[RequireComponent(typeof(FirstPersonController))]
public class PhoneUIController : MonoBehaviour
{
    [Header("UI and Post-Process References")]
    public GameObject phonePanel;
    public Volume postProcessVolume;
    public FirstPersonController fpController;  // reference to player controller

    [Header("Phone Pages")]
    public GameObject HomePage;
    public List<GameObject> allPages; // All pages including HomePage

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
        
        // Deactivate all pages at start
        DeactivateAllPages();
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

        if (isActive)
        {
            // Phone is being opened - activate home page and deactivate others
            DeactivateAllPages();
            HomePage.SetActive(true);
            
            if (dof != null)
            {
                // Focus very close so the background blurs
                dof.focusDistance.value = 0.1f;
            }

            // Pause movement and show cursor
            fpController.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // Phone is being closed - deactivate all pages
            DeactivateAllPages();
            
            // Resume movement and hide cursor
            fpController.enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
    /// <summary>
    /// Deactivates all pages in the allPages list
    /// </summary>
    private void DeactivateAllPages()
    {
        if (allPages != null)
        {
            foreach (GameObject page in allPages)
            {
                if (page != null)
                    page.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// Simple page switching method - deactivates current page and activates target page
    /// </summary>
    public void SwitchToPage(GameObject targetPage)
    {
        if (targetPage == null) return;
        
        // Deactivate all pages first
        DeactivateAllPages();
        
        // Activate the target page
        targetPage.SetActive(true);
    }
} 