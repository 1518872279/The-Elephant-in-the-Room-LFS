using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DoorTransition : MonoBehaviour, IInteractable
{
    public Image fadeImage;
    public float fadeDuration = 1f;
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
            StartCoroutine(Transition(TimeManager.Instance.eveningStart));
    }

    private IEnumerator Transition(int targetTime)
    {
        isBusy = true;
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
        isBusy = false;
    }
} 