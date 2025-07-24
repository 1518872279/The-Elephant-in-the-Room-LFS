# Core Systems Setup

This document summarizes the foundational systems for your Unity project and provides the complete scripts for Cursor to ingest.

---

## 1. First-Person Controller

**Overview:** Handles player movement, camera look, and object interaction.

**Setup Steps:**

1. Create a **Player** GameObject.
2. Add a **CharacterController** component.
3. Add a **Camera** as a child and assign to `cameraTransform`.
4. Define an “Interactable” layer for clickable objects.

````csharp
// StormController.cs
using UnityEngine;

public class StormController : MonoBehaviour
{
    public static StormController Instance;

    [Header("Subsystem References")]
    public GameObject rainSystem;                   // RainSystem GameObject
    public RainOverlayController rainOverlay;       // UI overlay controller
    public WindZone windZone;                       // Scene WindZone
    public LightningController lightningController; // Lightning controller component
    public AudioSource stormAudio;                  // Rain/wind ambience AudioSource

    private bool stormActive;
    private ParticleSystem rainParticles;

    void Awake()
    {
        Instance = this;
        // Cache particle system
        if (rainSystem != null)
            rainParticles = rainSystem.GetComponent<ParticleSystem>();
        DeactivateStorm(); // ensure off at start
    }

    public void ActivateStorm()
    {
        if (stormActive) return;
        stormActive = true;

        // Rain particles
        if (rainSystem != null)
            rainSystem.SetActive(true);
        if (rainParticles != null)
            rainParticles.Play();

        // UI overlay
        if (rainOverlay != null && rainOverlay.overlay != null)
        {
            rainOverlay.overlay.gameObject.SetActive(true);
            rainOverlay.overlay.enabled = true;
        }

        // Wind and lightning
        if (windZone != null)
            windZone.gameObject.SetActive(true);
        if (lightningController != null)
            lightningController.enabled = true;

        // Audio
        if (stormAudio != null)
            stormAudio.Play();
    }

    public void DeactivateStorm()
    {
        stormActive = false;

        // Rain particles
        if (rainParticles != null)
        {
            rainParticles.Stop();
            rainSystem.SetActive(false);
        }

        // UI overlay
        if (rainOverlay != null && rainOverlay.overlay != null)
            rainOverlay.overlay.gameObject.SetActive(false);

        // Wind and lightning
        if (windZone != null)
            windZone.gameObject.SetActive(false);
        if (lightningController != null)
            lightningController.enabled = false;

        // Audio
        if (stormAudio != null)
            stormAudio.Stop();
    }
}
```*
````
14. Day Transition Mechanics

Allows instant time jumps between Morning and Evening when interacting with the door or bed, complete with UI fade transitions.

14.1. Extend TimeManager

Add a method to manually set the in-game clock:

// In TimeManager.cs
public void SetTime(int minutes)
{
    currentTime = minutes;
    OnTimeChanged?.Invoke(currentTime);
}

14.2. DoorTransition (Morning → Evening)

Attach this to your MainDoor object (implementing IInteractable):

// DoorTransition.cs
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

14.3. BedTransition (Evening → Morning)

Attach to the Bed object (implementing IInteractable):

// BedTransition.cs
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

End of setup.
