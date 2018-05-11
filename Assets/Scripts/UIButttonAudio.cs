using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButttonAudio : MonoBehaviour, IPointerClickHandler
{
    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (button.interactable)
            SoundManager.Instance.SimpleButtonClick();
    }
}
