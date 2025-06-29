using UnityEngine;

public class PanController : MonoBehaviour
{
    public Transform panTransform;
    public Rigidbody panRigidbody;
    public float shakeSensitivity = 5f;
    [HideInInspector] public bool FinishedSpawning;

    private Vector3 prevMousePos;

    public void ResetPan()
    {
        FinishedSpawning = false;
        prevMousePos = Input.mousePosition;
        panRigidbody.velocity = Vector3.zero;
        panTransform.localRotation = Quaternion.identity;
    }

    public void RotateWithMouse()
    {
        Vector3 delta = Input.mousePosition - prevMousePos;
        panRigidbody.AddTorque(new Vector3(-delta.y, 0f, delta.x) * shakeSensitivity, ForceMode.Acceleration);
        prevMousePos = Input.mousePosition;
    }
} 