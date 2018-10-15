using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlacableButton : MonoBehaviour//, IPointerDownHandler
{
    public Text Label;
    public Image Icon;
    public GameObject[] Backs;
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
        for (int i = 0; i < Backs.Length; i++)
            Backs[i].SetActive(false);
        if (placable.DataType == PlacableDataType.Tower)
        {
            Backs[0].SetActive(true);
            GetComponent<Button>().targetGraphic = Backs[0].GetComponent<Image>();
        }
        else if (placable.DataType == PlacableDataType.Cat)
        {
            Backs[1].SetActive(true);
            GetComponent<Button>().targetGraphic = Backs[1].GetComponent<Image>();
        }
        else if (placable.DataType == PlacableDataType.Toy)
        {
            Backs[2].SetActive(true);
            GetComponent<Button>().targetGraphic = Backs[2].GetComponent<Image>();
        }
        else if (placable.DataType == PlacableDataType.Treat)
        {
            Backs[3].SetActive(true);
            GetComponent<Button>().targetGraphic = Backs[3].GetComponent<Image>();
        }
    }
    /*
    public void OnPointerDown(PointerEventData eventData)
    {
        PlacementManager.Instance.StartPlacing(Placable);
        PlayerManager.Instance.RemoveInventory(Placable);
    }*/
}