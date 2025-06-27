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
// InventoryUI.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Hotbar Slots (Bottom HUD)")]
    [Tooltip("Assign six slot UI Images in order: slots 1-6 from left to right")]  
    public Image[] hotbarSlots;    // length = 6

    [Header("Fixed Items (Slots 1-3)")]
    [Tooltip("Assign the Phone, Wallet, and Watch ScriptableItems here in order")]  
    public List<Item> fixedItems;  // must contain exactly 3 items: Phone, Wallet, Watch

    void Start()
    {
        Inventory.Instance.OnChanged += RefreshUI;
        // initially hide all slot icons
        ClearAllSlots();
    }

    void RefreshUI()
    {
        ClearAllSlots();

        // 1. Place fixed items in slots 1-3 if owned
        for (int i = 0; i < fixedItems.Count && i < hotbarSlots.Length; i++)
        {
            if (Inventory.Instance.Items.Contains(fixedItems[i]))
            {
                hotbarSlots[i].sprite = fixedItems[i].icon;
                hotbarSlots[i].enabled = true;
            }
        }

        // 2. Place remaining items in FILO order into slots 4-6
        List<Item> dynamicItems = new List<Item>();
        foreach (var item in Inventory.Instance.Items)
        {
            if (!fixedItems.Contains(item))
                dynamicItems.Add(item);
        }
        
        // FILO: most recently added item first
        dynamicItems.Reverse();
        int startIndex = fixedItems.Count;
        for (int j = 0; j < dynamicItems.Count && (startIndex + j) < hotbarSlots.Length; j++)
        {
            hotbarSlots[startIndex + j].sprite = dynamicItems[j].icon;
            hotbarSlots[startIndex + j].enabled = true;
        }
    }

    void ClearAllSlots()
    {
        foreach (var slot in hotbarSlots)
        {
            slot.sprite = null;
            slot.enabled = false;
        }
    }

    void Update()
    {
        // Optionally, handle hotkey selection: 1-6
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                // TODO: implement use of that slot's item
            }
        }
    }
}
```

---

## 3. Phone UI Panel

**Overview:** Toggleable panel for in-game phone interface with URP Post-Processing blur effect.

**Prerequisites:** Ensure your project uses the **Universal Render Pipeline** and has Post-Processing enabled in your URP Asset.

**Setup Steps:**

1. **Create UI Panel**: In your Canvas, create a **PhonePanel** GameObject (design your UI here) and set it inactive.
2. **Add a Global Volume**:

   * In the Hierarchy, create an empty GameObject named **PostProcessVolume**.
   * Add a **Volume** component, check **Is Global**, and assign a new **Volume Profile**.
   * In the Volume Profile, click **Add Override** ▶ **Unity** ▶ **DepthOfField**.
   * Configure **DepthOfField** settings:

     * **Focus Distance**: e.g. 0.1 (keeps the phone in sharp focus)
     * **Aperture**: e.g. 32 (higher values yield stronger blur)
     * **Focal Length**: e.g. 50
   * Set the Volume’s **Weight** to **0**.
3. **Assign References**: On your **PhoneUIController** script, expose the **Volume** reference alongside the **phonePanel**.
4. **Toggle Logic**: Update your script to enable/disable both the phone UI and the Volume’s weight.

```csharp
// PhoneUIController.cs
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PhoneUIController : MonoBehaviour
{
    [Header("UI and Post-Process References")]
    public GameObject phonePanel;
    public Volume postProcessVolume;

    private DepthOfField dof;

    void Start()
    {
        // Start with UI and blur disabled
        phonePanel.SetActive(false);
        postProcessVolume.weight = 0f;

        // Cache the DepthOfField override
        if (!postProcessVolume.profile.TryGet(out dof))
            Debug.LogWarning("DepthOfField override not found on Volume Profile.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            TogglePhone();
    }

    void TogglePhone()
    {
        bool isActive = !phonePanel.activeSelf;
        phonePanel.SetActive(isActive);
        postProcessVolume.weight = isActive ? 1f : 0f;

        if (isActive && dof != null)
        {
            // Focus very close so the background blurs
            dof.focusDistance.value = 0.1f;
        }
    }
}
```

## 7. Event Trigger Tester

**Overview:**
A simple prototype script that lets you trigger defined events via number keys (1–n) to advance game time and test your event logic.

```csharp
// EventTester.cs
using UnityEngine;

public class EventTester : MonoBehaviour
{
    [Tooltip("List of event names defined in TimeManager, order corresponds to number keys 1..n")]  
    public string[] testEvents;

    void Update()
    {
        for (int i = 0; i < testEvents.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                bool started = TimeManager.Instance.TryStartEvent(testEvents[i]);
                if (started)
                    Debug.Log($"[EventTester] Started event '{testEvents[i]}'. Current time: {TimeManager.Instance.GetCurrentTime()} mins since midnight.");
                else
                    Debug.LogWarning($"[EventTester] Failed to start '{testEvents[i]}'. Either undefined or exceeds window.");
            }
        }
    }
}
```

Place this on any GameObject (e.g. a DebugManager) and assign your event names in the inspector to quickly test your time-driven events.

*End of setup.*

---

## 6. Day-Part Manager

**Overview:** Automatically switches URP post-processing volumes and lighting settings based on the current game time window (morning/evening).

```csharp
// DayPartManager.cs
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
```

## *End of setup.*

**UI Blur Shader & Material:**

1. **Create the Shader:** In `Assets/Shaders/`, create a new shader file named **UIBlur.shader** with the following content:

```shader
Shader "UI/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Size ("Blur Size", Range(0,10)) = 2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Pass
        {
            Cull Off ZWrite Off ZTest Always
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t { float4 vertex : POSITION; float2 texcoord : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Size;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * 0.227027;
                col += tex2D(_MainTex, i.uv + float2(_Size, 0) * _MainTex_TexelSize.xy) * 0.316216;
                col += tex2D(_MainTex, i.uv - float2(_Size, 0) * _MainTex_TexelSize.xy) * 0.316216;
                col += tex2D(_MainTex, i.uv + float2(0, _Size) * _MainTex_TexelSize.xy) * 0.070270;
                col += tex2D(_MainTex, i.uv - float2(0, _Size) * _MainTex_TexelSize.xy) * 0.070270;
                return col;
            }
            ENDCG
        }
    }
}
```

2. **Create the Material:** Right-click the **UIBlur.shader** > **Create** > **Material**. Name it **UIBlurMat** and set its shader to **UI/Blur**.
3. **Assign Material:** Select your **BlurOverlay** Image and set its **Material** to **UIBlurMat**. Adjust the **\_Size** property on the material to control blur strength.

## *End of setup.*

\$1

### Note: Manual Time Progression

TimeManager advances `currentTime` only when `TryStartEvent` is called, so time remains static until the player triggers events.

## 7. Event Interaction System

**Overview:**
Allows players to trigger events by interacting with world objects on the **EventObject** layer. Each object specifies its associated `eventName`, and interaction attempts to start the event and advance time.

**Setup Steps:**

1. Create a layer named **EventObject** and assign it to all event-trigger objects.
2. Attach the `EventObject` component to each, setting the `eventName` in the Inspector.
3. Add the `EventInteractionController` to the Player (with a Camera component).
4. Configure the `eventLayer` mask to include **EventObject** and set `interactDistance`.

```csharp
// EventObject.cs
using UnityEngine;

