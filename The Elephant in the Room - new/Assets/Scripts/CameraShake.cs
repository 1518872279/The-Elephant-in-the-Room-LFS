using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public float strength = 0.1f;
    public float duration = 0.2f;

    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        Vector3 origin = transform.localPosition;
        float t = duration;
        while (t > 0)
        {
            transform.localPosition = origin + Random.insideUnitSphere * strength;
            t -= Time.deltaTime;
            yield return null;
        }
        transform.localPosition = origin;
    }
} 