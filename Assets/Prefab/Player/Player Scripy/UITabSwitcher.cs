using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UITabSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class UITab
    {
        public string tabName;
        public Button tabButton;
        public GameObject tabPanel;
    }

    [Header("Menu Root")]
    public GameObject menuRoot;
    public bool openMenuWhenHotkeyPressed = true;

    [Header("Tabs")]
    public List<UITab> tabs = new List<UITab>();

    [Header("Tab Indexes")]
    public int inventoryTabIndex = 0;
    public int bestiaryTabIndex = 1;

    [Header("Default")]
    public int defaultTabIndex = 0;
    public bool openDefaultOnStart = true;

    [Header("Hotkeys")]
    public bool useHotkeys = true;
    public bool pressOneTogglesBestiary = true;

    [Header("Button Colors")]
    public bool changeButtonColors = true;
    public Color selectedColor = Color.white;
    public Color unselectedColor = new Color(1f, 1f, 1f, 0.5f);

    private int currentTabIndex = -1;
    private bool buttonsInitialized = false;

    private void Awake()
    {
        InitializeButtons();
    }

    private void Start()
    {
        InitializeButtons();

        if (openDefaultOnStart)
        {
            ShowTab(defaultTabIndex);
        }
    }

    private void OnEnable()
    {
        InitializeButtons();

        if (openDefaultOnStart && currentTabIndex == -1)
        {
            ShowTab(defaultTabIndex);
        }
    }

    private void Update()
    {
        if (!useHotkeys)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        if (pressOneTogglesBestiary && Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            ToggleBestiary();
        }

        if (pressOneTogglesBestiary && Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            ToggleBestiary();
        }
    }

    private void InitializeButtons()
    {
        if (buttonsInitialized)
        {
            return;
        }

        for (int i = 0; i < tabs.Count; i++)
        {
            int index = i;

            if (tabs[index].tabButton != null)
            {
                tabs[index].tabButton.onClick.AddListener(() => ShowTab(index));
                Debug.Log("Tab button connected: " + tabs[index].tabName);
            }
            else
            {
                Debug.LogWarning("Tab button missing at index: " + index);
            }

            if (tabs[index].tabPanel == null)
            {
                Debug.LogWarning("Tab panel missing at index: " + index);
            }
        }

        buttonsInitialized = true;
    }

    public void ToggleBestiary()
    {
        if (openMenuWhenHotkeyPressed && menuRoot != null)
        {
            menuRoot.SetActive(true);
        }

        if (currentTabIndex == bestiaryTabIndex)
        {
            ShowTab(inventoryTabIndex);
        }
        else
        {
            ShowTab(bestiaryTabIndex);
        }
    }

    public void ShowTab(int tabIndex)
    {
        if (tabIndex < 0 || tabIndex >= tabs.Count)
        {
            Debug.LogError("Invalid tab index: " + tabIndex);
            return;
        }

        currentTabIndex = tabIndex;

        for (int i = 0; i < tabs.Count; i++)
        {
            bool isSelected = i == tabIndex;

            if (tabs[i].tabPanel != null)
            {
                tabs[i].tabPanel.SetActive(isSelected);
            }

            if (changeButtonColors && tabs[i].tabButton != null)
            {
                Image buttonImage = tabs[i].tabButton.GetComponent<Image>();

                if (buttonImage != null)
                {
                    buttonImage.color = isSelected ? selectedColor : unselectedColor;
                }
            }
        }

        Debug.Log("Switched to tab: " + tabs[tabIndex].tabName);
    }

    public void ShowInventoryTab()
    {
        ShowTab(inventoryTabIndex);
    }

    public void ShowBestiaryTab()
    {
        ShowTab(bestiaryTabIndex);
    }

    public int GetCurrentTabIndex()
    {
        return currentTabIndex;
    }
}