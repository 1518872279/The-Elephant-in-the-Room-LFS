using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public Item itemData;

    public void Interact()
    {
        Inventory.Instance.Add(itemData);
        Destroy(gameObject);
    }
}
