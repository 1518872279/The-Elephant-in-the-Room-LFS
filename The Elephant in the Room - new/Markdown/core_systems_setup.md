# Core Systems Setup

This document summarizes the foundational systems for your Unity project and provides the complete scripts for Cursor to ingest.

---

## 1. First-Person Controller

**Overview:** Handles player movement, camera look, and object interaction.

**Setup Steps:**

1. Create a **Player** GameObject.
2. Add a **CharacterController** component.
3. Add a **Camera** as a child and assign to `cameraTransform`.
4. Define an “Interactable” layer for clickable objects.

```csharp
// FirstPersonController.cs
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
```

```csharp
// IInteractable.cs
public interface IInteractable
{
    void Interact();
}
```

---

## 2. Inventory System + UI

**Overview:** Manages item data, storage, and a toggleable inventory panel.

**Scripts & Setup:**

```csharp
// Item.cs
using UnityEngine;
[CreateAssetMenu(menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
}
```

```csharp
// Inventory.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public event Action OnChanged;
    private List<Item> items = new List<Item>();
    public IReadOnlyList<Item> Items => items;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Add(Item item)
    {
        items.Add(item);
        OnChanged?.Invoke();
    }

    public void Remove(Item item)
    {
        if (items.Remove(item)) OnChanged?.Invoke();
    }
}
```

```csharp
// InventoryUI.cs
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject panel;
    public Transform itemsParent;
    public GameObject slotPrefab;

    void Start()
    {
        Inventory.Instance.OnChanged += RefreshUI;
        panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            panel.SetActive(!panel.activeSelf);
    }

    void RefreshUI()
    {
        foreach (Transform t in itemsParent) Destroy(t.gameObject);
        foreach (var item in Inventory.Instance.Items)
        {
            var slot = Instantiate(slotPrefab, itemsParent);
            slot.GetComponent<Image>().sprite = item.icon;
        }
    }
}
```

---

## 3. Phone UI Panel

**Overview:** Toggleable panel for in-game phone interface.

```csharp
// PhoneUIController.cs
using UnityEngine;

public class PhoneUIController : MonoBehaviour
{
    public GameObject phonePanel;

    void Start()
    {
        phonePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            phonePanel.SetActive(!phonePanel.activeSelf);
    }
}
```

---

## 4. Time Progression System

**Overview:** Tracks duration of named actions to accumulate time spent.

```csharp
// TimeManager.cs
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    private Dictionary<string, float> timeSpent = new Dictionary<string, float>();
    private string currentAction;
    private float actionStartTime;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartAction(string actionName)
    {
        if (!string.IsNullOrEmpty(currentAction)) EndAction();
        currentAction = actionName;
        actionStartTime = Time.time;
    }

    public void EndAction()
    {
        if (string.IsNullOrEmpty(currentAction)) return;
        float duration = Time.time - actionStartTime;
        if (!timeSpent.ContainsKey(currentAction)) timeSpent[currentAction] = 0f;
        timeSpent[currentAction] += duration;
        currentAction = null;
    }

    public float GetTimeSpent(string actionName)
        => timeSpent.TryGetValue(actionName, out var t) ? t : 0f;
}
```

---

**Usage Notes:**

* Call `Inventory.Instance.Add(itemSO)` within `IInteractable.Interact()` to pick up items.
* Use `TimeManager.Instance.StartAction("ActionName")` and `EndAction()` around significant actions.
* Wire up prefabs, UI panels, layers, and input keys in the Inspector.

---

## 5. Pickup & Examine System

**Overview:** Allows the player to pick up non-inventory objects tagged as "Examinable", hold them in front of the camera, and rotate them by moving the mouse. Press left click to pick up or drop the object, and while holding, spin the mouse to examine.

**Setup Steps:**

1. **Layer & Components**: Create a layer named **Examinable**. Assign it to all objects you want to inspect. Ensure each has a Collider and a Rigidbody (set **isKinematic** to **false** by default).
2. **Hold Parent**: Under your **Camera**, create an empty GameObject called **HoldPoint** at a suitable distance (e.g. 2 units forward). Assign it as the `holdParent` in the script.
3. **ExamineController**: Attach this to your **Player** (same GameObject as FirstPersonController).
4. **ExaminableObject**: Attach to each object to inspect.

```csharp
// ExamineController.cs
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
        fpController = GetComponent<FirstPersonController>();
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
```

```csharp
// ExaminableObject.cs
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class ExaminableObject : MonoBehaviour
{
    void Reset()
    {
        gameObject.layer = LayerMask.NameToLayer("Examinable");
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }
}
```

*End of setup.*
