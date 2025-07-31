using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContactPreview : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI ContactName;
    public TextMeshProUGUI MessagePreview;
    public Button ButtonToReplyInterface;

    [Header("Contact Configuration")]
    [Tooltip("The name of the contact to show messages for")]
    public string contactName = "Contact A";

    private DayPartManager dayPartManager;
    private MessageManager messageManager;

    void Start()
    {
        // Get references to managers
        dayPartManager = DayPartManager.Instance;
        messageManager = MessageManager.Instance;

        if (dayPartManager == null)
        {
            Debug.LogError("ContactPreview: DayPartManager not found!");
            return;
        }

        if (messageManager == null)
        {
            Debug.LogError("ContactPreview: MessageManager not found!");
            return;
        }

        // Subscribe to day part changes
        dayPartManager.OnDayPartChanged += OnDayPartChanged;

        // Set the contact name in UI
        if (ContactName != null)
        {
            ContactName.text = contactName;
        }

        // Check UI components
        if (ContactName == null)
            Debug.LogWarning("ContactPreview: ContactName UI component is not assigned!");
        if (MessagePreview == null)
            Debug.LogWarning("ContactPreview: MessagePreview UI component is not assigned!");
        if (ButtonToReplyInterface == null)
            Debug.LogWarning("ContactPreview: ButtonToReplyInterface UI component is not assigned!");

        // Setup button click handler
        if (ButtonToReplyInterface != null)
        {
            ButtonToReplyInterface.onClick.RemoveAllListeners();
            ButtonToReplyInterface.onClick.AddListener(OnReplyButtonClicked);
        }
        
        // Delay initial update to ensure DayPartManager is properly initialized
        StartCoroutine(DelayedInitialUpdate());
    }
    
    private IEnumerator DelayedInitialUpdate()
    {
        // Wait a frame to ensure DayPartManager has initialized
        yield return null;
        
        // If day part is still None, wait a bit more
        if (dayPartManager.currentPart == DayPartManager.DayPart.None)
        {
            Debug.Log("ContactPreview: Waiting for DayPartManager to initialize...");
            yield return new WaitForSeconds(0.1f);
        }
        
        UpdateMessagePreview();
    }
    
    /// <summary>
    /// Handles the reply button click
    /// </summary>
    private void OnReplyButtonClicked()
    {
        // You can implement logic here to open the reply interface
        // For example, you might want to show the full message in MessageManager
        Debug.Log($"ContactPreview: Reply button clicked for {contactName}");
        
        // Optional: You could trigger the MessageManager to show the current message
        // if there is one for this contact on the current day/part
        int currentDay = dayPartManager.daysElapsed;
        DayPartManager.DayPart currentDayPart = dayPartManager.currentPart;
        List<TimedMessage> contactMessages = messageManager.GetMessagesForContact(contactName, currentDay, currentDayPart);
        
        if (contactMessages.Count > 0)
        {
            // Show the message in the main message panel
            messageManager.ShowMessageInMainPanel(contactMessages[0]);
        }
    }

    void OnDestroy()
    {
        if (dayPartManager != null)
            dayPartManager.OnDayPartChanged -= OnDayPartChanged;
    }

    private void OnDayPartChanged(DayPartManager.DayPart newDayPart)
    {
        Debug.Log($"ContactPreview: Day part changed to {newDayPart}");
        UpdateMessagePreview();
    }

    /// <summary>
    /// Updates the message preview based on current day, day part, and contact name
    /// </summary>
    public void UpdateMessagePreview()
    {
        if (messageManager == null || dayPartManager == null)
        {
            Debug.LogError("ContactPreview: Required managers not found!");
            return;
        }

        int currentDay = dayPartManager.daysElapsed;
        DayPartManager.DayPart currentDayPart = dayPartManager.currentPart;

        Debug.Log($"ContactPreview: Looking for messages from {contactName} on day {currentDay}, {currentDayPart}");
        Debug.Log($"ContactPreview: Total messages in MessageManager: {messageManager.messages.Count}");

        // Debug: Print all messages to see what's available
        for (int i = 0; i < messageManager.messages.Count; i++)
        {
            var msg = messageManager.messages[i];
            Debug.Log($"ContactPreview: Message {i}: Day={msg.day}, Part={msg.triggerPart}, Sender={msg.senderName}, Text={msg.messageText.Substring(0, Mathf.Min(30, msg.messageText.Length))}...");
        }

        // Find messages for this contact on the current day and day part
        List<TimedMessage> contactMessages;
        
        // If current day part is None, look for messages in both Morning and Evening
        if (currentDayPart == DayPartManager.DayPart.None)
        {
            Debug.Log($"ContactPreview: Current day part is None, checking both Morning and Evening messages");
            List<TimedMessage> morningMessages = messageManager.GetMessagesForContact(contactName, currentDay, DayPartManager.DayPart.Morning);
            List<TimedMessage> eveningMessages = messageManager.GetMessagesForContact(contactName, currentDay, DayPartManager.DayPart.Evening);
            
            contactMessages = new List<TimedMessage>();
            contactMessages.AddRange(morningMessages);
            contactMessages.AddRange(eveningMessages);
            
            Debug.Log($"ContactPreview: Found {morningMessages.Count} morning messages and {eveningMessages.Count} evening messages");
        }
        else
        {
            contactMessages = messageManager.GetMessagesForContact(contactName, currentDay, currentDayPart);
        }
        
        Debug.Log($"ContactPreview: Found {contactMessages.Count} total messages for {contactName} on day {currentDay}");

        if (contactMessages.Count > 0)
        {
            // Show the first message as preview
            TimedMessage message = contactMessages[0];
            if (MessagePreview != null)
            {
                // Show first 100 characters of message as preview
                string preview = message.messageText.Length > 100 
                    ? message.messageText.Substring(0, 100) + "..." 
                    : message.messageText;
                MessagePreview.text = preview;
                Debug.Log($"ContactPreview: Set MessagePreview text to: {preview}");
            }
            else
            {
                Debug.LogError("ContactPreview: MessagePreview UI component is null!");
            }
            Debug.Log($"ContactPreview: Found message from {contactName}: {contactMessages[0].messageText.Substring(0, Mathf.Min(50, contactMessages[0].messageText.Length))}...");
        }
        else
        {
            // No message for this contact on current day/part
            if (MessagePreview != null)
            {
                MessagePreview.text = "";
                Debug.Log("ContactPreview: Set MessagePreview text to empty string");
            }
            else
            {
                Debug.LogError("ContactPreview: MessagePreview UI component is null!");
            }
            Debug.Log($"ContactPreview: No messages found for {contactName} on day {currentDay}, {currentDayPart}");
        }
    }

    /// <summary>
    /// Sets the contact name and updates the preview
    /// </summary>
    public void SetContactName(string newContactName)
    {
        contactName = newContactName;
        if (ContactName != null)
        {
            ContactName.text = contactName;
        }
        UpdateMessagePreview();
    }

    /// <summary>
    /// Manually trigger a preview update (for testing)
    /// </summary>
    [ContextMenu("Update Message Preview")]
    public void ManualUpdatePreview()
    {
        UpdateMessagePreview();
    }
    
    /// <summary>
    /// Add a test message for the current contact (for debugging)
    /// </summary>
    [ContextMenu("Add Test Message")]
    public void AddTestMessage()
    {
        if (messageManager == null)
        {
            Debug.LogError("ContactPreview: MessageManager not found!");
            return;
        }
        
        TimedMessage testMsg = new TimedMessage();
        testMsg.day = dayPartManager.daysElapsed;
        testMsg.triggerPart = dayPartManager.currentPart;
        testMsg.senderName = contactName;
        testMsg.messageText = $"This is a test message for {contactName} on day {testMsg.day}, {testMsg.triggerPart}.";
        testMsg.replyOption1 = "Test Reply 1";
        testMsg.replyOption2 = "Test Reply 2";
        
        messageManager.AddMessage(testMsg);
        Debug.Log($"ContactPreview: Added test message for {contactName}");
        
        // Update the preview
        UpdateMessagePreview();
    }
    
    /// <summary>
    /// Force set a test message in the preview (for debugging)
    /// </summary>
    [ContextMenu("Force Test Preview")]
    public void ForceTestPreview()
    {
        if (MessagePreview != null)
        {
            MessagePreview.text = $"Test preview for {contactName} - This should be visible!";
            Debug.Log("ContactPreview: Force set test preview text");
        }
        else
        {
            Debug.LogError("ContactPreview: MessagePreview component is null!");
        }
    }
    
    /// <summary>
    /// Simulate day part change to Morning (for testing)
    /// </summary>
    [ContextMenu("Simulate Morning")]
    public void SimulateMorning()
    {
        Debug.Log("ContactPreview: Simulating Morning day part");
        OnDayPartChanged(DayPartManager.DayPart.Morning);
    }
    
    /// <summary>
    /// Simulate day part change to Evening (for testing)
    /// </summary>
    [ContextMenu("Simulate Evening")]
    public void SimulateEvening()
    {
        Debug.Log("ContactPreview: Simulating Evening day part");
        OnDayPartChanged(DayPartManager.DayPart.Evening);
    }
}
