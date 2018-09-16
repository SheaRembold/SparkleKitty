using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HTC.UnityPlugin.Pointer3D;

public class BookController : Clickable
{
    GraphicRaycaster graphicRaycaster;
    CanvasRaycastTarget canvasRaycastTarget;

    private void Awake()
    {
        graphicRaycaster = GetComponentInChildren<GraphicRaycaster>();
        canvasRaycastTarget = GetComponentInChildren<CanvasRaycastTarget>();
    }

    public override void Click(RaycastHit hit)
    {
        if (PlacementManager.Instance.UseSteamVR)
            canvasRaycastTarget.enabled = false;
        else
            graphicRaycaster.enabled = false;
        PlacementManager.Instance.GrabBook(this);
    }

    public void PlaceBook()
    {
        if (PlacementManager.Instance.UseSteamVR)
            canvasRaycastTarget.enabled = true;
        else
            graphicRaycaster.enabled = true;
    }
}