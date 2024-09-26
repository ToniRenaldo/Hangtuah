using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabsController : MonoBehaviour
{
    // Start is called before the first frame update
    [System.Serializable]
    internal class Tabs
    {
        public Button tabButton;
        public GameObject tabPanel;
    }
    [Header("Tabs")]
    [SerializeField] private List<Tabs> tabs;


    private void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        foreach(var tab in tabs)
        {
            tab.tabButton.onClick.AddListener(() => ShowTab(tab.tabPanel));  
        }
        OpenPanel();
    }

    public void OpenPanel()
    {
        ShowTab(tabs[0].tabPanel);
    }

    private void ShowTab(GameObject tabPanel)
    {
        foreach(var tab in tabs)
        {
            tab.tabButton.interactable = tabPanel != tab.tabPanel;
            tab.tabPanel.SetActive( tab.tabPanel == tabPanel);
        }
    }

}
