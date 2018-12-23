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
    Outline helpOutlinePart2;
    [SerializeField]
    AudioClip openAudio;
    [SerializeField]
    AudioClip closeAudio;
    [SerializeField]
    GameObject[] bookColliders;

    [System.NonSerialized]
    public MoveParticles letterParticles;

    GraphicRaycaster graphicRaycaster;
    CanvasRaycastTarget canvasRaycastTarget;
    Animator animator;
    AudioSource audioSource;
    Vector3 startPos;
    Quaternion startRot;

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

        startPos = transform.position;
        startRot = transform.rotation;
    }

    private void Start()
    {
        if (HelpManager.Instance.CurrentStep <= TutorialStep.GrabBook || PlacementManager.Instance.IsNonXR)
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

        for (int i = 0; i < bookColliders.Length; i++)
            bookColliders[i].layer = LayerMask.NameToLayer("Placable");
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

        for (int i = 0; i < bookColliders.Length; i++)
            bookColliders[i].layer = LayerMask.NameToLayer("UI");
    }

    public override void Click(RaycastHit hit)
    {
        if (HelpManager.Instance.CurrentStep == TutorialStep.GrabBook)
        {
            //helpParticle.SetActive(false);
            helpOutline.enabled = false;
            helpOutlinePart2.enabled = true;
        }

        if (letterParticles != null)
        {
            letterParticles.StartPlaying();
            letterParticles = null;
        }
        
        PlacementManager.Instance.GrabBook(this);
    }

    public void PlaceBook()
    {
        helpOutlinePart2.enabled = false;
        HelpManager.Instance.CompleteTutorialStep(TutorialStep.GrabBook);
        if (HelpManager.Instance.CurrentStep > TutorialStep.GrabBook)
            OpenBook();
    }

    public void ReturnBook()
    {
        CloseBook();
        transform.position = startPos;
        transform.rotation = startRot;
    }
}