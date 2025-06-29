using UnityEngine;

public class PanControllerDirect : MonoBehaviour
{
    public Transform panTransform;
    public float rotationSpeed = 50f;
    public float maxTiltAngle = 30f;
    public float smoothSpeed = 5f;

    private Vector2 targetAngles;
    private Vector2 currentAngles;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // Accumulate target angles from mouse movement
            targetAngles.x += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            targetAngles.y -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            // Clamp tilt
            targetAngles.y = Mathf.Clamp(targetAngles.y, -maxTiltAngle, maxTiltAngle);
        }

        // Smoothly interpolate current angles towards target
        currentAngles = Vector2.Lerp(currentAngles, targetAngles, smoothSpeed * Time.deltaTime);
        // Apply rotation (pitch and roll)
        panTransform.localRotation = Quaternion.Euler(currentAngles.y, 0f, currentAngles.x);
    }
} 