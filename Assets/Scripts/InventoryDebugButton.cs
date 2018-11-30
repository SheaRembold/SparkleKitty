using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryDebugButton : MonoBehaviour, IPointerDownHandler
{
    public Text Label;
    public Image Icon;
    PlacableData Placable;

    public void SetPlacable(PlacableData placable)
    {
        Placable = placable;
        Icon.sprite = Placable.Icon;
        UpdateCount();
        PlayerManager.Instance.onInventoryChange += UpdateCount;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Placable.Prefab.GetComponent<ItemController>() != null)
            PlayerManager.Instance.AddItemHealth(Placable, 1f);
        PlayerManager.Instance.AddInventory(Placable);
    }

    void UpdateCount()
    {
        Label.text = PlayerManager.Instance.GetInventoryCount(Placable).ToString();
    }
}