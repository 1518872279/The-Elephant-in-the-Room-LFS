using System.Collections;
using UnityEngine;

[System.Serializable]
public class FlickerPattern
{
    [Tooltip("Name of this flicker pattern")]
    public string patternName = "Default";
    
    [Tooltip("Base intensity of the light")]
    [Range(0f, 2f)]
    public float baseIntensity = 0.8f;
    
    [Tooltip("Maximum flicker intensity variation")]
    [Range(0f, 1f)]
    public float flickerIntensity = 0.3f;
    
    [Tooltip("Speed of the flicker (higher = faster)")]
    [Range(0.1f, 10f)]
    public float flickerSpeed = 2f;
    
    [Tooltip("Randomness in the flicker timing")]
    [Range(0f, 1f)]
    public float flickerRandomness = 0.2f;
    
    [Tooltip("Occasional power fluctuations (like old TVs)")]
    [Range(0f, 1f)]
    public float powerFluctuationChance = 0.1f;
    
    [Tooltip("Duration of power fluctuations")]
    [Range(0.1f, 2f)]
    public float powerFluctuationDuration = 0.5f;
    

}

public class TVFlickerLight : MonoBehaviour
{
    [Header("TV Light Settings")]
    [Tooltip("The light component that will flicker")]
    public Light tvLight;
    
    [Tooltip("The TV screen material (optional, for screen glow effect)")]
    public Material tvScreenMaterial;
    
    [Tooltip("Emission property name in the TV screen material")]
    public string emissionPropertyName = "_EmissionColor";
    
    [Header("Flicker Patterns")]
    [Tooltip("Available flicker patterns")]
    public FlickerPattern[] flickerPatterns;
    
    [Tooltip("Current flicker pattern index")]
    public int currentPatternIndex = 0;
    
    [Header("Simple Controls")]
    [Tooltip("Enable/disable the flicker effect")]
    public bool flickerEnabled = true;
    
    [Tooltip("Smooth transition when turning off")]
    public bool smoothTransitions = true;
    
    [Tooltip("Transition duration in seconds")]
    [Range(0.1f, 3f)]
    public float transitionDuration = 1f;
    
    [Header("Debug")]
    [Tooltip("Show debug information in console")]
    public bool debugMode = false;
    
    // Private variables
    private FlickerPattern currentPattern;
    private float originalIntensity;
    private Color originalEmissionColor;
    private bool isFlickering = false;
    private Coroutine flickerCoroutine;
    private Coroutine transitionCoroutine;
    
    // Animation curve for smooth flickering
    private AnimationCurve flickerCurve;
    
    void Start()
    {
        InitializeTVLight();
        SetupFlickerCurve();
        
        if (flickerPatterns.Length > 0)
        {
            currentPattern = flickerPatterns[currentPatternIndex];
        }
        else
        {
            // Create default pattern if none provided
            currentPattern = new FlickerPattern();
        }
        
        // Start flickering if enabled
        if (flickerEnabled)
        {
            StartFlickering();
        }
    }
    
    void InitializeTVLight()
    {
        // If no light assigned, try to find one on this GameObject
        if (tvLight == null)
        {
            tvLight = GetComponent<Light>();
        }
        
        // If still no light, create one
        if (tvLight == null)
        {
            tvLight = gameObject.AddComponent<Light>();
            tvLight.type = LightType.Point;
            tvLight.range = 3f;
            tvLight.intensity = 0f;
            tvLight.color = Color.white;
        }
        
        // Store original values
        originalIntensity = tvLight.intensity;
        
        // Initialize TV screen material
        if (tvScreenMaterial != null)
        {
            originalEmissionColor = tvScreenMaterial.GetColor(emissionPropertyName);
        }
        
        if (debugMode)
        {
            Debug.Log($"TVFlickerLight: Initialized on {gameObject.name}");
        }
    }
    
    void SetupFlickerCurve()
    {
        // Create a smooth curve for natural flickering
        flickerCurve = new AnimationCurve();
        flickerCurve.AddKey(0f, 0.8f);
        flickerCurve.AddKey(0.2f, 1.0f);
        flickerCurve.AddKey(0.4f, 0.9f);
        flickerCurve.AddKey(0.6f, 1.1f);
        flickerCurve.AddKey(0.8f, 0.95f);
        flickerCurve.AddKey(1.0f, 1.0f);
    }
    
    public void StartFlickering()
    {
        if (isFlickering) return;
        
        isFlickering = true;
        flickerCoroutine = StartCoroutine(FlickerRoutine());
        
        if (debugMode)
        {
            Debug.Log($"TVFlickerLight: Started flickering on {gameObject.name}");
        }
    }
    
    public void StopFlickering()
    {
        if (!isFlickering) return;
        
        isFlickering = false;
        
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }
        
