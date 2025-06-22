using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    private Dictionary<string, float> timeSpent = new Dictionary<string, float>();
    private string currentAction;
    private float actionStartTime;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartAction(string actionName)
    {
        if (!string.IsNullOrEmpty(currentAction)) EndAction();
        currentAction = actionName;
        actionStartTime = Time.time;
    }

    public void EndAction()
    {
        if (string.IsNullOrEmpty(currentAction)) return;
        float duration = Time.time - actionStartTime;
        if (!timeSpent.ContainsKey(currentAction)) timeSpent[currentAction] = 0f;
        timeSpent[currentAction] += duration;
        currentAction = null;
    }

    public float GetTimeSpent(string actionName)
        => timeSpent.TryGetValue(actionName, out var t) ? t : 0f;
} 