public class EventObject : MonoBehaviour
{
    [Tooltip("Name of the event defined in TimeManager to trigger when interacted")]
    public string eventName;
}
```

````csharp
// EventInteractionController.cs
```csharp
// EventInteractionController.cs
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class EventInteractionController : MonoBehaviour
{
    [Header("Event Interaction Settings")]
    public LayerMask eventLayer;
    public float interactDistance = 3f;
    public Camera cam;

    void Start()
    {
        if (cam == null) cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, eventLayer))
            {
                var evtObj = hit.collider.GetComponent<EventObject>();
                if (evtObj)
                {
                    bool started = TimeManager.Instance.TryStartEvent(evtObj.eventName);
                    if (!started)
                        Debug.LogWarning($"Failed to trigger event '{evtObj.eventName}'.");
                }
            }
        }
    }
}
````

## 8. Interaction Hint UI

**Overview:**
Displays a context-sensitive cursor hint at the screen center when an interactable object is in range.

**Setup Steps:**

1. **Canvas & Hint Image**: In your UI Canvas (Screen Space – Overlay), add a full‑screen **HintCanvas**. Under it, create an **Image** named **HintIcon**, anchored to center (position (0.5,0.5)).

   * Default: assign a small white dot sprite and disable the Image component.
2. **Layers/Tags**: Ensure your interactive objects use layers or tags:

   * **Door** objects: tag as "Door".
   * **Pickable** objects: layer "Pickable" (or tag).
   * Other interactive: use your existing "Interactable" layer.
