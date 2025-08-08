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
    
    [Header("Pre-Interaction")]
    [Tooltip("Game object to deactivate at start of game (before player picks up this item)")]
    public GameObject objectToDeactivate;
    
    void Start()
    {
        // Deactivate the specified object at start of game
        if (objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(false);
            Debug.Log($"Deactivated at start: {objectToDeactivate.name} (item not picked up yet)");
        }
    }
    
    public void Interact()
    {
        // Activate the specified object when item is picked up
        if (objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(true);
            Debug.Log($"Activated after pickup: {objectToDeactivate.name}");
        }
        
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
