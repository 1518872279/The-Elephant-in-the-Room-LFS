 PhoneUIManager (Reset to Home on Open)

This **PhoneUIManager** version ensures that whenever the phone is activated (via `OpenPhone()`), it always returns to the home page (page index 0), regardless of which page was active previously.

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Canvas))]
public class PhoneUIManager : MonoBehaviour
{
    [Header("Phone Panel")]
    [Tooltip("Root GameObject for the entire phone UI")]
    public GameObject phonePanel;

    [Header("Phone Pages")]
    [Tooltip("Individual sub-panels/screens (e.g. Home, ShopSecondary, MessagesSecondary, MessageTertiary, ShopTertiary)")]
    public List<GameObject> pages;

    [Header("Player Control")]
    [Tooltip("Reference to the FirstPersonController to pause movement")]
    public FirstPersonController playerController;

    // Internal index of the currently active page
    private int currentPage = -1;

    void Start()
    {
        phonePanel.SetActive(false);
        HideAllPages();

        if (playerController == null)
            playerController = FindObjectOfType<FirstPersonController>();

        EnsureEventSystem();
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogWarning("PhoneUIManager: No EventSystem found! Creating one...");
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }

    /// <summary>
    /// Opens the phone UI, resets to the home page (index 0),
    /// pauses player control, and shows the cursor for UI interaction.
    /// </summary>
    public void OpenPhone()
    {
        // Activate the panel
        phonePanel.SetActive(true);

        // Reset to home page
        HideAllPages();
        ShowPage(0);

        // Pause player movement
        if (playerController != null)
            playerController.enabled = false;

        // Show cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        EnsureEventSystem();
        Debug.Log("Phone opened and reset to home page");
    }

    /// <summary>
    /// Closes the phone UI and resumes gameplay.
    /// </summary>
    public void ClosePhone()
    {
        phonePanel.SetActive(false);
        HideAllPages();

        if (playerController != null)
            playerController.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Switches to the specified page index, deactivating the previous one.
    /// </summary>
    public void ShowPage(int index)
    {
        if (index < 0 || index >= pages.Count) return;

        // Deactivate the current page if valid
        if (currentPage >= 0 && currentPage < pages.Count)
            pages[currentPage].SetActive(false);

        // Activate the new page
        pages[index].SetActive(true);
        currentPage = index;
    }

    private void HideAllPages()
    {
        for (int i = 0; i < pages.Count; i++)
            pages[i].SetActive(false);
        currentPage = -1;
    }

    // Named helper methods for use in button OnClick() events:
    public void ShowHome()              => ShowPage(0);
    public void ShowShopSecondary()     => ShowPage(1);
    public void ShowMessagesSecondary() => ShowPage(2);
    public void ShowMessageTertiory()   => ShowPage(3);
    public void ShowShopTertiory()      => ShowPage(4);
}
```

## Setup Instructions

1. **Save Script**  
   Save this code as `PhoneUIManager.cs` in your project’s Scripts folder, replacing the existing one.

2. **Inspector Configuration**  
   - **Phone Panel**: Drag your root phone UI panel.  
   - **Pages**: Set the list size to match your sub-panels (Home, ShopSecondary, MessagesSecondary, MessageTertiary, ShopTertiary) and assign each in order.  
   - **Player Controller**: (Optional) Drag your `FirstPersonController`, or leave blank to auto-find.

3. **UI Buttons**  
   - Hook each interface button’s **On Click()** to its helper method (e.g., `ShowHome()`, `ShowShopSecondary()`, etc.).  
   - Call `OpenPhone()` from your activation logic (e.g., key press) and `ClosePhone()` from your close button or input.

With this setup, every time the player opens their phone, the UI will reset to the home page automatically, providing a consistent starting point.

# Phone Messaging System

This file defines a timed messaging system that delivers messages at the start of the morning or evening and allows the player to reply with two choices.

## 1. Extending DayPartManager

First, broadcast day-part changes by adding an event to your existing `DayPartManager`:

```csharp
using System;

// inside DayPartManager class
public event Action<DayPart> OnDayPartChanged;

private void OnTimeChanged(int minutes)
{
    var newPart = DeterminePart(minutes);
    if (newPart != currentPart)
    {
        OnDayPartChanged?.Invoke(newPart);    // fire event
        ApplyPart(newPart);
        currentPart = newPart;
    }
}
```

## 2. Define the TimedMessage Data

Create a serializable class to configure each message in the Inspector:

```csharp
[System.Serializable]
public class TimedMessage
{
    public DayPartManager.DayPart triggerPart;  // Morning or Evening
    public string senderName;
    [TextArea] public string messageText;
    public string replyOption1;
    public string replyOption2;
}
```

## 3. MessageManager Script

Add a new `MessageManager` to handle scheduling, display, and replies:

```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance { get; private set; }

    [Header("Configured Messages")]
    public List<TimedMessage> messages;

    [Header("UI References")]
    public GameObject messagePanel;    // The pop-up panel
    public Text senderText;            // UI Text for sender name
    public Text messageContentText;    // UI Text for message body
    public Button replyButton1;        // Button for first reply choice
    public Button replyButton2;        // Button for second reply choice

    // Fired after the player selects a reply (0 or 1)
    public event System.Action<int> OnReplySelected;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Hide at start
        messagePanel.SetActive(false);
        // Subscribe to day-part changes
        DayPartManager.Instance.OnDayPartChanged += ShowMessageForPart;
    }

    void OnDestroy()
    {
        if (DayPartManager.Instance != null)
            DayPartManager.Instance.OnDayPartChanged -= ShowMessageForPart;
    }

    private void ShowMessageForPart(DayPartManager.DayPart part)
    {
        // Find the first matching message for this part
        TimedMessage msg = messages.Find(m => m.triggerPart == part);
        if (msg == null) return;

        // Populate UI
        senderText.text         = msg.senderName;
        messageContentText.text = msg.messageText;
        replyButton1.GetComponentInChildren<Text>().text = msg.replyOption1;
        replyButton2.GetComponentInChildren<Text>().text = msg.replyOption2;

        // Clear previous listeners
        replyButton1.onClick.RemoveAllListeners();
        replyButton2.onClick.RemoveAllListeners();

        // Hook up new listeners
        replyButton1.onClick.AddListener(() => HandleReply(0));
        replyButton2.onClick.AddListener(() => HandleReply(1));

        // Show panel
        messagePanel.SetActive(true);
    }

    private void HandleReply(int choice)
    {
        // Notify subscribers which choice was made
        OnReplySelected?.Invoke(choice);
        // Hide the panel
        messagePanel.SetActive(false);
    }
}
```

## 4. Setup Instructions

1. **Extend DayPartManager**  
   - Implement the `OnDayPartChanged` event as shown.

2. **Create UI Panel**  
   - Add a `messagePanel` under your Canvas.  
   - Inside it, place two Text components (for sender and message) and two Buttons.  
   - Assign those GameObjects to the `MessageManager` fields in the Inspector.

3. **Populate Messages**  
   - In the `MessageManager` component, set the size of **Configured Messages**.  
   - For each entry, choose **triggerPart** (Morning or Evening), fill **senderName**, **messageText**, and both **replyOption1** & **replyOption2**.

4. **Handle Replies**  
   - Subscribe to `MessageManager.Instance.OnReplySelected` from your game logic to react (e.g., adjust happiness, trigger events) based on the player’s choice.

With these scripts in place, players will receive a message from a specified sender at the start of each morning or evening, and can reply using one of two options. Let me know if you need additional customization!
