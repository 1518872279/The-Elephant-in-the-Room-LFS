using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UITestHelper : MonoBehaviour
{
    [Header("UI Test Settings")]
    public Button testButton;
    public Text debugText;
    
    void Start()
    {
        // Test if EventSystem exists
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("UITestHelper: No EventSystem found! UI won't work!");
        }
        else
        {
            Debug.Log($"UITestHelper: EventSystem found: {eventSystem.name}");
        }
        
        // Test if Canvas has GraphicRaycaster
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                Debug.LogError("UITestHelper: Canvas missing GraphicRaycaster! UI won't work!");
            }
            else
            {
                Debug.Log("UITestHelper: GraphicRaycaster found on Canvas");
            }
        }
        
        // Set up test button if assigned
        if (testButton != null)
        {
            testButton.onClick.AddListener(OnTestButtonClick);
        }
    }
    
    void OnTestButtonClick()
    {
        Debug.Log("UITestHelper: Test button clicked! UI is working!");
        if (debugText != null)
        {
            debugText.text = "Button clicked at: " + System.DateTime.Now.ToString("HH:mm:ss");
        }
    }
    
    void Update()
    {
        // Test mouse position and UI raycasting
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            Debug.Log($"UITestHelper: Mouse clicked at: {mousePos}");
            
            // Test if UI is blocking the raycast
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = mousePos;
            
            System.Collections.Generic.List<RaycastResult> results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            if (results.Count > 0)
            {
                Debug.Log($"UITestHelper: UI element hit: {results[0].gameObject.name}");
            }
            else
            {
                Debug.Log("UITestHelper: No UI element hit");
            }
        }
    }
} 