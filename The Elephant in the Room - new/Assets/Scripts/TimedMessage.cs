using UnityEngine;

[System.Serializable]
public class TimedMessage
{
    public DayPartManager.DayPart triggerPart;  // Morning or Evening
    public int day;  // Which day this message should appear (1, 2, 3, etc.)
    public string senderName;
    [TextArea] public string messageText;
    public string replyOption1;
    public string replyOption2;
} 