using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float lookSpeed = 2f;
    public Transform cameraTransform;
    public float interactDistance = 3f;
    public LayerMask interactLayer;

    private CharacterController controller;
    private float pitch;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleLook();
        HandleMove();
        if (Input.GetMouseButtonDown(0)) HandleInteract();
    }

    void HandleLook()
    {
        float yaw = Input.GetAxis("Mouse X") * lookSpeed;
        float pitchDelta = -Input.GetAxis("Mouse Y") * lookSpeed;
        transform.Rotate(Vector3.up, yaw);
        pitch = Mathf.Clamp(pitch + pitchDelta, -80f, 80f);
        cameraTransform.localEulerAngles = Vector3.right * pitch;
    }

    void HandleMove()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 move = (transform.right * input.x + transform.forward * input.z) * walkSpeed;
        controller.Move(move * Time.deltaTime);
    }

    void HandleInteract()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
            hit.collider.GetComponent<IInteractable>()?.Interact();
    }
} 