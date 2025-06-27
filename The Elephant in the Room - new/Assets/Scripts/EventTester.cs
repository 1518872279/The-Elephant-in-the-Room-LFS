using UnityEngine;

public class EventTester : MonoBehaviour
{
    [Tooltip("List of event names defined in TimeManager, order corresponds to number keys 1..n")]  
    public string[] testEvents;

    void Update()
    {
        for (int i = 0; i < testEvents.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.T + i))
            {
                bool started = TimeManager.Instance.TryStartEvent(testEvents[i]);
                if (started)
                    Debug.Log($"[EventTester] Started event '{testEvents[i]}'. Current time: {TimeManager.Instance.GetCurrentTime()} mins since midnight.");
                else
                    Debug.LogWarning($"[EventTester] Failed to start '{testEvents[i]}'. Either undefined or exceeds window.");
            }
        }
    }
} 