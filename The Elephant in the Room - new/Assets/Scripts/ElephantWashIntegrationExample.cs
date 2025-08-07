using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Example script showing how to integrate all elephant wash components.
/// This demonstrates the complete setup and can be used as a reference.
/// </summary>
public class ElephantWashIntegrationExample : MonoBehaviour
{
    [Header("Core Components")]
    [Tooltip("The elephant GameObject to wash")]
    public GameObject elephantObject;
    [Tooltip("The ElephantWashManager component")]
    public ElephantWashManager elephantWashManager;
    
    [Tooltip("The water gun GameObject")]
    public GameObject waterGun;
    
    [Tooltip("The wash trigger GameObject")]
    public ElephantWashTrigger washTrigger;
    
    [Header("UI Components")]
    [Tooltip("Canvas GameObject for the wash mini-game UI")]
    public GameObject washCanvas;
    
    [Tooltip("Progress bar showing wash progress")]
    public Slider progressBar;
    
    [Tooltip("Text showing remaining stains")]
    public TextMeshProUGUI stainCountText;
    
    [Header("Camera Positions (Optional - for fixed camera views)")]
    [Tooltip("Camera position during wash (optional)")]
    public Transform washCameraPosition;
    
    [Tooltip("Camera position to return to after wash (optional)")]
    public Transform originalCameraPosition;
    
    [Header("Audio Sources")]
    [Tooltip("Audio source for wash sounds")]
    public AudioSource washAudioSource;
    
    [Tooltip("Audio source for water gun sounds")]
    public AudioSource waterGunAudioSource;
    
    [Header("Audio Clips")]
    [Tooltip("Sound when wash starts")]
    public AudioClip washStartSound;
    
    [Tooltip("Sound when wash completes")]
    public AudioClip washCompleteSound;
    
    [Tooltip("Sound when water gun sprays")]
    public AudioClip spraySound;
    
    [Tooltip("Sound when stain is cleaned")]
    public AudioClip stainCleanSound;
    
    [Header("Effects")]
    [Tooltip("Splash effect prefab")]
    public GameObject splashPrefab;
    
    [Tooltip("Stain damage effect prefab")]
    public GameObject stainDamageEffect;

    void Start()
    {
        SetupIntegration();
    }

    /// <summary>
    /// Set up all the connections between components
    /// </summary>
    private void SetupIntegration()
    {
        if (elephantWashManager == null)
        {
            Debug.LogError("ElephantWashIntegrationExample: No ElephantWashManager assigned!");
            return;
        }

        // Set up ElephantWashManager connections
        elephantWashManager.elephantObject = elephantObject;
        elephantWashManager.washCanvas = washCanvas;
        elephantWashManager.progressBar = progressBar;
        elephantWashManager.stainCountText = stainCountText;
        elephantWashManager.washCameraPosition = washCameraPosition;
        elephantWashManager.originalCameraPosition = originalCameraPosition;
        elephantWashManager.playerController = FindObjectOfType<FirstPersonController>();
        elephantWashManager.waterGun = waterGun;
        elephantWashManager.washAudioSource = washAudioSource;
        elephantWashManager.washStartSound = washStartSound;
        elephantWashManager.washCompleteSound = washCompleteSound;

        // Set up WaterGunController
        if (waterGun != null)
        {
            WaterGunController waterGunController = waterGun.GetComponent<WaterGunController>();
            if (waterGunController == null)
            {
                waterGunController = waterGun.AddComponent<WaterGunController>();
            }
            
            elephantWashManager.waterGunController = waterGunController;
            
                    // Set up water gun components
        // The waterGun GameObject should be the parent containing all particle systems
        waterGunController.waterGunParent = waterGun;
        elephantWashManager.waterGunController = waterGunController;
        
        // Optional: Assign a specific particle system for collision detection
        ParticleSystem waterSpray = waterGun.GetComponentInChildren<ParticleSystem>();
        if (waterSpray != null)
        {
            waterGunController.waterSpray = waterSpray;
        }
            waterGunController.splashPrefab = splashPrefab;
            waterGunController.waterGunAudio = waterGunAudioSource;
            waterGunController.spraySound = spraySound;
            waterGunController.stainLayerMask = LayerMask.GetMask("Stain");
        }

        // Set up wash trigger
        if (washTrigger != null)
        {
            washTrigger.washManager = elephantWashManager;
        }

        // Note: FirstPersonController no longer has washManager field
        // The wash mini-game is now triggered only through event objects

        // Subscribe to events
        if (elephantWashManager != null)
        {
            elephantWashManager.OnWashStarted += OnWashStarted;
            elephantWashManager.OnWashCompleted += OnWashCompleted;
            elephantWashManager.OnStainCleaned += OnStainCleaned;
        }

        Debug.Log("ElephantWashIntegrationExample: Integration setup complete!");
    }