3. **InteractionHintController**: Attach this to your Player (with Camera). Assign:

   * `cam`: your main Camera.
   * `hintImage`: the HintIcon Image.
   * Sprites: `defaultDot`, `doorIcon`, `handIcon`.
   * `hintDistance`: matching your interaction range.
   * `interactableLayers`: mask including Door, Pickable, and Interactable layers.

```csharp
// InteractionHintController.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class InteractionHintController : MonoBehaviour
{
    public Camera cam;
    public Image hintImage;
    public float hintDistance = 3f;
    public LayerMask interactableLayers;

    [Header("Hint Sprites")]
    public Sprite defaultDot;
    public Sprite doorIcon;
    public Sprite handIcon;

    void Start()
    {
        if (cam == null) cam = GetComponent<Camera>();
        hintImage.enabled = false;
    }

    void Update()
    {
        // Raycast from screen center
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, hintDistance, interactableLayers))
        {
            // Determine icon based on object
            if (hit.collider.CompareTag("Door"))
                hintImage.sprite = doorIcon;
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Pickable"))
                hintImage.sprite = handIcon;
            else
                hintImage.sprite = defaultDot;

            hintImage.enabled = true;
        }
        else
        {
            hintImage.enabled = false;
        }
    }
}
```

\*End of setup.\*csharp
// TimeManager.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
public static TimeManager Instance { get; private set; }

```
[Header("Day Schedule (minutes since midnight)")]
public int morningStart = 8 * 60;
public int morningEnd   = 9 * 60;
public int eveningStart = 18 * 60;
public int eveningEnd   = 23 * 60;

[Header("Fixed Event Durations")]
public List<string> eventNames;
public List<int> eventDurations; // in minutes

private Dictionary<string, int> durations = new Dictionary<string,int>();
private int currentTime;

public event Action<int> OnTimeChanged;

void Awake()
{
    if (Instance == null) Instance = this;
    else { Destroy(gameObject); return; }

    for (int i = 0; i < Math.Min(eventNames.Count, eventDurations.Count); i++)
        durations[eventNames[i]] = eventDurations[i];

    currentTime = morningStart;
    OnTimeChanged?.Invoke(currentTime);
}

/// <summary>Tries to start an event by name. Advances time if within window.</summary>
public bool TryStartEvent(string eventName)
{
    if (!durations.TryGetValue(eventName, out int duration))
    {
        Debug.LogWarning($"Event '{eventName}' not defined.");
        return false;
    }
    int windowEnd = GetWindowEnd();
    if (currentTime + duration > windowEnd)
    {
        Debug.LogWarning($"Cannot start '{eventName}': exceeds time window.");
        return false;
    }
    currentTime += duration;
    OnTimeChanged?.Invoke(currentTime);
    return true;
}

private int GetWindowEnd()
{
    if (currentTime >= morningStart && currentTime < morningEnd)
        return morningEnd;
    if (currentTime >= eveningStart && currentTime < eveningEnd)
        return eveningEnd;
    return currentTime;
}

public int GetCurrentTime() => currentTime;
```

}

````## 5. Pickup & Examine System

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
````

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
