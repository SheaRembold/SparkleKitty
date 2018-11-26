using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButttonAudio : MonoBehaviour, IPointerClickHandler
{
    Selectable button;

    void Awake()
    {
        button = GetComponent<Selectable>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (button.interactable)
            SoundManager.Instance.SimpleButtonClick();
    }
}