    /// <summary>
    /// Set up a basic particle system for water spray
    /// </summary>
    private void SetupParticleSystem(ParticleSystem ps)
    {
        var main = ps.main;
        main.duration = 0f; // Continuous
        main.startLifetime = 1.5f;
        main.startSpeed = 15f;
        main.startSize = 0.2f;
        main.startColor = Color.cyan;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 150;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 20f;
        shape.radius = 0.1f;

        var collision = ps.collision;
        collision.enabled = true;
        collision.type = ParticleSystemCollisionType.World;
        collision.sendCollisionMessages = true;
        collision.maxCollisionShapes = 256;
        collision.quality = ParticleSystemCollisionQuality.High;
        collision.collidesWith = LayerMask.GetMask("Stain");
    }

    /// <summary>
    /// Called when wash starts
    /// </summary>
    private void OnWashStarted()
    {
        Debug.Log("ElephantWashIntegrationExample: Wash started!");
        
        // You can add additional logic here
        // For example, show a UI message, play music, etc.
    }

    /// <summary>
    /// Called when wash completes
    /// </summary>
    private void OnWashCompleted()
    {
        Debug.Log("ElephantWashIntegrationExample: Wash completed!");
        
        // You can add additional logic here
        // For example, show a completion message, give rewards, etc.
    }

    /// <summary>
    /// Called when a stain is cleaned
    /// </summary>
    private void OnStainCleaned(int remainingStains)
    {
        Debug.Log($"ElephantWashIntegrationExample: Stain cleaned! {remainingStains} remaining");
        
        // You can add additional logic here
        // For example, play a sound, show particle effects, etc.
    }

    /// <summary>
    /// Test method to start the wash mini-game
    /// </summary>
    [ContextMenu("Test Start Wash")]
    public void TestStartWash()
    {
        if (elephantWashManager != null)
        {
            elephantWashManager.StartWash();
        }
    }

    /// <summary>
    /// Test method to force end the wash mini-game
    /// </summary>
    [ContextMenu("Test End Wash")]
    public void TestEndWash()
    {
        if (elephantWashManager != null)
        {
            elephantWashManager.EndWash();
        }
    }

    /// <summary>
    /// Validate all required components are assigned
    /// </summary>
    [ContextMenu("Validate Setup")]
    public void ValidateSetup()
    {
        bool isValid = true;
        
        if (elephantWashManager == null)
        {
            Debug.LogError("ElephantWashIntegrationExample: ElephantWashManager not assigned!");
            isValid = false;
        }
        
        if (waterGun == null)
        {
            Debug.LogError("ElephantWashIntegrationExample: WaterGun not assigned!");
            isValid = false;
        }
        
        if (washCanvas == null)
        {
            Debug.LogError("ElephantWashIntegrationExample: WashCanvas not assigned!");
            isValid = false;
        }
        
        if (progressBar == null)
        {
            Debug.LogError("ElephantWashIntegrationExample: ProgressBar not assigned!");
            isValid = false;
        }
        
        if (stainCountText == null)
        {
            Debug.LogError("ElephantWashIntegrationExample: StainCountText not assigned!");
            isValid = false;
        }
        
        if (washCameraPosition == null)
        {
            Debug.LogError("ElephantWashIntegrationExample: WashCameraPosition not assigned!");
            isValid = false;
        }
        
        if (originalCameraPosition == null)
        {
            Debug.LogError("ElephantWashIntegrationExample: OriginalCameraPosition not assigned!");
            isValid = false;
        }
        
        if (isValid)
        {
            Debug.Log("ElephantWashIntegrationExample: All components validated successfully!");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (elephantWashManager != null)
        {
            elephantWashManager.OnWashStarted -= OnWashStarted;
            elephantWashManager.OnWashCompleted -= OnWashCompleted;
            elephantWashManager.OnStainCleaned -= OnStainCleaned;
        }
    }
} 