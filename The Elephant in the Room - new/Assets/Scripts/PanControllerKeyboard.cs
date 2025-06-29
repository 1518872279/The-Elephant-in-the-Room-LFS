using UnityEngine;

public class PanControllerKeyboard : MonoBehaviour
{
    public Transform panTransform;
    [Tooltip("Maximum lean angle in degrees")]
    public float maxAngle = 30f;
    [Tooltip("Rotation speed in degrees per second")]
    public float rotateSpeed = 90f;

    void Update()
    {
        // Read keyboard input
        float forward = Input.GetKey(KeyCode.W) ? 1f : (Input.GetKey(KeyCode.S) ? -1f : 0f);
        float right   = Input.GetKey(KeyCode.D) ? 1f : (Input.GetKey(KeyCode.A) ? -1f : 0f);

        // Compute target lean angles
        float xAngle = Mathf.Clamp(-forward * maxAngle, -maxAngle, maxAngle); // forward/back tilt
        float zAngle = Mathf.Clamp(right   * maxAngle, -maxAngle, maxAngle); // left/right tilt

        // Smoothly rotate towards target
        Quaternion targetRot = Quaternion.Euler(xAngle, 90f, zAngle);
        panTransform.localRotation = Quaternion.RotateTowards(
            panTransform.localRotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );
    }
} 