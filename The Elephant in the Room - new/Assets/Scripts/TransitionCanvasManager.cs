using UnityEngine;
using UnityEngine.UI;

public class TransitionCanvasManager : MonoBehaviour
{
    [Header("Transition Canvas Setup")]
    public Canvas transitionCanvas;
    public Image fadeImage;
    
    void Start()
    {
        // Ensure transition canvas is properly set up
        if (transitionCanvas != null)
        {
            // Make sure canvas is enabled
            if (!transitionCanvas.enabled)
            {
                Debug.Log("TransitionCanvasManager: Enabling transition canvas");
                transitionCanvas.enabled = true;
            }
            
            // Ensure canvas is set to Screen Space - Overlay for proper fade coverage
            if (transitionCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                Debug.Log("TransitionCanvasManager: Setting canvas to Screen Space - Overlay");
                transitionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            
            // Ensure canvas is on top
            transitionCanvas.sortingOrder = 999;
        }
        else
        {
            Debug.LogWarning("TransitionCanvasManager: No transition canvas assigned!");
        }
        
        // Ensure fade image is properly set up
        if (fadeImage != null)
        {
            // Make sure fade image is transparent at start
            fadeImage.color = new Color(0, 0, 0, 0);
            
            // Ensure fade image covers the full screen
            RectTransform rectTransform = fadeImage.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }
        }
        else
        {
            Debug.LogWarning("TransitionCanvasManager: No fade image assigned!");
        }
    }
    
    void OnValidate()
    {
        // Auto-find transition canvas if not assigned
        if (transitionCanvas == null)
        {
            transitionCanvas = FindObjectOfType<Canvas>();
            if (transitionCanvas != null)
            {
                Debug.Log($"TransitionCanvasManager: Auto-assigned canvas: {transitionCanvas.name}");
            }
        }
        
        // Auto-find fade image if not assigned
        if (fadeImage == null && transitionCanvas != null)
        {
            fadeImage = transitionCanvas.GetComponentInChildren<Image>();
            if (fadeImage != null)
            {
                Debug.Log($"TransitionCanvasManager: Auto-assigned fade image: {fadeImage.name}");
            }
        }
    }
} 