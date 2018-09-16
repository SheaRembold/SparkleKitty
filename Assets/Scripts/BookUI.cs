using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookUI : MonoBehaviour
{
    [System.Serializable]
    public class TabData
    {
        public Toggle Tab;
        public ListPageUI ListPage;
        public ItemPageUI ItemPage;
        public PlacableDataType PlacableDataType;
    }

    [SerializeField]
    UIManager uiManager;
    [SerializeField]
    TabData[] tabs;

    int current;

    List<GameObject> requs = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].ItemPage.Book = this;
            tabs[i].Tab.onValueChanged.AddListener(OnToggleTab);
        }
        OnToggleTab(true);
    }

    public void OnToggleTab(bool value)
    {
        if (value)
        {
            for (int i=0;i<tabs.Length;i++)
            {
                if (tabs[i].Tab.isOn)
                {
                    uiManager.ShowUI(tabs[i].ListPage.gameObject);
                    break;
                }
            }
        }
    }

    public void ShowItem(PlacableData item)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i].PlacableDataType == item.DataType)
            {
                tabs[i].ItemPage.SetItem(item);
                uiManager.ShowUI(tabs[i].ItemPage.gameObject);
                break;
            }
        }
    }
}