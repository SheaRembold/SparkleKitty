using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlacableButton : MonoBehaviour//, IPointerDownHandler
{
    public Text Label;
    public Image Icon;
    PlacableData Placable;

    public void SetPlacable(PlacableData placable)
    {
        Placable = placable;
        if (Placable.Icon != null)
        {
            Label.gameObject.SetActive(false);
            Icon.gameObject.SetActive(true);
            Icon.sprite = Placable.Icon;
        }
        else
        {
            Icon.gameObject.SetActive(false);
            Label.gameObject.SetActive(true);
            Label.text = Placable.Name;
        }
    }
    /*
    public void OnPointerDown(PointerEventData eventData)
    {
        PlacementManager.Instance.StartPlacing(Placable);
        PlayerManager.Instance.RemoveInventory(Placable);
    }*/
}