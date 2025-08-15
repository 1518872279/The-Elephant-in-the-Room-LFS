using UnityEngine;

public class TornadoStormIntegration : MonoBehaviour
{
    [Header("Tornado Integration")]
    public TornadoController tornadoController;
    public bool tornadoActive = false;
    
    [Header("Storm Integration")]
    public StormController stormController;
    
    [Header("Tornado Activation Settings")]
    public float tornadoActivationDelay = 5f;       // Delay after storm starts before tornado activates
    public float tornadoDeactivationDelay = 2f;     // Delay after storm ends before tornado deactivates
    public bool autoActivateWithStorm = true;       // Automatically activate tornado with storm
    
    [Header("Tornado Intensity")]
    public float minRadius = 5f;                    // Minimum tornado radius
    public float maxRadius = 15f;                   // Maximum tornado radius
    public float minForce = 30f;                    // Minimum force
    public float maxForce = 80f;                    // Maximum force
    public float intensityRampTime = 10f;           // Time to reach full intensity
    
    private float tornadoStartTime;
    private bool isRampingUp = false;
    private bool isRampingDown = false;
    
    void Start()
    {
        if (tornadoController == null)
            tornadoController = GetComponent<TornadoController>();
            
        if (stormController == null)
            stormController = StormController.Instance;
            
        // Subscribe to storm events if available
        if (stormController != null)
        {
            // You can add event listeners here if StormController has events
        }
    }
    
    void Update()
    {
        if (autoActivateWithStorm && stormController != null)
        {
            HandleStormIntegration();
        }
        
        if (tornadoActive)
        {
            UpdateTornadoIntensity();
        }
    }
    
    void HandleStormIntegration()
    {
        if (stormController.IsStormActive && !tornadoActive && !isRampingUp)
        {
            // Storm is active, start tornado after delay
            Invoke(nameof(ActivateTornado), tornadoActivationDelay);
            isRampingUp = true;
        }
        else if (!stormController.IsStormActive && tornadoActive && !isRampingDown)
        {
            // Storm ended, deactivate tornado after delay
            Invoke(nameof(DeactivateTornado), tornadoDeactivationDelay);
            isRampingDown = true;
        }
    }
    
    void UpdateTornadoIntensity()
    {
        if (!isRampingUp && !isRampingDown) return;
        
        float elapsedTime = Time.time - tornadoStartTime;
        float intensity = 0f;
        
        if (isRampingUp)
        {
            intensity = Mathf.Clamp01(elapsedTime / intensityRampTime);
            if (intensity >= 1f)
            {
                isRampingUp = false;
                intensity = 1f;
            }
        }
        else if (isRampingDown)
        {
            intensity = Mathf.Clamp01(1f - (elapsedTime / intensityRampTime));
            if (intensity <= 0f)
            {
                isRampingDown = false;
                intensity = 0f;
            }
        }
        
        // Apply intensity to tornado parameters
        if (tornadoController != null)
        {
            tornadoController.SetRadius(Mathf.Lerp(minRadius, maxRadius, intensity));
            tornadoController.SetForce(Mathf.Lerp(minForce, maxForce, intensity));
        }
    }
    
    public void ActivateTornado()
    {
        if (tornadoActive) return;
        
        tornadoActive = true;
        tornadoStartTime = Time.time;
        isRampingUp = true;
        isRampingDown = false;
        
        if (tornadoController != null)
        {
            tornadoController.enabled = true;
            // Start with minimum values
            tornadoController.SetRadius(minRadius);
            tornadoController.SetForce(minForce);
        }
        
        Debug.Log("Tornado activated!");
    }
    
    public void DeactivateTornado()
    {
        if (!tornadoActive) return;
        
        tornadoActive = false;
        tornadoStartTime = Time.time;
        isRampingUp = false;
        isRampingDown = true;
        
        Debug.Log("Tornado deactivating...");
        
        // The actual deactivation will happen after ramp down
        Invoke(nameof(DisableTornadoController), intensityRampTime);
    }
    
    void DisableTornadoController()
    {
        if (tornadoController != null)
        {
            tornadoController.enabled = false;
            tornadoController.ClearAllObjects();
        }
        
        isRampingDown = false;
        Debug.Log("Tornado deactivated!");
    }
    
    public void ToggleTornado()
    {
        if (tornadoActive)
            DeactivateTornado();
        else
            ActivateTornado();
    }
    
    // Public methods for external control
    public bool IsTornadoActive => tornadoActive;
    
    public float GetTornadoIntensity()
    {
        if (!tornadoActive) return 0f;
        
        float elapsedTime = Time.time - tornadoStartTime;
        if (isRampingUp)
            return Mathf.Clamp01(elapsedTime / intensityRampTime);
        else if (isRampingDown)
            return Mathf.Clamp01(1f - (elapsedTime / intensityRampTime));
        else
            return 1f;
    }
    
    void OnDrawGizmos()
    {
        if (tornadoController != null && tornadoActive)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, tornadoController.radius);
        }
    }
} 