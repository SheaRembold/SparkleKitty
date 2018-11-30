using System.Collections;
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
    [SerializeField]
    Outline helpOutline;
    [SerializeField]
    AudioClip openAudio;
    [SerializeField]
    AudioClip closeAudio;

    GraphicRaycaster graphicRaycaster;
    CanvasRaycastTarget canvasRaycastTarget;
    Animator animator;
    AudioSource audioSource;

    [System.NonSerialized]
    public MoveParticles letterParticles;

    private void Awake()
    {
        Instance = this;

        graphicRaycaster = uiRoot.GetComponent<GraphicRaycaster>();
        canvasRaycastTarget = uiRoot.GetComponent<CanvasRaycastTarget>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (HelpManager.Instance.CurrentStep == TutorialStep.GrabBook)
        {
            //helpParticle.SetActive(true);
            helpOutline.enabled = true;
        }

        HelpManager.Instance.onCompleteTutorialStep += OnCompleteTutorialStep;
    }

    private void Start()
    {
        if (HelpManager.Instance.CurrentStep <= TutorialStep.GrabBook)
            CloseBook();
    }

    void OnCompleteTutorialStep(TutorialStep currentStep)
    {
        if (HelpManager.Instance.CurrentStep == TutorialStep.GrabBook)
        {
            //helpParticle.SetActive(true);
            helpOutline.enabled = true;
        }
    }

    public void CloseBook()
    {
        uiRoot.SetActive(false);
        animator.SetBool("IsOpen", false);
        if (PlacementManager.Instance.IsUsingSteamVR)
            canvasRaycastTarget.enabled = false;
        else
            graphicRaycaster.enabled = false;

        audioSource.clip = closeAudio;
        audioSource.Play();
    }

    public void OpenBook()
    {
        uiRoot.SetActive(true);
        animator.SetBool("IsOpen", true);
        if (PlacementManager.Instance.IsUsingSteamVR)
            canvasRaycastTarget.enabled = true;
        else
            graphicRaycaster.enabled = true;
        
        audioSource.clip = openAudio;
        audioSource.Play();
    }

    public override void Click(RaycastHit hit)
    {
        //helpParticle.SetActive(false);
        helpOutline.enabled = false;

        if (letterParticles != null)
        {
            letterParticles.StartPlaying();
            letterParticles = null;
        }

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