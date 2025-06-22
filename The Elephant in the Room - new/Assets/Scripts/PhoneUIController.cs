using UnityEngine;

public class PhoneUIController : MonoBehaviour
{
    public GameObject phonePanel;

    void Start()
    {
        phonePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            phonePanel.SetActive(!phonePanel.activeSelf);
    }
} 