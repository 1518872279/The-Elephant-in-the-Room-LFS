using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Canvas))]
public class PhoneUIManager : MonoBehaviour
{
    [Header("Phone Panel")]
    [Tooltip("Root GameObject for the entire phone UI")]
    public GameObject phonePanel;

    [Header("Phone Pages")]
    [Tooltip("Individual sub-panels/screens (e.g. Home, ShopSecondary, MessagesSecondary, MessageTertiary, ShopTertiary)")]
    public List<GameObject> pages;

    [Header("Player Control")]
    [Tooltip("Reference to the FirstPersonController to pause movement")]
    public FirstPersonController playerController;

    // Internal index of the currently active page
    private int currentPage = -1;

    void Start()
    {
        phonePanel.SetActive(false);
        HideAllPages();

        if (playerController == null)
            playerController = FindObjectOfType<FirstPersonController>();

        EnsureEventSystem();
    }
    
    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogWarning("PhoneUIManager: No EventSystem found! Creating one...");
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }

    /// <summary>
    /// Opens the phone UI, resets to the home page (index 0),
    /// pauses player control, and shows the cursor for UI interaction.
    /// </summary>
    public void OpenPhone()
    {
        // Activate the panel
        phonePanel.SetActive(true);

        // Reset to home page - directly show page 0 without deactivating
        for (int i = 0; i < pages.Count; i++)
            pages[i].SetActive(i == 0);
        currentPage = 0;

        // Pause player movement
        if (playerController != null)
            playerController.enabled = false;

        // Show cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        EnsureEventSystem();
        Debug.Log("Phone opened and reset to home page");
    }

    /// <summary>
    /// Closes the phone UI and resumes gameplay.
    /// </summary>
    public void ClosePhone()
    {
        phonePanel.SetActive(false);
        HideAllPages();

        if (playerController != null)
            playerController.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Switches to the specified page index, deactivating the previous one.
    /// </summary>
    public void ShowPage(int index)
    {
        if (index < 0 || index >= pages.Count) return;

        // Deactivate the current page if valid
        if (currentPage >= 0 && currentPage < pages.Count)
            pages[currentPage].SetActive(false);

        // Activate the new page
        pages[index].SetActive(true);
        currentPage = index;
    }

    private void HideAllPages()
    {
        for (int i = 0; i < pages.Count; i++)
            pages[i].SetActive(false);
        currentPage = -1;
    }

    // Named helper methods for use in button OnClick() events:
    public void ShowHome()              => ShowPage(0);
    public void ShowShopSecondary()     => ShowPage(1);
    public void ShowMessagesSecondary() => ShowPage(2);
    public void ShowMessageTertiory()   => ShowPage(3);
    public void ShowShopTertiory()      => ShowPage(4);

} 