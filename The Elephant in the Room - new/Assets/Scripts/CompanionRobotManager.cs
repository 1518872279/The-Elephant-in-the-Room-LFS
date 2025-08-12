using UnityEngine;

public class CompanionRobotManager : MonoBehaviour
{
    public static CompanionRobotManager Instance { get; private set; }

    [Header("Robot Settings")]
    [Tooltip("Prefab of the companion robot to spawn")]
    public GameObject companionRobotPrefab;
    
    [Tooltip("Transform where the robot will be spawned")]
    public Transform spawnPosition;
    
    [Header("Elephant Effects")]
    [Tooltip("Stability increase when robot is spawned")]
    public float stabilityIncrease = 15f;
    
    [Tooltip("Happiness decrease when robot is spawned")]
    public float happinessDecrease = 10f;
    
    [Header("Animation")]
    [Tooltip("Animator component of the elephant")]
    public Animator elephantAnimator;
    
    private GameObject currentRobot;
    private bool robotSpawned = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Spawn the companion robot and apply effects to elephant
    /// </summary>
    public void SpawnCompanionRobot()
    {
        if (robotSpawned)
        {
            Debug.Log("CompanionRobotManager: Robot already spawned!");
            return;
        }

        if (companionRobotPrefab == null)
        {
            Debug.LogError("CompanionRobotManager: No companion robot prefab assigned!");
            return;
        }

        if (spawnPosition == null)
        {
            Debug.LogError("CompanionRobotManager: No spawn position assigned!");
            return;
        }

        // Spawn the robot
        currentRobot = Instantiate(companionRobotPrefab, spawnPosition.position, spawnPosition.rotation);
        robotSpawned = true;

        // Apply effects to elephant
        ApplyElephantEffects();

        Debug.Log("CompanionRobotManager: Companion robot spawned successfully!");
    }

    /// <summary>
    /// Apply stability increase and happiness decrease to elephant
    /// </summary>
    private void ApplyElephantEffects()
    {
        if (ElephantStateController.Instance != null)
        {
            // Increase stability
            ElephantStateController.Instance.stability += stabilityIncrease;
            
            // Decrease happiness
            ElephantStateController.Instance.happiness -= happinessDecrease;
            
            // Clamp values
            ElephantStateController.Instance.happiness = Mathf.Clamp(ElephantStateController.Instance.happiness, 0f, 100f);
            ElephantStateController.Instance.stability = Mathf.Max(0f, ElephantStateController.Instance.stability);
            
            Debug.Log($"CompanionRobotManager: Applied effects - Stability: +{stabilityIncrease}, Happiness: -{happinessDecrease}");
        }
        else
        {
            Debug.LogWarning("CompanionRobotManager: ElephantStateController not found!");
        }

        // Trigger sad animation
        TriggerSadAnimation();
    }

    /// <summary>
    /// Trigger the "isSad" animation on the elephant
    /// </summary>
    private void TriggerSadAnimation()
    {
        if (elephantAnimator != null)
        {
            elephantAnimator.SetTrigger("isSad");
            Debug.Log("CompanionRobotManager: Triggered 'isSad' animation on elephant");
        }
        else
        {
            Debug.LogWarning("CompanionRobotManager: Elephant animator not assigned!");
        }
    }

    /// <summary>
    /// Check if robot is currently spawned
    /// </summary>
    public bool IsRobotSpawned()
    {
        return robotSpawned;
    }

    /// <summary>
    /// Remove the current robot (if needed)
    /// </summary>
    public void RemoveRobot()
    {
        if (currentRobot != null)
        {
            Destroy(currentRobot);
            currentRobot = null;
            robotSpawned = false;
            Debug.Log("CompanionRobotManager: Robot removed");
        }
    }

    /// <summary>
    /// Test method to spawn robot (for debugging)
    /// </summary>
    [ContextMenu("Spawn Companion Robot")]
    public void TestSpawnRobot()
    {
        SpawnCompanionRobot();
    }
} 