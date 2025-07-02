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
// PanControllerDirect.cs
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
```

### Alternative Pan Control: Keyboard Input

```csharp
// PanControllerKeyboard.cs
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
        Quaternion targetRot = Quaternion.Euler(xAngle, 0f, zAngle);
        panTransform.localRotation = Quaternion.RotateTowards(
            panTransform.localRotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );
    }
}
```

---

## 11. Physics Configuration for Pan & Food

**Objective:** Prevent physics jitter by correctly configuring Rigidbody and Collider settings for both the pan and food items.

### Pan Setup

1. **Rigidbody Settings:**

   * Remove or disable the pan’s `Rigidbody` component if using **PanControllerDirect** or **PanControllerKeyboard** (transform-based rotation).
   * If a `Rigidbody` is required, set it to **isKinematic = true** and under **Constraints**, freeze **Position X/Y/Z** and **Rotation Y** (allow only X/Z rotation).
   * Set **Collision Detection** to **Discrete** for stable behavior.
2. **Collider Settings:**

   * Use a **Mesh Collider** or **Box Collider** matching the pan’s interior as a **trigger** if you only need clamping logic (no physical bounce).

### Food Setup

1. **Rigidbody Settings:**

   * On each food prefab’s `Rigidbody`, set **Use Gravity = false** to prevent falling and interference.
   * During drag (`OnMouseDown`), set `rb.isKinematic = true` to disable physics.
2. **Drop Logic:**

   * After clamping inside the pan (in `OnMouseUp`), parent the food to the pan’s transform and keep its `Rigidbody.isKinematic = true`. This attaches the food without physics jitter.

```csharp
// Updated DraggableFood.Drop logic
void OnMouseUp()
{
    var rb = GetComponent<Rigidbody>();
    if (rb) rb.isKinematic = true;
    if (panCollider != null)
    {
        Vector3 clampedPos = panCollider.ClosestPoint(transform.position);
        transform.position = clampedPos;
    }
    // Parent to pan for static positioning
    transform.SetParent(panCollider.transform);
}
```

With these settings, the pan will rotate smoothly via transform controls, and food will remain firmly inside without physics-induced shaking.

---

## 12. Garbage Cleanup Mini-Game

**Overview:**
Procedurally generate floor-based stains and trash with multiple variation prefabs, within defined spawn ranges. Players must approach each item and press left mouse button to clean. The event advances time by a fixed 30 minutes.

### TimeManager Setup

1. In **TimeManager** inspector, add:

   * **Event Name:** "GarbageCleanup"
   * **Duration:** 30 (minutes)

### Scene Setup

1. Create empty GameObjects with **BoxCollider** (set as trigger) to define spawn volumes:

   * **StainRanges** parent object; its children represent individual spawn areas.
   * **TrashRanges** parent object.
2. Assign these parent objects’ transforms in the generator.
3. Ensure all BoxColliders cover only floor regions (no ceilings or walls).
4. Set the **Floor** layer on your floor meshes; assign this layer to `floorLayer`.

### GarbageCleanupController

Attach this to an empty **GarbageCleanupController** GameObject and configure:

```csharp
// GarbageCleanupController.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GarbageCleanupController : MonoBehaviour
{
    public static GarbageCleanupController Instance;

    [Header("Range Parents (with BoxColliders)")]
    public Transform[] stainRanges;
    public Transform[] trashRanges;

    [Header("Garbage Variations & Counts")]
    public GameObject[] stainPrefabs;
    public GameObject[] trashPrefabs;
    public int stainCount = 10;
    public int trashCount = 8;

    [Header("Spawn Settings")]
    public LayerMask floorLayer;
    public float verticalOffset = 0.01f;

    [Header("Debug UI")]
    public Text debugText;

    [Header("End Fade Image")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    private List<GameObject> spawnedItems = new List<GameObject>();
    private int totalItems;
    private int cleanedItems;

    void Awake()
    {
        Instance = this;
    }

    /// <summary>Call this when GarbageCleanup event starts.</summary>
    public void StartMinigame()
    {
        GenerateGarbage();
        cleanedItems = 0;
        totalItems = spawnedItems.Count;
        UpdateDebugText();
    }

    void GenerateGarbage()
    {
        // Clear previous
        foreach (var go in spawnedItems) Destroy(go);
        spawnedItems.Clear();

        // Spawn stains and trash
        SpawnVariations(stainRanges, stainPrefabs, stainCount);
        SpawnVariations(trashRanges, trashPrefabs, trashCount);

        // Set totals
        totalItems = spawnedItems.Count;
    }

    void SpawnVariations(Transform[] ranges, GameObject[] prefabs, int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Pick random range
            Transform rangeT = ranges[Random.Range(0, ranges.Length)];
            var box = rangeT.GetComponent<BoxCollider>();
            Vector3 randomPoint = new Vector3(
                Random.Range(box.bounds.min.x, box.bounds.max.x),
                box.bounds.max.y + 1f,
                Random.Range(box.bounds.min.z, box.bounds.max.z)
            );
            // Raycast down to floor
            if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, Mathf.Infinity, floorLayer))
            {
                Vector3 spawnPos = hit.point + Vector3.up * verticalOffset;
                // Select random prefab variation
                GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
                var go = Instantiate(prefab, spawnPos, Quaternion.identity);
                go.AddComponent<GarbageItem>();
                spawnedItems.Add(go);
            }
        }
    }

    public void ItemCleaned()
    {
        cleanedItems++;
        UpdateDebugText();
        if (cleanedItems >= totalItems)
        {
            StartCoroutine(EndRoutine());
        }
    }

    void UpdateDebugText()
    {
        if (debugText != null)
            debugText.text = $"Cleaned: {cleanedItems} / {totalItems}";
        else
            Debug.Log($"Cleaned: {cleanedItems} / {totalItems}");
    }

    private IEnumerator EndRoutine()
    {
        // Fade to black
        float t = 0f;
        while (t < fadeDuration)
        {
            fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }
        // Fade back in
        t = fadeDuration;
        while (t > 0f)
        {
            fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
            t -= Time.deltaTime;
            yield return null;
        }
        // End of minigame logic, e.g., advance time
        TimeManager.Instance.TryStartEvent("GarbageCleanup");
    }
}
```

### Garbage Item Interaction

Attach this to each generated garbage instance (or let the controller add it):

```csharp
// GarbageItem.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GarbageItem : MonoBehaviour
{
    public float interactDistance = 2f;
    private Camera cam;

    void Start() => cam = Camera.main;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance)
                && hit.collider.gameObject == gameObject)
            {
                // Clean and remove
                Destroy(gameObject);
            }
        }
    }
}
```

### Hookup

In **EventInteractionController**, handle the "GarbageCleanup" event:

```csharp
if (evtObj.eventName == "GarbageCleanup")
    GarbageCleanupController.Instance.StartMinigame();
