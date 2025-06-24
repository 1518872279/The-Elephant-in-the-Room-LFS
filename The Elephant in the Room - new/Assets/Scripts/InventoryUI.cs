using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class InventoryUI : MonoBehaviour
{
    [Header("Hotbar Slots (Bottom HUD)")]
    [Tooltip("Assign six slot UI Images in order: slots 1-6 from left to right")]  
    public Image[] hotbarSlots;    // length = 6

    [Header("Fixed Items (Slots 1-3)")]
    [Tooltip("Assign the Phone, Wallet, and Watch ScriptableItems here in order")]  
    public List<Item> fixedItems;  // must contain exactly 3 items: Phone, Wallet, Watch

    void Start()
    {
        Inventory.Instance.OnChanged += RefreshUI;
        // initially hide all slot icons
        ClearAllSlots();
    }

    void RefreshUI()
    {
        ClearAllSlots();

        // 1. Place fixed items in slots 1-3 if owned
        for (int i = 0; i < fixedItems.Count && i < hotbarSlots.Length; i++)
        {
            if (Inventory.Instance.Items.Contains(fixedItems[i]))
            {
                hotbarSlots[i].sprite = fixedItems[i].icon;
                hotbarSlots[i].enabled = true;
            }
        }

        // 2. Place remaining items in FILO order into slots 4-6
        List<Item> dynamicItems = new List<Item>();
        foreach (var item in Inventory.Instance.Items)
        {
            if (!fixedItems.Contains(item))
                dynamicItems.Add(item);
        }
        
        // FILO: most recently added item first
        dynamicItems.Reverse();
        int startIndex = fixedItems.Count;
        for (int j = 0; j < dynamicItems.Count && (startIndex + j) < hotbarSlots.Length; j++)
        {
            hotbarSlots[startIndex + j].sprite = dynamicItems[j].icon;
            hotbarSlots[startIndex + j].enabled = true;
        }
    }

    void ClearAllSlots()
    {
        foreach (var slot in hotbarSlots)
        {
            slot.sprite = null;
            slot.enabled = false;
        }
    }

    void Update()
    {
        // Optionally, handle hotkey selection: 1-6
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                // TODO: implement use of that slot's item
            }
        }
    }
} 