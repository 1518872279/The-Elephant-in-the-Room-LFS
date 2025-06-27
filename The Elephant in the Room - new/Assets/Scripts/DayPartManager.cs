using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DayPartManager : MonoBehaviour
{
    public Volume morningVolume;
    public Volume eveningVolume;
    public Light directionalLight;

    [Header("Lighting Intensities")]
    public float morningIntensity = 1f;
    public float eveningIntensity = 0.5f;

    private enum DayPart { None, Morning, Evening }
    private DayPart currentPart = DayPart.None;

    void Start()
    {
        TimeManager.Instance.OnTimeChanged += OnTimeChanged;
        OnTimeChanged(TimeManager.Instance.GetCurrentTime());
    }

    private void OnTimeChanged(int minutes)
    {
        DayPart newPart = DeterminePart(minutes);
        if (newPart != currentPart)
        {
            ApplyPart(newPart);
            currentPart = newPart;
        }
    }

    private DayPart DeterminePart(int minutes)
        => minutes >= TimeManager.Instance.morningStart && minutes < TimeManager.Instance.morningEnd ? DayPart.Morning
         : minutes >= TimeManager.Instance.eveningStart && minutes < TimeManager.Instance.eveningEnd ? DayPart.Evening
         : DayPart.None;

    private void ApplyPart(DayPart part)
    {
        morningVolume.weight = part == DayPart.Morning ? 1f : 0f;
        eveningVolume.weight = part == DayPart.Evening ? 1f : 0f;
        if (part == DayPart.Morning)
            directionalLight.intensity = morningIntensity;
        else if (part == DayPart.Evening)
            directionalLight.intensity = eveningIntensity;
    }
} 