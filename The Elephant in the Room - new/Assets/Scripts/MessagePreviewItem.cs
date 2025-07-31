using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MessagePreviewItem : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI senderNameText;
    public TextMeshProUGUI messagePreviewText;
    public Button selectButton;
    
    private TimedMessage messageData;
    private Action<TimedMessage> onMessageSelected;
    
    public void SetupPreview(TimedMessage message, Action<TimedMessage> callback)
    {
        messageData = message;
        onMessageSelected = callback;
        
        // Populate UI
        if (senderNameText != null)
            senderNameText.text = message.senderName;
            
        if (messagePreviewText != null)
        {
            // Show first 50 characters of message as preview
            string preview = message.messageText.Length > 50 
                ? message.messageText.Substring(0, 50) + "..." 
                : message.messageText;
            messagePreviewText.text = preview;
        }
        
        // Setup button click
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnPreviewItemClicked);
        }
    }
    
    private void OnPreviewItemClicked()
    {
        onMessageSelected?.Invoke(messageData);
    }
} 