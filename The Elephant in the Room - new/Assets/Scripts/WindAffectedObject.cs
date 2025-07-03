using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class WindAffectedObject : MonoBehaviour
{
    [Header("Wind Response")]
    public float windSensitivity = 1f;
    public float maxBendAngle = 30f;
    public float windFrequency = 1f;
    
    [Header("Material Properties")]
    public string windDirectionProperty = "_WindDirection";
    public string windStrengthProperty = "_WindStrength";
    public string windFrequencyProperty = "_WindFrequency";
    public string windAmplitudeProperty = "_WindAmplitude";

    private Renderer rend;
    private Material[] materials;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        rend = GetComponent<Renderer>();
        materials = rend.materials;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        // Get wind zone if available
        WindZone windZone = FindObjectOfType<WindZone>();
        if (windZone != null)
        {
            ApplyWindEffect(windZone);
        }
    }

    private void ApplyWindEffect(WindZone windZone)
    {
        // Calculate wind direction and strength
        Vector3 windDirection = windZone.transform.forward;
        float windStrength = windZone.windMain * windSensitivity;
        
        // Apply wind effect to materials
        foreach (Material mat in materials)
        {
            if (mat.HasProperty(windDirectionProperty))
            {
                mat.SetVector(windDirectionProperty, new Vector4(windDirection.x, windDirection.y, windDirection.z, windZone.windPulseFrequency));
            }
            
            if (mat.HasProperty(windStrengthProperty))
            {
                mat.SetFloat(windStrengthProperty, windStrength);
            }
            
            if (mat.HasProperty(windFrequencyProperty))
            {
                mat.SetFloat(windFrequencyProperty, windFrequency);
            }
            
            if (mat.HasProperty(windAmplitudeProperty))
            {
                mat.SetFloat(windAmplitudeProperty, windZone.windPulseMagnitude);
            }
        }

        // Apply physical wind effect (bending)
        float windPhase = Time.time * windZone.windPulseFrequency;
        float windBend = Mathf.Sin(windPhase) * windZone.windPulseMagnitude * windStrength;
        
        // Calculate bend direction based on wind
        Vector3 bendAxis = Vector3.Cross(windDirection, Vector3.up).normalized;
        float bendAngle = windBend * maxBendAngle;
        
        // Apply rotation
        transform.rotation = originalRotation * Quaternion.AngleAxis(bendAngle, bendAxis);
    }

    // Public method to set wind sensitivity at runtime
    public void SetWindSensitivity(float sensitivity)
    {
        windSensitivity = sensitivity;
    }

    // Public method to reset object to original state
    public void ResetToOriginal()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
} 