else
    TimeManager.Instance.TryStartEvent(evtObj.eventName);
```

*End of setup.*

*End of setup.*

---

## Triggering the Cooking Mini-Game

To launch the breakfast mini-game when the player interacts with the stove:

1. **Stove Setup**

   * Create an empty GameObject at the stove and add the **EventObject** component.
   * Set its **eventName** to **"Breakfast"**.
   * Assign it to the **EventObject** layer.

2. **EventInteractionController**

   * Update the interaction logic so that when **eventName** is **"Breakfast"**, you invoke the cooking mini-game instead of advancing time:

```csharp
// Inside EventInteractionController.cs, replace or extend the interaction block:
if (evtObj.eventName == "Breakfast")
{
    // Launch cooking mini-game
    CookingMinigameController.Instance.StartMinigame();
}
else
{
    // Regular time event
    TimeManager.Instance.TryStartEvent(evtObj.eventName);
}
```

3. **Input Trigger**

   * Ensure your `eventLayer` mask on **EventInteractionController** includes the **EventObject** layer.
   * Use the same left-click interaction: pointing at the stove and clicking will now start the mini-game.

That’s it—interacting with the stove object labeled "Breakfast" will teleport the player and begin the cooking sequence. Let me know if you need any refinements!

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

**Overview:** Allows players to trigger events by interacting with world objects on the **EventObject** layer. Each object specifies its associated `eventName`, and interaction attempts to start the event and advance time.

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

**Overview:** Displays a context-sensitive cursor hint at the screen center when an interactable object is in range.

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

\*End of setup.\*csharp // TimeManager.cs using System; using System.Collections.Generic; using UnityEngine;

public class TimeManager : MonoBehaviour { public static TimeManager Instance { get; private set; }

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

````##

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

## 9. Breakfast Cooking Mini‑Game

**Overview:**
A dedicated mini‑game for cooking breakfast when the **Breakfast** event is triggered. Players are teleported to a stove area, camera and movement are locked, and they must drag ingredients into the pan in a fixed sequence while shaking the pan. The mini‑game runs for a configurable duration (default 15 s) and ends with a screen fade transition, then returns the player and camera to their original state.

### Setup Steps:

1. **Scene Setup**

   * Create an empty GameObject **StovePoint** at the stove location; this is the `teleportPoint`.
   * Create an empty child **CameraLockPoint** under **StovePoint**; this is where you lock the camera during the mini‑game.
2. **UI Setup**

   * Add a **MinigameCanvas** (Screen Space–Overlay) with:

     * An Image **FadeImage** covering full screen (black, alpha=0).
     * Any UI instructions or timers you need.
   * Disable **MinigameCanvas** by default.
3. **Prefabs**

   * Prepare ingredient prefabs (Bacon, Egg, Toast) with Collider + Rigidbody.
   * Tag or layer them as **Pickable**.
   * Add the **DraggableFood** component (script below) to each prefab for mouse-driven dragging; for eggs, assign the `cookedPrefab` field to your cooked-egg prefab.
4. **Controllers**

   * Attach the **CookingMinigameController** to your Player.

     * Assign `teleportPoint`, `cameraLockPoint`, `fpController`, `playerCamera`, `minigameCanvas`, `fadeImage`, `panController`, and ingredient prefabs + `spawnPoint`.
     * Set `gameDuration` (default 15 s).
   * Create a **PanController** script on your pan object; assign its `panTransform` and `panRigidbody`.
5. **Event Hookup**

   * In your **EventInteractionController**, when `evtObj.eventName == "Breakfast"`, call `CookingMinigameController.Instance.StartMinigame()` instead of `TryStartEvent`.

```csharp
// DraggableFood.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DraggableFood : MonoBehaviour
{
    private Camera cam;
    private Vector3 offset;
    private float zDepth;

    [Header("Pan Bounds")]
    [Tooltip("Collider of the pan to clamp dropped items inside its bounds")]
    public Collider panCollider;

    [Header("Optional Cooked Prefab")]
    [Tooltip("For items that change upon cooking (e.g., eggs)")]
    public GameObject cookedPrefab;

    void Start()
    {
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        zDepth = cam.WorldToScreenPoint(transform.position).z;
        Vector3 screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDepth);
        offset = transform.position - cam.ScreenToWorldPoint(screenPoint);
    }

    void OnMouseDrag()
    {
        Vector3 curScreen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDepth);
        Vector3 curWorld = cam.ScreenToWorldPoint(curScreen) + offset;
        transform.position = curWorld;
    }

    void OnMouseUp()
    {
        // Disable physics
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        Vector3 finalPos = transform.position;
        if (panCollider != null)
        {
            // Project position onto pan surface
            Transform panTransform = panCollider.transform;
            Plane panPlane = new Plane(panTransform.up, panTransform.position);
            Vector3 rayOrigin = transform.position + panTransform.up * 5f;
            Ray downRay = new Ray(rayOrigin, -panTransform.up);
            if (panPlane.Raycast(downRay, out float distance))
            {
                Vector3 surfacePoint = downRay.GetPoint(distance);
                finalPos = panCollider.ClosestPoint(surfacePoint);
            }
        }

        if (cookedPrefab != null)
        {
            // Instantiate cooked version with correct orientation and destroy raw
            Quaternion spawnRot = Quaternion.FromToRotation(Vector3.up, panCollider.transform.up);
            Instantiate(cookedPrefab, finalPos, spawnRot, panCollider.transform);
            Destroy(gameObject);
        }
        }
        else
        {
            // Place original on pan surface
            transform.position = finalPos;
            transform.SetParent(panCollider.transform);
        }
    }
}
```

*End of setup.*
