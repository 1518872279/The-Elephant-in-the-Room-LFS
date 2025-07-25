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

15. Door Open/Close Mechanic

Allows the player to press E when near the door to smoothly open or close it around its hinge pivot.

15.1. Scene Setup

Door Object: Ensure your door has a child Hinge Transform located at the desired pivot (e.g., the hinge side).

Collider: Add a BoxCollider (set Is Trigger) around the door’s area. Tag the player GameObject (default: "Player").

15. Door Open/Close Mechanic

Allows the player to press E when near the door to smoothly open or close it around its hinge pivot.

15.1. Scene Setup

Door Object & Hinge: Your visible Door should have a child Hinge Transform at its pivot point.

Trigger Zone: Create a new empty child GameObject named DoorTriggerZone under the door parent:

Add a BoxCollider component.

Set Is Trigger = true.

Position and size this box to cover the area where the player should auto‐open/close the door (e.g. directly in front of it).

Tag or Layer: Ensure the player GameObject is tagged (default: "Player") or on a detectable layer.

15.2. DoorOpenClose.cs

Attach this script to the DoorTriggerZone GameObject (not the Door itself). Then in the Inspector:

Hinge: drag the Door’s Hinge transform here.

Player Tag: set to your player’s tag (e.g. "Player").

// DoorOpenClose.cs
using UnityEngine;
using System.Collections;

public class DoorOpenClose : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Transform hinge;              // Pivot for rotation (the actual door hinge)
    public float openAngle = 90f;        // Degrees to open
    public float openSpeed = 3f;         // Lerp speed
    public string playerTag = "Player"; // Tag used for detecting player entry/exit

    private bool isOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        // Cache the start rotation of the door hinge
        if (hinge == null)
            Debug.LogError("Hinge Transform not set on DoorOpenClose.");
        closedRot = hinge.localRotation;
        openRot   = closedRot * Quaternion.Euler(0f, openAngle, 0f);
    }

    // Trigger events now fire on this trigger zone object
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !isOpen)
        {
            StopAllCoroutines();
            StartCoroutine(RotateDoor(openRot));
            isOpen = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && isOpen)
        {
            StopAllCoroutines();
            StartCoroutine(RotateDoor(closedRot));
            isOpen = false;
        }
    }

    private IEnumerator RotateDoor(Quaternion target)
    {
        // Smoothly slerp hinge rotation towards target
        while (Quaternion.Angle(hinge.localRotation, target) > 0.1f)
        {
            hinge.localRotation = Quaternion.Slerp(
                hinge.localRotation,
                target,
                Time.deltaTime * openSpeed);
            yield return null;
        }
        hinge.localRotation = target;
    }
}

End of setup. DoorOpenClose.cs
Attach this to the Door GameObject (or its parent) and assign the hinge Transform:

// DoorOpenClose.cs
ing UnityEngine;
using System.Collections;

public class DoorOpenClose : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Transform hinge;              // Pivot for rotation
    public float openAngle = 90f;        // Degrees to open
    public float openSpeed = 3f;         // Lerp speed
    public string playerTag = "Player"; // Tag used for detecting player

    private bool isOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        if (hinge == null)
            hinge = transform;
        closedRot = hinge.localRotation;
        openRot   = closedRot * Quaternion.Euler(0f, openAngle, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !isOpen)
        {
            StopAllCoroutines();
            StartCoroutine(RotateDoor(openRot));
            isOpen = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && isOpen)
        {
            StopAllCoroutines();
            StartCoroutine(RotateDoor(closedRot));
            isOpen = false;
        }
    }

    private IEnumerator RotateDoor(Quaternion target)
    {
        while (Quaternion.Angle(hinge.localRotation, target) > 0.1f)
        {
            hinge.localRotation = Quaternion.Slerp(
                hinge.localRotation,
                target,
                Time.deltaTime * openSpeed);
            yield return null;
        }
        hinge.localRotation = target;
    }
}

End of setup.*