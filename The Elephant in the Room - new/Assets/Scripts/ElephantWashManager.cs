using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ElephantWashManager : MonoBehaviour
{
    [Header("Elephant Reference")]
    public GameObject elephantObject;
    
    [Header("Stain System")]
    public GameObject stainPrefab;
    public int stainCount = 20;
    
    [Header("UI Elements")]
    public Slider progressBar;
    public TextMeshProUGUI stainCountText;
    public GameObject washCanvas;
    
    [Header("Camera & Controls")]
    public Transform washCameraPosition;
    public Transform originalCameraPosition;
    public FirstPersonController playerController;
    
    [Header("Water Gun")]
    public GameObject waterGun;
    public WaterGunController waterGunController;
    
    [Header("Audio")]
    public AudioSource washAudioSource;
    public AudioClip washStartSound;
    public AudioClip washCompleteSound;
    
    [Header("Integration")]
    public string washEventName = "ElephantWash";
    public int washDuration = 5;
    
    [Header("Fade Transition")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    private Mesh elephantMesh;
    private MeshCollider meshCollider;
    private MeshFilter meshFilter;
    private int remainingStains;
    private bool isWashActive = false;
    private List<GameObject> activeStains = new List<GameObject>();
    
    public System.Action OnWashStarted;
    public System.Action OnWashCompleted;
    public System.Action<int> OnStainCleaned;

    void Awake()
    {
        if (elephantObject != null)
        {
            meshCollider = elephantObject.GetComponent<MeshCollider>();
            meshFilter = elephantObject.GetComponent<MeshFilter>();
            
            if (meshFilter != null)
            {
                elephantMesh = meshFilter.mesh;
            }
        }
        
        if (washCanvas != null)
            washCanvas.SetActive(false);
            
        if (waterGun != null)
            waterGun.SetActive(false);
    }

    public void StartWash()
    {
        if (isWashActive) return;
        
        isWashActive = true;
        remainingStains = stainCount;
        
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.TryStartEvent(washEventName);
        }
        
        if (washAudioSource != null && washStartSound != null)
        {
            washAudioSource.PlayOneShot(washStartSound);
        }
        
        StartCoroutine(SwitchToWashView());
        SpawnStains();
        UpdateUI();
        
        OnWashStarted?.Invoke();
    }

    private void SpawnStains()
    {
        ClearExistingStains();
        
        for (int i = 0; i < stainCount; i++)
        {
            Vector3 worldPos;
            
            if (elephantMesh != null && elephantObject != null)
            {
                worldPos = RandomPointOnMeshSurface(elephantMesh, elephantObject.transform.localToWorldMatrix);
            }
            else
            {
                worldPos = GetFallbackStainPosition();
            }
            
            GameObject stain = Instantiate(stainPrefab, worldPos, Quaternion.identity, transform);
            
            if (meshCollider != null && meshCollider.sharedMesh != null)
            {
                stain.transform.rotation = Quaternion.LookRotation(meshCollider.sharedMesh.normals[0]);
            }
            else
            {
                stain.transform.rotation = Quaternion.LookRotation((worldPos - elephantObject.transform.position).normalized);
            }
            
            Stain stainComponent = stain.GetComponent<Stain>();
            if (stainComponent != null)
            {
                stainComponent.onCleaned.AddListener(HandleStainCleaned);
            }
            
            activeStains.Add(stain);
        }
    }
    
    private Vector3 GetFallbackStainPosition()
    {
        if (elephantObject == null)
        {
            return transform.position;
        }
        
        Renderer renderer = elephantObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;
            Vector3 randomPoint = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
            return randomPoint;
        }
        
        Vector3 randomOffset = Random.onUnitSphere * 2f;
        return elephantObject.transform.position + randomOffset;
    }

    private void HandleStainCleaned()
    {
        remainingStains--;
        UpdateUI();
        OnStainCleaned?.Invoke(remainingStains);
        
        if (remainingStains <= 0)
        {
            EndWash();
        }
    }

    public void EndWash()
    {
        if (!isWashActive) return;
        
        isWashActive = false;
        
        if (washAudioSource != null && washCompleteSound != null)
        {
            washAudioSource.PlayOneShot(washCompleteSound);
        }
        
        ClearExistingStains();
        StartCoroutine(EndWashWithFade());
        
        if (ElephantStateController.Instance != null)
        {
            ElephantStateController.Instance.OnEventCompleted("ElephantWash");
        }
        
        OnWashCompleted?.Invoke();
    }

    private IEnumerator SwitchToWashView()
    {
        if (waterGun != null)
        {
            waterGun.SetActive(true);
        }
        
        if (washCanvas != null)
        {
            washCanvas.SetActive(true);
        }
        
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator EndWashWithFade()
    {
        // Ensure fade image is available
        if (fadeImage == null)
        {
            Debug.LogError("ElephantWashManager: No fade image assigned!");
            yield break;
        }
        
        // Ensure the canvas is enabled
        Canvas canvas = fadeImage.GetComponentInParent<Canvas>();
        if (canvas != null && !canvas.enabled)
        {
            Debug.Log("ElephantWashManager: Enabling transition canvas");
            canvas.enabled = true;
        }
        
        // Fade out
        float fadeOutTime = 0f;
        while (fadeOutTime < fadeDuration)
        {
            fadeOutTime += Time.deltaTime;
            float alpha = fadeOutTime / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        
        // Switch to normal view
        if (waterGun != null)
        {
            waterGun.SetActive(false);
        }
        
        if (washCanvas != null)
        {
            washCanvas.SetActive(false);
        }
        
        // Fade in
        float fadeInTime = fadeDuration;
        while (fadeInTime > 0f)
        {
            fadeInTime -= Time.deltaTime;
            float alpha = fadeInTime / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        
        yield return new WaitForSeconds(0.5f);
    }

    private void UpdateUI()
    {
        if (progressBar != null)
        {
            progressBar.value = 1f - ((float)remainingStains / stainCount);
        }
        
        if (stainCountText != null)
        {
            stainCountText.text = $"Stains Left: {remainingStains}";
        }
    }

    private void ClearExistingStains()
    {
        foreach (GameObject stain in activeStains)
        {
            if (stain != null)
            {
                Destroy(stain);
            }
        }
        activeStains.Clear();
    }

    private Vector3 RandomPointOnMeshSurface(Mesh mesh, Matrix4x4 localToWorld)
    {
        if (mesh == null)
        {
            return transform.position;
        }
        
        var tris = mesh.triangles;
        var verts = mesh.vertices;
        
        if (tris == null || tris.Length == 0 || verts == null || verts.Length == 0)
        {
            return transform.position;
        }
        
        if (tris.Length % 3 != 0)
        {
            return transform.position;
        }
        
        int triangleCount = tris.Length / 3;
        if (triangleCount == 0)
        {
            return transform.position;
        }
        
        float[] cumAreas = new float[triangleCount];
        float total = 0;
        
        for (int i = 0; i < triangleCount; i++)
        {
            int baseIndex = i * 3;
            
            if (baseIndex + 2 >= tris.Length)
            {
                return transform.position;
            }
            
            int v0Index = tris[baseIndex];
            int v1Index = tris[baseIndex + 1];
            int v2Index = tris[baseIndex + 2];
            
            if (v0Index >= verts.Length || v1Index >= verts.Length || v2Index >= verts.Length)
            {
                return transform.position;
            }
            
            Vector3 a = verts[v0Index];
            Vector3 b = verts[v1Index];
            Vector3 c = verts[v2Index];
            
            float area = Vector3.Cross(b - a, c - a).magnitude * 0.5f;
            total += area;
            cumAreas[i] = total;
        }
        
        if (total <= 0)
        {
            return transform.position;
        }
        
        float r = Random.value * total;
        int triIndex = System.Array.FindIndex(cumAreas, area => area >= r);
        
        if (triIndex < 0 || triIndex >= triangleCount)
        {
            triIndex = Random.Range(0, triangleCount);
        }
        
        int finalBaseIndex = triIndex * 3;
        Vector3 v0 = verts[tris[finalBaseIndex]];
        Vector3 v1 = verts[tris[finalBaseIndex + 1]];
        Vector3 v2 = verts[tris[finalBaseIndex + 2]];
        
        float u = Random.value;
        float v = Random.value;
        if (u + v > 1) 
        { 
            u = 1 - u; 
            v = 1 - v; 
        }
        
        Vector3 point = v0 + u * (v1 - v0) + v * (v2 - v0);
        return localToWorld.MultiplyPoint(point);
    }
} 