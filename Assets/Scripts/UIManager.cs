using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [SerializeField]
    GameObject mainUI;
    [SerializeField]
    SpeechUI speechUI;
    [SerializeField]
    UpgradeUI upgradeUI;

    GameObject currentUI;
    Stack<GameObject> uiStack = new Stack<GameObject>();

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }
    
    public void ResetToMainUI()
    {
        GoBackToUI(mainUI);
    }
    
    public void ShowUI(GameObject uiObj)
    {
        if (uiObj != currentUI)
        {
            if (currentUI != null)
            {
                currentUI.SetActive(false);
                uiStack.Push(currentUI);
            }
            currentUI = uiObj;
            currentUI.SetActive(true);
        }
    }

    public void GoBackToUI(GameObject uiObj)
    {
        if (uiObj != currentUI)
        {
            if (currentUI != null)
                currentUI.SetActive(false);
            while (uiStack.Count > 0 && currentUI != uiObj)
                currentUI = uiStack.Pop();
            currentUI = uiObj;
            currentUI.SetActive(true);
        }
    }

    public void GoBack()
    {
        if (uiStack.Count > 0)
        {
            if (currentUI != null)
                currentUI.SetActive(false);
            currentUI = uiStack.Pop();
            currentUI.SetActive(true);
        }
    }

    public void ShowSpeechUI(Transform target)
    {
        speechUI.ShowSpeech(target);
    }

    public void ShowUpgradeUI(UpgradeClickable upgradable)
    {
        upgradeUI.ShowUpgrade(upgradable);
        ShowUI(upgradeUI.gameObject);
    }
}