using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PhoneUIController : MonoBehaviour
{
    [Header("UI and Post-Process References")]
    public GameObject phonePanel;
    public Volume postProcessVolume;

    private DepthOfField dof;

    void Start()
    {
        // Start with UI and blur disabled
        phonePanel.SetActive(false);
        postProcessVolume.weight = 0f;

        // Cache the DepthOfField override
        if (!postProcessVolume.profile.TryGet(out dof))
            Debug.LogWarning("DepthOfField override not found on Volume Profile.");
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
        Debug.Log("phone's up");
        if (isActive && dof != null)
        {
            // Focus very close so the background blurs
            dof.focusDistance.value = 0.1f;
        }
    }
} 