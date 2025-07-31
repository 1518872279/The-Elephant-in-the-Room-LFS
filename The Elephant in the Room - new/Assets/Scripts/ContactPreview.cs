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

        // Initial update
        UpdateMessagePreview();
        
        // Setup button click handler
        if (ButtonToReplyInterface != null)
        {
            ButtonToReplyInterface.onClick.RemoveAllListeners();
            ButtonToReplyInterface.onClick.AddListener(OnReplyButtonClicked);
        }
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

        // Find messages for this contact on the current day and day part
        List<TimedMessage> contactMessages = messageManager.GetMessagesForContact(contactName, currentDay, currentDayPart);

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
            }
            Debug.Log($"ContactPreview: Found message from {contactName}: {contactMessages[0].messageText.Substring(0, Mathf.Min(50, contactMessages[0].messageText.Length))}...");
        }
        else
        {
            // No message for this contact on current day/part
            if (MessagePreview != null)
            {
                MessagePreview.text = "";
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
}
