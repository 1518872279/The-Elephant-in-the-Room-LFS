using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance { get; private set; }

    [Header("Configured Messages")]
    public List<TimedMessage> messages;

    [Header("UI References")]
    public GameObject messagePanel;    // The pop-up panel
    public TextMeshProUGUI senderText;            // UI Text for sender name
    public TextMeshProUGUI messageContentText;    // UI Text for message body
    public Button replyButton1;        // Button for first reply choice
    public Button replyButton2;        // Button for second reply choice
    
    [Header("Reply Display")]
    public TextMeshProUGUI chosenReplyText;    // UI Text to show the chosen reply
    public GameObject replyOptionsPanel;        // Panel containing the reply buttons

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
        replyButton1.GetComponentInChildren<TextMeshProUGUI>().text = msg.replyOption1;
        replyButton2.GetComponentInChildren<TextMeshProUGUI>().text = msg.replyOption2;

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
} 