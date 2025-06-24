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
