using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject panel;
    public Transform itemsParent;
    public GameObject slotPrefab;

    void Start()
    {
        Inventory.Instance.OnChanged += RefreshUI;
        panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            panel.SetActive(!panel.activeSelf);
    }

    void RefreshUI()
    {
        foreach (Transform t in itemsParent) Destroy(t.gameObject);
        foreach (var item in Inventory.Instance.Items)
        {
            var slot = Instantiate(slotPrefab, itemsParent);
            slot.GetComponent<Image>().sprite = item.icon;
        }
    }
} 