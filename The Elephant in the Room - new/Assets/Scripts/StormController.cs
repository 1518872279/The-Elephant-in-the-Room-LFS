using UnityEngine;
using System.Collections.Generic;

public class StormController : MonoBehaviour
{
    public static StormController Instance;

    [Header("Subsystem References")]
    public GameObject rainSystem;                   // RainSystem GameObject
    public RainOverlayController rainOverlay;       // UI overlay controller
    public WindZone windZone;                       // Scene WindZone
    public LightningController lightningController; // Lightning controller component
    public AudioSource stormAudio;                  // Rain/wind ambience AudioSource

    [Header("Light Flickering")]
    public List<Light> lightsToFlicker = new List<Light>();  // Lights that will flicker during storm
    public bool autoFindLights = true;                       // Automatically find lights in scene
    public float flickerIntensity = 0.3f;                    // How much the lights flicker (0-1)
    public float flickerSpeed = 2f;                          // Speed of flickering
    public float flickerRandomness = 0.5f;                   // Randomness of flicker timing
    public bool restoreOriginalIntensity = true;             // Restore original light intensity when storm ends

    [Header("Debug")]
    [SerializeField] private bool stormActive;
    
    private ParticleSystem rainParticles;
    private List<LightFlickerData> lightFlickerData = new List<LightFlickerData>();

    public bool IsStormActive => stormActive;

    [System.Serializable]
    public class LightFlickerData
    {
        public Light light;
        public float originalIntensity;
        public float flickerTimer;
        public float flickerOffset;
        
        public LightFlickerData(Light lightComponent)
        {
            light = lightComponent;
            originalIntensity = lightComponent.intensity;
            flickerTimer = 0f;
            flickerOffset = Random.Range(0f, 2f * Mathf.PI); // Random phase offset
        }
    }

    void Awake()
    {
        Instance = this;
        // Cache particle system
        if (rainSystem != null)
            rainParticles = rainSystem.GetComponent<ParticleSystem>();
        
        // Initialize light flickering
        InitializeLightFlickering();
        
        DeactivateStorm(); // ensure off at start
    }

    void InitializeLightFlickering()
    {
        lightFlickerData.Clear();
        
        // Auto-find lights if enabled
        if (autoFindLights)
        {
            Light[] allLights = FindObjectsOfType<Light>();
            foreach (Light light in allLights)
            {
                if (light != null && light.enabled)
                {
                    lightsToFlicker.Add(light);
                }
            }
        }
        
        // Initialize flicker data for all lights
        foreach (Light light in lightsToFlicker)
        {
            if (light != null)
            {
                LightFlickerData flickerData = new LightFlickerData(light);
                lightFlickerData.Add(flickerData);
            }
        }
    }

    void Update()
    {
        if (stormActive)
        {
            UpdateLightFlickering();
        }
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

    void UpdateLightFlickering()
    {
        foreach (LightFlickerData flickerData in lightFlickerData)
        {
            if (flickerData.light == null) continue;
            
            // Update flicker timer with randomness
            flickerData.flickerTimer += Time.deltaTime * flickerSpeed * (1f + Random.Range(-flickerRandomness, flickerRandomness));
            
            // Calculate flicker intensity using sine wave with random offset
            float flickerValue = Mathf.Sin(flickerData.flickerTimer + flickerData.flickerOffset);
            
            // Apply flicker to light intensity
            float newIntensity = flickerData.originalIntensity * (1f - flickerIntensity * (1f - flickerValue) * 0.5f);
            flickerData.light.intensity = Mathf.Max(0f, newIntensity);
        }
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
        
        // Restore light intensities
        if (restoreOriginalIntensity)
        {
            RestoreLightIntensities();
        }
    }

    void RestoreLightIntensities()
    {
        foreach (LightFlickerData flickerData in lightFlickerData)
        {
            if (flickerData.light != null)
            {
                flickerData.light.intensity = flickerData.originalIntensity;
            }
        }
    }

    public void ToggleStorm()
    {
        if (stormActive)
            DeactivateStorm();
        else
            ActivateStorm();
    }

    // Public methods for light flickering control
    public void AddLightToFlicker(Light light)
    {
        if (light != null && !lightsToFlicker.Contains(light))
        {
            lightsToFlicker.Add(light);
            LightFlickerData flickerData = new LightFlickerData(light);
            lightFlickerData.Add(flickerData);
        }
    }

    public void RemoveLightFromFlicker(Light light)
    {
        if (lightsToFlicker.Contains(light))
        {
            lightsToFlicker.Remove(light);
            lightFlickerData.RemoveAll(data => data.light == light);
            
            // Restore original intensity if storm is active
            if (stormActive && restoreOriginalIntensity)
            {
                light.intensity = light.intensity; // This will be restored properly when storm ends
            }
        }
    }

    public void SetFlickerIntensity(float intensity)
    {
        flickerIntensity = Mathf.Clamp01(intensity);
    }

    public void SetFlickerSpeed(float speed)
    {
        flickerSpeed = Mathf.Max(0f, speed);
    }
}