﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HTC.UnityPlugin.Pointer3D;

public class BookController : Clickable
{
    public static BookController Instance;

    [SerializeField]
    GameObject uiRoot;
    [SerializeField]
    GameObject helpParticle;

    GraphicRaycaster graphicRaycaster;
    CanvasRaycastTarget canvasRaycastTarget;

    private void Awake()
    {
        Instance = this;

        graphicRaycaster = uiRoot.GetComponent<GraphicRaycaster>();
        canvasRaycastTarget = uiRoot.GetComponent<CanvasRaycastTarget>();

        if (HelpManager.Instance.CurrentStep <= TutorialStep.GrabBook)
            CloseBook();

        if (HelpManager.Instance.CurrentStep == TutorialStep.GrabBook)
        {
            helpParticle.SetActive(true);
        }

        HelpManager.Instance.onCompleteTutorialStep += OnCompleteTutorialStep;
    }

    void OnCompleteTutorialStep(TutorialStep currentStep)
    {
        if (HelpManager.Instance.CurrentStep == TutorialStep.GrabBook)
        {
            helpParticle.SetActive(true);
        }
    }

    public void CloseBook()
    {
        uiRoot.SetActive(false);
        if (PlacementManager.Instance.IsUsingSteamVR)
            canvasRaycastTarget.enabled = false;
        else
            graphicRaycaster.enabled = false;
    }

    public void OpenBook()
    {
        uiRoot.SetActive(true);
        if (PlacementManager.Instance.IsUsingSteamVR)
            canvasRaycastTarget.enabled = true;
        else
            graphicRaycaster.enabled = true;
    }

    public override void Click(RaycastHit hit)
    {
        helpParticle.SetActive(false);

        CloseBook();
        PlacementManager.Instance.GrabBook(this);
    }

    public void PlaceBook()
    {
        HelpManager.Instance.CompleteTutorialStep(TutorialStep.GrabBook);
        if (HelpManager.Instance.CurrentStep > TutorialStep.GrabBook)
            OpenBook();
    }
}