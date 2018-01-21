using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlacableButton : MonoBehaviour, IPointerDownHandler
{
    public Text Label;
    PlacableData Placable;

    public void SetPlacable(PlacableData placable)
    {
        Placable = placable;
        Label.text = Placable.Name;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PlacementManager.Instance.StartPlacing(Placable);
    }
}