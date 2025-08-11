using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GarbageCleanupController : MonoBehaviour
{
    public static GarbageCleanupController Instance;

    [Header("Range Parents (with BoxColliders)")]
    public Transform[] stainRanges;
    public Transform[] trashRanges;

    [Header("Garbage Variations & Counts")]
    public GameObject[] stainPrefabs;
    public GameObject[] trashPrefabs;
    public int stainCount = 10;
    public int trashCount = 8;

    [Header("Spawn Settings")]
    public LayerMask floorLayer;
    public float verticalOffset = 0.01f;

    [Header("Debug UI")]
    public Text debugText;

    [Header("End Fade Image")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    private List<GameObject> spawnedItems = new List<GameObject>();
    private int totalItems;
    private int cleanedItems;

    // Day tracking for once-per-day restriction
    private int lastCompletedDay = -1;

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        // Subscribe to day changes to reset the once-per-day restriction
        if (DayPartManager.Instance != null)
        {
            DayPartManager.Instance.OnDayPartChanged += OnDayPartChanged;
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from day changes
        if (DayPartManager.Instance != null)
        {
            DayPartManager.Instance.OnDayPartChanged -= OnDayPartChanged;
        }
    }
    
    private void OnDayPartChanged(DayPartManager.DayPart newPart)
    {
        // Reset the restriction when a new day starts (morning)
        if (newPart == DayPartManager.DayPart.Morning)
        {
            lastCompletedDay = -1;
            Debug.Log("GarbageCleanupController: New day started, garbage cleanup minigame is now available again");
        }
    }

    /// <summary>Call this when GarbageCleanup event starts.</summary>
    public void StartMinigame()
    {
        // Check if already completed today
        if (DayPartManager.Instance != null && lastCompletedDay == DayPartManager.Instance.daysElapsed)
        {
            Debug.Log("GarbageCleanupController: Garbage cleanup minigame already completed today. Try again tomorrow!");
            return;
        }
        
        GenerateGarbage();
        cleanedItems = 0;
        totalItems = spawnedItems.Count;
        UpdateDebugText();
    }

    void GenerateGarbage()
    {
        // Clear previous
        foreach (var go in spawnedItems) Destroy(go);
        spawnedItems.Clear();

        // Spawn stains and trash
        SpawnVariations(stainRanges, stainPrefabs, stainCount);
        SpawnVariations(trashRanges, trashPrefabs, trashCount);

        // Set totals
        totalItems = spawnedItems.Count;
    }

    void SpawnVariations(Transform[] ranges, GameObject[] prefabs, int count)
    {
        int rangesCount = ranges.Length;
        if (rangesCount == 0) return;
        int baseCount = count / rangesCount;
        int remainder = count % rangesCount;

        for (int i = 0; i < rangesCount; i++)
        {
            int spawnCount = baseCount + (i < remainder ? 1 : 0);
            var box = ranges[i].GetComponent<BoxCollider>();
            for (int j = 0; j < spawnCount; j++)
            {
                // Pick random point within this specific range
                Vector3 randomPoint = new Vector3(
                    Random.Range(box.bounds.min.x, box.bounds.max.x),
                    box.bounds.max.y + 1f,
                    Random.Range(box.bounds.min.z, box.bounds.max.z)
                );
                // Raycast down to floor
                if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, Mathf.Infinity, floorLayer))
                {
                    Vector3 spawnPos = hit.point + Vector3.up * verticalOffset;
                    // Select random prefab variation
                    GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
                var go = Instantiate(prefab, spawnPos, prefab.transform.rotation);
                
                // Preserve the original scale to prevent stretching
                go.transform.localScale = prefab.transform.localScale;
                
                    go.AddComponent<GarbageItem>();
                    spawnedItems.Add(go);
                }
            }
        }
    }

    public void ItemCleaned()
    {
        cleanedItems++;
        UpdateDebugText();
        if (cleanedItems >= totalItems)
        {
            StartCoroutine(EndRoutine());
        }
    }

    void UpdateDebugText()
    {
        if (debugText != null)
            debugText.text = $"Cleaned: {cleanedItems} / {totalItems}";
        else
            Debug.Log($"Cleaned: {cleanedItems} / {totalItems}");
    }

    private IEnumerator EndRoutine()
    {
        // Mark as completed for today
        if (DayPartManager.Instance != null)
        {
            lastCompletedDay = DayPartManager.Instance.daysElapsed;
            Debug.Log($"GarbageCleanupController: Garbage cleanup minigame completed for day {lastCompletedDay}");
        }
        
        // Ensure fade image is available and canvas is enabled
        if (fadeImage != null)
        {
            Canvas fadeCanvas = fadeImage.GetComponentInParent<Canvas>();
            if (fadeCanvas != null && !fadeCanvas.enabled)
            {
                fadeCanvas.enabled = true;
            }
            
            // Fade to black
            float t = 0f;
            while (t < fadeDuration)
            {
                fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
                t += Time.deltaTime;
                yield return null;
            }
            // Fade back in
            t = fadeDuration;
            while (t > 0f)
            {
                fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
                t -= Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            // Fallback: just wait a moment if no fade image
            yield return new WaitForSeconds(fadeDuration);
        }
        
        // End of minigame logic, e.g., advance time
        TimeManager.Instance.TryStartEvent("GarbageCleanup");
    }
    
    /// <summary>
    /// Check if the garbage cleanup minigame is available (not completed today)
    /// </summary>
    public bool IsMinigameAvailable()
    {
        if (DayPartManager.Instance == null) return true;
        return lastCompletedDay != DayPartManager.Instance.daysElapsed;
    }
    
    /// <summary>
    /// Get the current day number
    /// </summary>
    public int GetCurrentDay()
    {
        return DayPartManager.Instance != null ? DayPartManager.Instance.daysElapsed : 1;
    }
    
    /// <summary>
    /// Get the last day the garbage cleanup was completed
    /// </summary>
    public int GetLastCompletedDay()
    {
        return lastCompletedDay;
    }
} 