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

    void Awake()
    {
        Instance = this;
    }

    /// <summary>Call this when GarbageCleanup event starts.</summary>
    public void StartMinigame()
    {
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
        for (int i = 0; i < count; i++)
        {
            // Pick random range
            Transform rangeT = ranges[Random.Range(0, ranges.Length)];
            var box = rangeT.GetComponent<BoxCollider>();
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
                var go = Instantiate(prefab, spawnPos, Quaternion.identity);
                go.AddComponent<GarbageItem>();
                spawnedItems.Add(go);
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
} 