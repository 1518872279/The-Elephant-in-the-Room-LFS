using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item Settings")]
    public Item itemData;
    
    [Header("Visual Feedback")]
    public bool destroyOnPickup = true;
    public GameObject pickupEffect; // Optional particle effect
    
    public void Interact()
    {
        // Add item to inventory
        Inventory.Instance.Add(itemData);
        
        // Optional: Play pickup effect
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, transform.rotation);
        }
        
        // Optional: Play sound
        // AudioManager.Instance.PlaySound("pickup");
        
        // Destroy the object (or disable it)
        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
        
        Debug.Log($"Picked up: {itemData.itemName}");
    }
}
