using UnityEngine;

[System.Serializable]
public class TimedMessage
{
    public DayPartManager.DayPart triggerPart;  // Morning or Evening
    public string senderName;
    [TextArea] public string messageText;
    public string replyOption1;
    public string replyOption2;
} 