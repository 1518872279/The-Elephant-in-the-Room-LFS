using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance { get; private set; }

    [Header("Configured Messages")]
    public List<TimedMessage> messages = new List<TimedMessage>();
    
    // Current day part for synchronization
    private DayPartManager.DayPart currentDayPart = DayPartManager.DayPart.None;

    [Header("UI References")]
    public GameObject SecondaryInterface;
    public GameObject messagePanel;    // The pop-up panel
    public TextMeshProUGUI senderText;            // UI Text for sender name
    public TextMeshProUGUI messageContentText;    // UI Text for message body
    public Button replyButton1;        // Button for first reply choice
    public Button replyButton2;        // Button for second reply choice
    
    [Header("Reply Display")]
    public TextMeshProUGUI chosenReplyText;    // UI Text to show the chosen reply
    public GameObject replyOptionsPanel;        // Panel containing the reply buttons
    
    //[Header("Message Preview")]
    // [DISABLED] Preview system temporarily disabled
    // public GameObject previewPanel;             // Panel to show all messages for current day part
    // public Transform previewContentParent;      // Parent transform for preview message items
    // public GameObject messagePreviewItemPrefab; // Prefab for individual message preview items

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
        
        // Ensure chosen reply text is hidden initially
        if (chosenReplyText != null)
            chosenReplyText.gameObject.SetActive(false);
        
        // Initialize preview panel (disabled)
        // if (previewPanel != null)
        //     previewPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (DayPartManager.Instance != null)
            DayPartManager.Instance.OnDayPartChanged -= ShowMessageForPart;
    }

    private void ShowMessageForPart(DayPartManager.DayPart part)
    {
        Debug.Log($"MessageManager: Day part changed to {part}");
        currentDayPart = part;
        ShowFirstMessageForDayPart(part);
    }
    
    /// <summary>
    /// Shows the first message for the given day part (simplified without preview)
    /// </summary>
    public void ShowFirstMessageForDayPart(DayPartManager.DayPart dayPart)
    {
        Debug.Log($"MessageManager: Looking for messages for day part {dayPart}");
        Debug.Log($"MessageManager: Total messages in list: {messages.Count}");
        
        // Find all messages for the day part
        List<TimedMessage> dayPartMessages = messages.FindAll(m => m.triggerPart == dayPart);
        Debug.Log($"MessageManager: Found {dayPartMessages.Count} messages for {dayPart}");
        
        if (dayPartMessages.Count == 0)
        {
            Debug.Log("MessageManager: No messages found for this day part");
            return;
        }

        // Show the first message in the main panel
        Debug.Log("MessageManager: Showing first message in main panel");
        TimedMessage firstMsg = dayPartMessages[0];
        ShowMessageInMainPanel(firstMsg);
    }
    
    // [DISABLED] Preview panel functionality temporarily disabled
    // private void ShowPreviewPanel(List<TimedMessage> dayPartMessages)
    // {
    //     // Preview panel code commented out
    // }
    
    public void ShowMessageInMainPanel(TimedMessage msg)
    {
        Debug.Log($"MessageManager: Showing message in main panel from {msg.senderName}");
        
        // Check if UI references are assigned
        if (senderText == null)
        {
            Debug.LogError("MessageManager: Sender text is null!");
            return;
        }
        
        if (messageContentText == null)
        {
            Debug.LogError("MessageManager: Message content text is null!");
            return;
        }
        
        if (replyButton1 == null || replyButton2 == null)
        {
            Debug.LogError("MessageManager: Reply buttons are null!");
            return;
        }
        
        // Populate UI
        senderText.text         = msg.senderName;
        messageContentText.text = msg.messageText;
        
        var button1Text = replyButton1.GetComponentInChildren<TextMeshProUGUI>();
        var button2Text = replyButton2.GetComponentInChildren<TextMeshProUGUI>();
        
        if (button1Text != null)
            button1Text.text = msg.replyOption1;
        else
            Debug.LogError("MessageManager: Button 1 text component is null!");
            
        if (button2Text != null)
            button2Text.text = msg.replyOption2;
        else
            Debug.LogError("MessageManager: Button 2 text component is null!");

        // Clear previous listeners
        replyButton1.onClick.RemoveAllListeners();
        replyButton2.onClick.RemoveAllListeners();

        // Hook up new listeners
        replyButton1.onClick.AddListener(() => HandleReply(0, msg.replyOption1));
        replyButton2.onClick.AddListener(() => HandleReply(1, msg.replyOption2));

        // Reset UI state
        if (chosenReplyText != null)
            chosenReplyText.gameObject.SetActive(false);
        if (replyOptionsPanel != null)
            replyOptionsPanel.SetActive(true);

        // Show panel
        messagePanel.SetActive(true);
        Debug.Log("MessageManager: Main message panel activated");
    }

    private void HandleReply(int choice, string chosenReply)
    {
        // Show the chosen reply text
        if (chosenReplyText != null)
        {
            chosenReplyText.text = chosenReply;
            chosenReplyText.gameObject.SetActive(true);
        }
        
        // Hide the reply options
        if (replyOptionsPanel != null)
            replyOptionsPanel.SetActive(false);
        
        // Notify subscribers which choice was made
        OnReplySelected?.Invoke(choice);
        
        // Note: Panel stays open to show the chosen reply
        // You can add a timer or close button to hide it later
    }
    
    /// <summary>
    /// Adds a new message to the list
    /// </summary>
    public void AddMessage(TimedMessage newMessage)
    {
        messages.Add(newMessage);
        Debug.Log($"MessageManager: Added message from {newMessage.senderName} for {newMessage.triggerPart}");
    }
    
    /// <summary>
    /// Removes a message from the list
    /// </summary>
    public void RemoveMessage(TimedMessage messageToRemove)
    {
        if (messages.Remove(messageToRemove))
        {
            Debug.Log($"MessageManager: Removed message from {messageToRemove.senderName}");
        }
    }
    
    /// <summary>
    /// Updates a message in the list
    /// </summary>
    public void UpdateMessage(TimedMessage oldMessage, TimedMessage updatedMessage)
    {
        int index = messages.IndexOf(oldMessage);
        if (index != -1)
        {
            messages[index] = updatedMessage;
            Debug.Log($"MessageManager: Updated message from {updatedMessage.senderName}");
        }
    }
    
    /// <summary>
    /// Clears all messages
    /// </summary>
    public void ClearAllMessages()
    {
        messages.Clear();
        Debug.Log("MessageManager: Cleared all messages");
    }
    
    /// <summary>
    /// Gets all messages for a specific day part
    /// </summary>
    public List<TimedMessage> GetMessagesForDayPart(DayPartManager.DayPart dayPart)
    {
        return messages.FindAll(m => m.triggerPart == dayPart);
    }
    
    /// <summary>
    /// Gets all messages for a specific contact, day, and day part
    /// </summary>
    public List<TimedMessage> GetMessagesForContact(string contactName, int day, DayPartManager.DayPart dayPart)
    {
        return messages.FindAll(m => 
            m.senderName == contactName && 
            m.day == day && 
            m.triggerPart == dayPart);
    }
    
    /// <summary>
    /// Gets the current active day part
    /// </summary>
    public DayPartManager.DayPart GetCurrentDayPart()
    {
        return currentDayPart;
    }
    
    /// <summary>
    /// Test method to manually trigger message display (for debugging)
    /// </summary>
    [ContextMenu("Test Message Display")]
    public void TestMessageDisplay()
    {
        Debug.Log("MessageManager: Testing message display...");
        
        // Create a test message if none exist
        if (messages.Count == 0)
        {
            Debug.Log("MessageManager: No messages found, creating test message");
            TimedMessage testMsg = new TimedMessage();
            testMsg.triggerPart = DayPartManager.DayPart.Morning;
            testMsg.day = 1; // Test for day 1
            testMsg.senderName = "Test Sender";
            testMsg.messageText = "This is a test message to verify the UI is working properly.";
            testMsg.replyOption1 = "Test Reply 1";
            testMsg.replyOption2 = "Test Reply 2";
            messages.Add(testMsg);
        }
        //disable the secondary interface after tertiory interface is activated
        SecondaryInterface.SetActive(false);
        // Show first message for current day part
        ShowFirstMessageForDayPart(DayPartManager.DayPart.Morning);

    }
    
    /// <summary>
    /// Test method to manually set day part and trigger message display
    /// </summary>
    [ContextMenu("Test Morning Messages")]
    public void TestMorningMessages()
    {
        Debug.Log("MessageManager: Testing morning messages...");
        ShowFirstMessageForDayPart(DayPartManager.DayPart.Morning);
    }
    
    [ContextMenu("Test Evening Messages")]
    public void TestEveningMessages()
    {
        Debug.Log("MessageManager: Testing evening messages...");
        ShowFirstMessageForDayPart(DayPartManager.DayPart.Evening);
    }
} 