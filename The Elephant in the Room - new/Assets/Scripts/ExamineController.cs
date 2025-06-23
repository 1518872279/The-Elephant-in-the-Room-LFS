using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ExamineController : MonoBehaviour
{
    public float examineDistance = 3f;
    public LayerMask examinableLayer;
    public Transform holdParent;
    public float rotationSpeed = 5f;

    private Camera cam;
    private GameObject currentObject;
    private bool isExamining;
    private FirstPersonController fpController;

    void Start()
    {
        cam = GetComponent<Camera>();
        //fpController = GetComponent<FirstPersonController>();
        fpController = FindAnyObjectByType<FirstPersonController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isExamining)
        {
            TryPickUp();
        }
        else if (Input.GetMouseButton(0) && isExamining)
        {
            RotateObject();
        }
        else if (Input.GetMouseButtonUp(0) && isExamining)
        {
            Drop();
        }
    }

    void TryPickUp()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, examineDistance, examinableLayer))
        {
            currentObject = hit.collider.gameObject;
            var rb = currentObject.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = true;
            currentObject.transform.SetParent(holdParent);
            currentObject.transform.localPosition = Vector3.zero;
            currentObject.transform.localRotation = Quaternion.identity;
            isExamining = true;
            if (fpController) fpController.enabled = false;
        }
    }

    void RotateObject()
    {
        float rotX = Input.GetAxis("Mouse X") * rotationSpeed;
        float rotY = Input.GetAxis("Mouse Y") * rotationSpeed;
        currentObject.transform.Rotate(cam.transform.up, -rotX, Space.World);
        currentObject.transform.Rotate(cam.transform.right, rotY, Space.World);
    }

    void Drop()
    {
        if (currentObject == null) return;
        var rb = currentObject.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;
        currentObject.transform.SetParent(null);
        currentObject = null;
        isExamining = false;
        if (fpController) fpController.enabled = true;
    }
} 