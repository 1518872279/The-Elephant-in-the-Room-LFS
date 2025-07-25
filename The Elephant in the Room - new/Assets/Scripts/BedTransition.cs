using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BedTransition : MonoBehaviour, IInteractable
{
    public Image fadeImage;
    public float fadeDuration = 1f;
    private bool isBusy;

    public void Interact()
    {
        if (isBusy) return;
        int min = TimeManager.Instance.GetCurrentTime();
        if (min >= TimeManager.Instance.eveningStart && min < TimeManager.Instance.eveningEnd)
            StartCoroutine(Transition(TimeManager.Instance.morningStart));
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
        // Jump to morning
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