using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LightningController : MonoBehaviour
{
    public Light mainLight;
    public Image flashImage; // full-screen white Image
    public float minInterval = 5f;
    public float maxInterval = 20f;
    public float flashIntensity = 5f;
    public float flashDuration = 0.1f;
    private float baseIntensity;

    void Start()
    {
        baseIntensity = mainLight.intensity;
        flashImage.color = Color.clear;
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            // Flash light
            mainLight.intensity = baseIntensity * flashIntensity;
            flashImage.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
            mainLight.intensity = baseIntensity;
            // Fade overlay
            float t = 1f;
            while (t > 0f)
            {
                flashImage.color = new Color(1, 1, 1, t);
                t -= Time.deltaTime / flashDuration;
                yield return null;
            }
        }
    }
} 