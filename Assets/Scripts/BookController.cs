using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookController : Clickable
{
    GraphicRaycaster graphicRaycaster;

    private void Awake()
    {
        graphicRaycaster = GetComponentInChildren<GraphicRaycaster>();
    }

    public override void Click(RaycastHit hit)
    {
        graphicRaycaster.enabled = false;
        PlacementManager.Instance.GrabBook(this);
    }

    public void PlaceBook()
    {
        graphicRaycaster.enabled = true;
    }
}