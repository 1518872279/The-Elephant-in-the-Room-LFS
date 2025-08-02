using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float lookSpeed = 2f;
    public Transform cameraTransform;
    public float interactDistance = 3f;
    public LayerMask interactLayer;
    
    [Header("Watch Control")]
    [Tooltip("Reference to the WatchAnimationManager component")]
    public WatchAnimationManager watchManager;
    
    [Header("Elephant Teaser Control")]
    [Tooltip("Reference to the ElephantTeaserAnimationManager component")]
    public ElephantTeaserAnimationManager teaserManager;
    
    [Header("Stair Climbing")]
    public float maxStepHeight = 0.3f;
    public float stepSmooth = 0.1f;
    public LayerMask groundLayer = -1;

    private CharacterController controller;
    private float pitch;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleLook();
        HandleMove();
        if (Input.GetMouseButtonDown(0)) 
        {
            HandleInteract();
            HandleTeaserMouseInput();
        }
        HandleWatchInput();
        HandleTeaserInput();
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
        // Check if grounded
        isGrounded = controller.isGrounded;
        
        // Get input
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 move = (transform.right * input.x + transform.forward * input.z) * walkSpeed;
        
        // Apply gravity
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force when grounded
        }
        else
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }
        
        // Add vertical velocity to movement
        move.y = velocity.y;
        
        // Try to move
        Vector3 originalMove = move;
        controller.Move(move * Time.deltaTime);
        
        // If we hit something and we're trying to move forward, try to step up
        if (!isGrounded && input.magnitude > 0.1f)
        {
            TryStepUp(originalMove);
        }
    }

    void TryStepUp(Vector3 moveDirection)
    {
        // Cast a ray forward to detect obstacles
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        Vector3 rayDirection = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;
        
        if (Physics.Raycast(rayStart, rayDirection, out RaycastHit hit, 0.5f, groundLayer))
        {
            // Check if the obstacle is within step height
            if (hit.distance < 0.5f && hit.point.y - transform.position.y <= maxStepHeight)
            {
                // Try to step up
                Vector3 stepUpPosition = transform.position + Vector3.up * maxStepHeight;
                
                // Check if there's space above the step
                if (!Physics.CheckCapsule(stepUpPosition, stepUpPosition + Vector3.up * (controller.height - maxStepHeight), 
                    controller.radius, groundLayer))
                {
                    // Move up the step
                    transform.position = Vector3.Lerp(transform.position, 
                        new Vector3(transform.position.x, hit.point.y + 0.1f, transform.position.z), 
                        stepSmooth);
                }
            }
        }
    }

    void HandleInteract()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
            hit.collider.GetComponent<IInteractable>()?.Interact();
    }
    
    void HandleWatchInput()
    {
        // Check for watch input (key 3)
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (watchManager != null)
            {
                watchManager.HandleWatchInput();
            }
            else
            {
                Debug.LogWarning("WatchManager not assigned to FirstPersonController");
            }
        }
    }
    
    void HandleTeaserInput()
    {
        // Check for elephant teaser input (key 4)
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (teaserManager != null)
            {
                teaserManager.HandleTeaserInput();
            }
            else
            {
                Debug.LogWarning("TeaserManager not assigned to FirstPersonController");
            }
        }
    }
    
    void HandleTeaserMouseInput()
    {
        // Check for elephant teaser mouse input (left mouse button)
        if (teaserManager != null)
        {
            teaserManager.HandleTeaserMouseInput();
        }
    }
} 