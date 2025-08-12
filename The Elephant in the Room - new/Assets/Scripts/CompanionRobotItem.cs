using UnityEngine;

/// <summary>
/// Component for the companion robot that handles it as a purchasable goods item
/// </summary>
public class CompanionRobotItem : MonoBehaviour
{
    [Header("Goods Data")]
    public Goods goodsData;
    
    [Header("Interaction")]
    [Tooltip("Whether the player can interact with this robot")]
    public bool isInteractable = true;
    
    // Events
    public System.Action<CompanionRobotItem> OnRobotInteracted;
    public System.Action<CompanionRobotItem> OnRobotCollected;

    void Start()
    {
        // Set up the goods data if not already set
        if (goodsData == null)
        {
            goodsData = new Goods("Companion Robot", 
                "A friendly companion robot that helps increase elephant stability but may affect happiness. Press 6 to spawn after purchase.", 
                "Companion robot for elephant stability.", 
                25);
        }
    }

    /// <summary>
    /// Set the goods data for this robot
    /// </summary>
    public void SetGoods(Goods goods)
    {
        goodsData = goods;
        Debug.Log($"CompanionRobotItem: Set goods data for '{goods.goodsName}'");
    }

    /// <summary>
    /// Handle interaction with the robot
    /// </summary>
    public void Interact()
    {
        if (!isInteractable) return;
        
        Debug.Log("CompanionRobotItem: Robot interacted with");
        OnRobotInteracted?.Invoke(this);
        
        // You can add additional interaction logic here
        // For example, playing a sound, showing UI, etc.
    }

    /// <summary>
    /// Collect the robot (remove from world)
    /// </summary>
    public void Collect()
    {
        Debug.Log("CompanionRobotItem: Robot collected");
        OnRobotCollected?.Invoke(this);
        
        // Destroy the robot object
        Destroy(gameObject);
    }
} 