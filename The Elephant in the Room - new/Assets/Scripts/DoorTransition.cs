using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DoorTransition : MonoBehaviour, IInteractable
{
    [Header("Transition Settings")]
    public Image fadeImage;
    public float fadeDuration = 1f;
    
    [Header("UI Feedback")]
    public TextMeshProUGUI transitionText;
    public string transitionMessage = "Entering...";
    
    [Header("Time Context")]
    public TextMeshProUGUI timeContextText;
    public string morningToEveningMessage = "Time to start your evening routine";
    public string eveningToMorningMessage = "Welcome to a new morning";
    
    private bool isBusy;

    public void Interact()
    {
        if (isBusy) return;
        
        // Ensure fade image is available
        if (fadeImage == null)
        {
            Debug.LogError("DoorTransition: No fade image assigned!");
            return;
        }
        
        // Ensure the canvas is enabled
        Canvas canvas = fadeImage.GetComponentInParent<Canvas>();
        if (canvas != null && !canvas.enabled)
        {
            Debug.Log("DoorTransition: Enabling transition canvas");
            canvas.enabled = true;
        }
        
        int min = TimeManager.Instance.GetCurrentTime();
        if (min >= TimeManager.Instance.morningStart && min < TimeManager.Instance.morningEnd)
        {
            // Show contextual message
            if (timeContextText != null)
                timeContextText.text = morningToEveningMessage;
                
            StartCoroutine(Transition(TimeManager.Instance.eveningStart));
        }
    }

    private IEnumerator Transition(int targetTime)
    {
        isBusy = true;
        
        // Show transition text
        if (transitionText != null)
        {
            transitionText.text = transitionMessage;
            transitionText.gameObject.SetActive(true);
        }
        
        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            fadeImage.color = new Color(0,0,0,t/fadeDuration);
            yield return null;
        }
        
        // Jump to evening
        TimeManager.Instance.SetTime(targetTime);
        
        // Fade in
        for (float t = fadeDuration; t > 0; t -= Time.deltaTime)
        {
            fadeImage.color = new Color(0,0,0,t/fadeDuration);
            yield return null;
        }
        
        // Hide transition text
        if (transitionText != null)
            transitionText.gameObject.SetActive(false);
            
        isBusy = false;
    }
} 