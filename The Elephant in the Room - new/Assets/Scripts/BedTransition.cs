using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BedTransition : MonoBehaviour, IInteractable
{
    [Header("Transition Settings")]
    public Image fadeImage;
    public float fadeDuration = 1f;
    
    [Header("UI Feedback")]
    public TextMeshProUGUI transitionText;
    public string transitionMessage = "Sleeping...";
    
    [Header("Time Context")]
    public TextMeshProUGUI timeContextText;
    public string eveningToMorningMessage = "Good morning! Time to start a new day";
    public string morningToEveningMessage = "Time to rest for the evening";
    
    [Header("Day Advancement")]
    [Tooltip("Whether sleeping should advance to the next day")]
    public bool advanceDayOnSleep = true;
    
    private bool isBusy;

    public void Interact()
    {
        if (isBusy) return;
        
        // Ensure fade image is available
        if (fadeImage == null)
        {
            Debug.LogError("BedTransition: No fade image assigned!");
            return;
        }
        
        // Ensure the canvas is enabled
        Canvas canvas = fadeImage.GetComponentInParent<Canvas>();
        if (canvas != null && !canvas.enabled)
        {
            Debug.Log("BedTransition: Enabling transition canvas");
            canvas.enabled = true;
        }
        
        int min = TimeManager.Instance.GetCurrentTime();
        if (min >= TimeManager.Instance.eveningStart && min < TimeManager.Instance.eveningEnd)
        {
            // Show contextual message
            if (timeContextText != null)
                timeContextText.text = eveningToMorningMessage;
                
            StartCoroutine(Transition(TimeManager.Instance.morningStart));
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
        
        // Jump to morning and advance day
        TimeManager.Instance.SetTime(targetTime);
        
        // Advance to next day if enabled
        if (advanceDayOnSleep)
        {
            TimeManager.Instance.AdvanceDay();
        }
        
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