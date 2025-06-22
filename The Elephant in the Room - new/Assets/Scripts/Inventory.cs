using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public event Action OnChanged;
    private List<Item> items = new List<Item>();
    public IReadOnlyList<Item> Items => items;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Add(Item item)
    {
        items.Add(item);
        OnChanged?.Invoke();
    }

    public void Remove(Item item)
    {
        if (items.Remove(item)) OnChanged?.Invoke();
    }
} 