        // Smoothly return to original state
        if (smoothTransitions)
        {
            transitionCoroutine = StartCoroutine(TransitionToOriginal());
        }
        else
        {
                    // Instant return to original state
        tvLight.intensity = 0f;
            
            if (tvScreenMaterial != null)
            {
                tvScreenMaterial.SetColor(emissionPropertyName, Color.black);
            }
        }
        
        if (debugMode)
        {
            Debug.Log($"TVFlickerLight: Stopped flickering on {gameObject.name}");
        }
    }
    
    IEnumerator FlickerRoutine()
    {
        while (isFlickering)
        {
            // Calculate base flicker
            float time = Time.time * currentPattern.flickerSpeed;
            float flickerValue = flickerCurve.Evaluate(time % 1f);
            
            // Add randomness
            float randomFactor = 1f + Random.Range(-currentPattern.flickerRandomness, currentPattern.flickerRandomness);
            
            // Calculate final intensity
            float targetIntensity = currentPattern.baseIntensity * flickerValue * randomFactor;
            
            // Check for power fluctuations
            if (Random.Range(0f, 1f) < currentPattern.powerFluctuationChance)
            {
                yield return StartCoroutine(PowerFluctuation());
            }
            
            // Apply intensity
            tvLight.intensity = targetIntensity;
            
            // Update TV screen emission
            if (tvScreenMaterial != null)
            {
                Color emissionColor = tvLight.color * targetIntensity * 0.5f;
                tvScreenMaterial.SetColor(emissionPropertyName, emissionColor);
            }
            
            // Wait for next frame
            yield return null;
        }
    }
    
    IEnumerator PowerFluctuation()
    {
        float originalIntensity = tvLight.intensity;
        float fluctuationStart = Time.time;
        
        while (Time.time - fluctuationStart < currentPattern.powerFluctuationDuration)
        {
            // Create a power fluctuation effect
            float fluctuation = Mathf.Sin((Time.time - fluctuationStart) * 20f) * 0.3f + 0.7f;
            tvLight.intensity = originalIntensity * fluctuation;
            
            yield return null;
        }
    }
    
    IEnumerator TransitionToOriginal()
    {
        float startTime = Time.time;
        float startIntensity = tvLight.intensity;
        Color startEmission = tvScreenMaterial != null ? tvScreenMaterial.GetColor(emissionPropertyName) : Color.black;
        
        while (Time.time - startTime < transitionDuration)
        {
            float t = (Time.time - startTime) / transitionDuration;
            t = Mathf.SmoothStep(0f, 1f, t);
            
            tvLight.intensity = Mathf.Lerp(startIntensity, 0f, t);
            
            if (tvScreenMaterial != null)
            {
                Color targetEmission = Color.Lerp(startEmission, Color.black, t);
                tvScreenMaterial.SetColor(emissionPropertyName, targetEmission);
            }
            
            yield return null;
        }
        
        // Ensure we end at exactly the original values
        tvLight.intensity = 0f;
        
        if (tvScreenMaterial != null)
        {
            tvScreenMaterial.SetColor(emissionPropertyName, Color.black);
        }
    }
    

    
    // Public methods for external control
    public void SetFlickerPattern(int patternIndex)
    {
        if (patternIndex >= 0 && patternIndex < flickerPatterns.Length)
        {
            currentPatternIndex = patternIndex;
            currentPattern = flickerPatterns[patternIndex];
        }
    }
    
    public void ToggleFlicker()
    {
        flickerEnabled = !flickerEnabled;
        
        if (flickerEnabled)
        {
            StartFlickering();
        }
        else
        {
            StopFlickering();
        }
    }
    
    void OnDestroy()
    {
        // Clean up when destroyed
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
        }
        
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        
        // Reset TV screen material
        if (tvScreenMaterial != null)
        {
            tvScreenMaterial.SetColor(emissionPropertyName, originalEmissionColor);
        }
    }
    
    // Editor helper methods
    [ContextMenu("Create Default Patterns")]
    void CreateDefaultPatterns()
    {
        flickerPatterns = new FlickerPattern[]
        {
            new FlickerPattern
            {
                patternName = "Modern TV",
                baseIntensity = 0.6f,
                flickerIntensity = 0.1f,
                flickerSpeed = 1.5f,
                flickerRandomness = 0.1f,
                powerFluctuationChance = 0.05f
            },
            new FlickerPattern
            {
                patternName = "Old CRT TV",
                baseIntensity = 0.8f,
                flickerIntensity = 0.4f,
                flickerSpeed = 3f,
                flickerRandomness = 0.3f,
                powerFluctuationChance = 0.2f,
                powerFluctuationDuration = 0.8f
            },
            new FlickerPattern
            {
                patternName = "Faulty TV",
                baseIntensity = 0.7f,
                flickerIntensity = 0.6f,
                flickerSpeed = 5f,
                flickerRandomness = 0.5f,
                powerFluctuationChance = 0.4f,
                powerFluctuationDuration = 1.2f
            }
        };
    }
} 