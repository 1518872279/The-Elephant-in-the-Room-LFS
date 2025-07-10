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

    [Header("Debug")]
    [SerializeField] private bool stormActive;
    
    private ParticleSystem rainParticles;

    public bool IsStormActive => stormActive;

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

    public void ToggleStorm()
    {
        if (stormActive)
            DeactivateStorm();
        else
            ActivateStorm();
    }
} 