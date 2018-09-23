using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HTC.UnityPlugin.Pointer3D;

public class UIManager : MonoBehaviour
{
    //public static UIManager Instance;

    public GameObject flashPrefab;

    [SerializeField]
    GameObject mainUI;
    //[SerializeField]
    //SpeechUI speechUI;

    GameObject currentUI;
    Stack<GameObject> uiStack = new Stack<GameObject>();

    GraphicRaycaster graphicRaycaster;
    CanvasRaycastTarget canvasRaycastTarget;

    private void Awake()
    {
        //Instance = this;

        graphicRaycaster = GetComponentInChildren<GraphicRaycaster>();
        canvasRaycastTarget = GetComponentInChildren<CanvasRaycastTarget>();
        if (PlacementManager.Instance.IsUsingSteamVR)
        {
            graphicRaycaster.enabled = false;
            canvasRaycastTarget.enabled = true;
        }
        else
        {
            canvasRaycastTarget.enabled = false;
            graphicRaycaster.enabled = true;
        }

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
        mainUI.SetActive(true);
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
    
    /*public void ShowSpeechUI(Transform target, string speech, bool isOnscreen)
    {
        speechUI.ShowSpeech(target, speech, isOnscreen);
        Selectable[] selectables = currentUI.GetComponentsInChildren<Selectable>();
        for (int i = 0; i < selectables.Length; i++)
        {
            selectables[i].interactable = false;
        }
    }

    public void HideSpeechUI()
    {
        Selectable[] selectables = currentUI.GetComponentsInChildren<Selectable>();
        for (int i = 0; i < selectables.Length; i++)
        {
            selectables[i].interactable = true;
        }
    }

    public BuildUI GetBuildUI(string name)
    {
        Transform uiObj = transform.Find(name);
        if (uiObj == null)
            return null;
        return uiObj.GetComponent<BuildUI>();
    }

    public BuildUI GetCurrentBuildUI()
    {
        return currentUI.GetComponent<BuildUI>();
    }*/
}