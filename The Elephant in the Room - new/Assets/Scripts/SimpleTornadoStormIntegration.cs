using UnityEngine;

public class SimpleTornadoStormIntegration : MonoBehaviour
{
    [Header("Tornado Effect")]
    public SimpleTornadoEffect tornadoEffect;
    
    [Header("Storm Integration")]
    public StormController stormController;
    
    [Header("Activation Settings")]
    public bool autoActivateWithStorm = true;
    public float activationDelay = 3f;
    public float deactivationDelay = 1f;
    
    private bool tornadoActive = false;
    
    void Start()
    {
        if (tornadoEffect == null)
            tornadoEffect = GetComponent<SimpleTornadoEffect>();
            
        if (stormController == null)
            stormController = StormController.Instance;
    }
    
    void Update()
    {
        if (autoActivateWithStorm && stormController != null)
        {
            HandleStormIntegration();
        }
    }
    
    void HandleStormIntegration()
    {
        if (stormController.IsStormActive && !tornadoActive)
        {
            // Storm is active, start tornado after delay
            Invoke(nameof(ActivateTornado), activationDelay);
            tornadoActive = true;
        }
        else if (!stormController.IsStormActive && tornadoActive)
        {
            // Storm ended, deactivate tornado after delay
            Invoke(nameof(DeactivateTornado), deactivationDelay);
            tornadoActive = false;
        }
    }
    
    public void ActivateTornado()
    {
        if (tornadoEffect != null)
        {
            tornadoEffect.StartSpinning();
            Debug.Log("Decorative tornado activated!");
        }
    }
    
    public void DeactivateTornado()
    {
        if (tornadoEffect != null)
        {
            tornadoEffect.StopSpinning();
            Debug.Log("Decorative tornado deactivated!");
        }
    }
    
    public void ToggleTornado()
    {
        if (tornadoEffect != null)
        {
            tornadoEffect.ToggleSpinning();
        }
    }
    
    // Public properties
    public bool IsTornadoActive => tornadoEffect != null && tornadoEffect.IsSpinning;
} 