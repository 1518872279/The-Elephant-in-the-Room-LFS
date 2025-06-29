using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CookingMinigameController : MonoBehaviour
{
    public static CookingMinigameController Instance;

    [Header("Teleport & Lock Points")]
    public Transform teleportPoint;
    public Transform cameraLockPoint;

    [Header("References")]
    public FirstPersonController fpController;
    public Camera playerCamera;
    public Canvas minigameCanvas;
    public Image fadeImage;
    public Transform spawnPoint;

    [Header("Pan Controller (Choose One)")]
    [Tooltip("Physics-based mouse control")]
    public PanController panController;
    [Tooltip("Direct mouse control")]
    public PanControllerDirect panControllerDirect;
    [Tooltip("Keyboard-based control")]
    public PanControllerKeyboard panControllerKeyboard;

    [Header("Ingredient Prefabs & Counts")]
    public GameObject baconPrefab;
    public GameObject eggPrefab;
    public GameObject toastPrefab;
    public int baconCount = 3;
    public int eggCount = 2;
    public int toastCount = 2;

    [Header("Mini‑Game Duration (seconds)")]
    public float gameDuration = 15f;

    private bool isPlaying;
    private Vector3 originalPlayerPos;
    private Quaternion originalPlayerRot;
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;

    void Awake()
    {
        Instance = this;
        minigameCanvas.enabled = false;
        // Ensure pan control is disabled when not in mini-game
        DisableAllPanControllers();
    }

    public void StartMinigame()
    {
        if (!isPlaying)
            StartCoroutine(RunMinigame());
    }

    private IEnumerator RunMinigame()
    {
        isPlaying = true;

        // Save original transforms
        originalPlayerPos = fpController.transform.position;
        originalPlayerRot = fpController.transform.rotation;
        originalCamPos = playerCamera.transform.position;
        originalCamRot = playerCamera.transform.rotation;

        // Teleport & lock controls
        fpController.enabled = false;
        fpController.transform.position = teleportPoint.position;
        fpController.transform.rotation = teleportPoint.rotation;
        playerCamera.transform.position = cameraLockPoint.position;
        playerCamera.transform.rotation = cameraLockPoint.rotation;

        // Show UI & cursor
        minigameCanvas.enabled = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Reset and enable pan control
        ResetPan();
        EnableActivePanController();

        // Spawn Ingredients in fixed sequence
        SpawnIngredients(baconPrefab, baconCount);
        yield return new WaitUntil(() => IsFinishedSpawning());
        SpawnIngredients(eggPrefab, eggCount);
        yield return new WaitUntil(() => IsFinishedSpawning());
        SpawnIngredients(toastPrefab, toastCount);

        // Cooking/shake phase
        float timer = 0f;
        while (timer < gameDuration)
        {
            // Pan control is handled automatically by the Update() method of the active controller
            timer += Time.deltaTime;
            yield return null;
        }

        // Disable pan control
        DisableAllPanControllers();

        // Fade out/in
        yield return StartCoroutine(FadeRoutine());

        // Restore UI & controls
        minigameCanvas.enabled = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Restore transforms
        fpController.transform.position = originalPlayerPos;
        fpController.transform.rotation = originalPlayerRot;
        playerCamera.transform.position = originalCamPos;
        playerCamera.transform.rotation = originalCamRot;

        fpController.enabled = true;
        isPlaying = false;
    }

    private void DisableAllPanControllers()
    {
        if (panController != null) panController.enabled = false;
        if (panControllerDirect != null) panControllerDirect.enabled = false;
        if (panControllerKeyboard != null) panControllerKeyboard.enabled = false;
    }

    private void EnableActivePanController()
    {
        if (panController != null) panController.enabled = true;
        if (panControllerDirect != null) panControllerDirect.enabled = true;
        if (panControllerKeyboard != null) panControllerKeyboard.enabled = true;
    }

    private void ResetPan()
    {
        if (panController != null)
        {
            panController.ResetPan();
        }
        // Note: PanControllerDirect and PanControllerKeyboard don't need explicit reset
        // as they handle their own state in Update()
    }

    private bool IsFinishedSpawning()
    {
        if (panController != null)
            return panController.FinishedSpawning;
        return true; // Direct and Keyboard controllers don't use spawning logic
    }

    private void SpawnIngredients(GameObject prefab, int count)
    {
        if (panController != null)
        {
            panController.FinishedSpawning = false;
        }
        
        for (int i = 0; i < count; i++)
            Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            
        if (panController != null)
        {
            panController.FinishedSpawning = true;
        }
    }

    private IEnumerator FadeRoutine()
    {
        float t = 0f;
        while (t < 1f)
        {
            fadeImage.color = new Color(0, 0, 0, t);
            t += Time.deltaTime;
            yield return null;
        }
        while (t > 0f)
        {
            fadeImage.color = new Color(0, 0, 0, t);
            t -= Time.deltaTime;
            yield return null;
        }
    }
} 