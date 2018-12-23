using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.XR;

public enum AreaType { None, Play, Build, Cook }

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;

    [SerializeField]
    bool UseSteamVR;
    [SerializeField]
    GameObject LoadingUI;
    [SerializeField]
    PlacementUI HelpUI;
    //[SerializeField]
    //GameObject MainUI;
    [SerializeField]
    GameObject PlayAreaPrefab;
    [SerializeField]
    GameObject BuildAreaPrefab;
    [SerializeField]
    GameObject CookAreaPrefab;

    public bool IsNonXR;

    public bool IsUsingSteamVR { get; private set; }

    PlayArea playArea;
    BuildArea buildArea;
    CookArea cookArea;
    PlacementProvider provider;
    Placable currentPlacing;
    Vector3? lastPos;
    bool placed;
    bool placingArea = true;
    PlacementArea currentArea;
    AreaType lastAreaType = AreaType.None;
    AreaType currentAreaType = AreaType.None;
    Clickable currentClickable;
    public Placable CurrentAttached { get; private set; }
    public List<IChasable> chasables = new List<IChasable>();
    BookController heldBook;

    public bool IsReady { get { return provider != null && !placingArea; } }

    private void Awake()
    {
        Instance = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void Start()
    {
        StartPlaying();
    }

    public void StartPlaying()
    {
        //UIManager.Instance.ShowUI(MainUI);
        if (provider == null)
        {
            if (IsNonXR)
            {
                provider = new NonXRPlacementProvider();
                StartPlacement();
                placingArea = false;
                HelpUI.gameObject.SetActive(false);
                playArea.SetArea();
                provider.FinishInit();
            }
            else
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                if (UseSteamVR && UnityEngine.XR.XRDevice.isPresent)
                {
                    IsUsingSteamVR = true;
                    provider = new SteamVRPlacementProvider();
                    StartPlacement();
                }
                else
                {
                    provider = new TestPlacementProvider();
                    StartPlacement();
                }
#elif VUFORIA
                provider = new VuforiaPlacementProvider();
                StartPlacement();
#elif UNITY_ANDROID
                StartCoroutine(WaitToStart());
#else
                provider = new ARPlacementProvider();
                StartPlacement();
#endif
            }
        }
        else
        {
            SetArea(AreaType.Play);
            if (placingArea)
            {
                //UIManager.Instance.ShowUI(HelpUI.gameObject);
                LoadingUI.SetActive(false);
                HelpUI.gameObject.SetActive(true);
            }
        }
    }

    IEnumerator WaitToStart()
    {
        //UIManager.Instance.ShowUI(LoadingUI);
        HelpUI.gameObject.SetActive(false);
        LoadingUI.SetActive(true);

        /*GoogleARCore.AsyncTask<GoogleARCore.ApkAvailabilityStatus> availTask = GoogleARCore.Session.CheckApkAvailability();
        yield return availTask.WaitForCompletion();

        if (availTask.Result == GoogleARCore.ApkAvailabilityStatus.SupportedInstalled)
        {*/
        provider = new ARPlacementProvider();
        /*}
        else if (availTask.Result == GoogleARCore.ApkAvailabilityStatus.SupportedApkTooOld || availTask.Result == GoogleARCore.ApkAvailabilityStatus.SupportedNotInstalled)
        {
            GoogleARCore.AsyncTask<GoogleARCore.ApkInstallationStatus> installTask = GoogleARCore.Session.RequestApkInstallation(false);
            yield return installTask.WaitForCompletion();

            if (installTask.Result == GoogleARCore.ApkInstallationStatus.Success)
            {
                provider = new ARPlacementProvider();
            }
            else
            {
                provider = new TestPlacementProvider();
            }
        }
        else
        {
            provider = new TestPlacementProvider();
        }*/

        yield return new WaitUntil(() => provider.IsReady());

        yield return new WaitForEndOfFrame();

        StartPlacement();
    }

    private void StartPlacement()
    {
        playArea = Instantiate(PlayAreaPrefab).GetComponent<PlayArea>();
        playArea.transform.SetParent(provider.GetRoot());
        currentArea = playArea;
        currentAreaType = AreaType.Play;
        //buildArea = BuildAreaPrefab.GetComponent<BuildArea>();
        //cookArea = CookAreaPrefab.GetComponent<CookArea>();
        //UIManager.Instance.ShowUI(HelpUI.gameObject);
        LoadingUI.SetActive(false);
        HelpUI.gameObject.SetActive(true);
    }

    public void SetAttached(PlacableData placable)
    {
        RemoveAttached();

        CurrentAttached = Instantiate(placable.Prefab).GetComponent<Placable>();
        CurrentAttached.Data = placable;
        ItemController itemController = CurrentAttached.GetComponent<ItemController>();
        if (itemController != null)
        {
            if (placable.Unlimited)
                itemController.SetAmountLeft(1f);
            else
                itemController.SetAmountLeft(PlayerManager.Instance.GetItemHealth(placable) - (PlayerManager.Instance.GetInventoryCount(placable) - 1));
        }
        CurrentAttached.transform.SetParent(provider.holdAttachPoint);
        CurrentAttached.transform.localPosition = Vector3.zero;
        CurrentAttached.transform.localRotation = placable.Prefab.transform.rotation;
        CurrentAttached.transform.localScale = Vector3.one;

        playArea.CheckForCats();
    }

    public void RemoveAttached()
    {
        if (CurrentAttached != null)
        {
            Destroy(CurrentAttached.gameObject);
            CurrentAttached = null;

            playArea.CheckWaitingCat();
        }
    }

    public PlayArea GetPlayArea()
    {
        return playArea;
    }

    public BuildArea GetBuildArea()
    {
        return buildArea;
    }

    public BuildArea GetCookArea()
    {
        return cookArea;
    }

    public void SetArea(AreaType areaType)
    {
        if (currentArea != null)
            currentArea.gameObject.SetActive(false);
        if (provider != null)
        {
            switch (areaType)
            {
                case AreaType.None:
                    provider.TurnOff();
                    currentArea = null;
                    break;
                case AreaType.Play:
                    provider.TurnOn();
                    currentArea = playArea;
                    break;
                case AreaType.Build:
                    provider.TurnOff();
                    currentArea = buildArea;
                    break;
                case AreaType.Cook:
                    provider.TurnOff();
                    currentArea = cookArea;
                    break;
            }
        }
        if (currentArea != null)
            currentArea.gameObject.SetActive(true);

        lastAreaType = currentAreaType;
        currentAreaType = areaType;
    }

    public void ResetLastArea()
    {
        SetArea(lastAreaType);
    }

    public void StartPlacing(PlacableData placable)
    {
        if (currentPlacing != null)
            Destroy(currentPlacing.gameObject);
        currentPlacing = Instantiate(placable.Prefab).GetComponent<Placable>();
        currentPlacing.Data = placable;
        ItemController itemController = currentPlacing.GetComponent<ItemController>();
        if (itemController != null)
        {
            if (placable.Unlimited)
                itemController.SetAmountLeft(1f);
            else
                itemController.SetAmountLeft(PlayerManager.Instance.GetItemHealth(placable) - (PlayerManager.Instance.GetInventoryCount(placable) - 1));
        }
        lastPos = null;
        HelpUI.gameObject.SetActive(true);
        playArea.ShowPlacing(true);
        PlaceCurrent();
        placingDown = false;
        if (IsNonXR)
            BookController.Instance.ReturnBook();
    }

    public void GrabBook(BookController book)
    {
        heldBook = book;

        heldBook.transform.SetParent(provider.holdAttachPoint);
        heldBook.transform.localPosition = provider.BookOffset;
        heldBook.transform.localRotation = Quaternion.identity;

        if(IsNonXR)
        {
            heldBook.transform.SetParent(playArea.transform);
            heldBook.PlaceBook();
            heldBook = null;
        }
        else
        {
            heldBook.CloseBook();
            HelpUI.gameObject.SetActive(true);
            HelpUI.ShowPlaceItem();
        }
    }

    Coroutine moveLetterCoroutine;
    [SerializeField]
    float moveLetterTime = 1f;
    public void GrabLetter(LetterController letter)
    {
        if (moveLetterCoroutine != null)
            StopCoroutine(moveLetterCoroutine);
        moveLetterCoroutine = StartCoroutine(MoveLetter(letter));
    }

    IEnumerator MoveLetter(LetterController letter)
    {
        Vector3 startPos = letter.transform.position;
        Quaternion startRot = letter.transform.rotation;
        float time = 0f;
        while (time < moveLetterTime)
        {
            time += Time.deltaTime;
            letter.transform.position = Vector3.Lerp(startPos, provider.viewAttachPoint.position + provider.viewAttachPoint.up * (provider.BookOffset.y * provider.viewAttachPoint.localScale.y) + provider.viewAttachPoint.forward * (provider.BookOffset.z * provider.viewAttachPoint.localScale.z), time / moveLetterTime);
            letter.transform.rotation = Quaternion.Lerp(startRot, provider.viewAttachPoint.rotation, time / moveLetterTime);
            yield return null;
        }

        moveLetterCoroutine = null;
    }

    PlacementArea tempArea;
    public void SetTempArea(AreaType areaType)
    {
        tempArea = currentArea;
        switch (areaType)
        {
            case AreaType.None:
                currentArea = null;
                break;
            case AreaType.Play:
                currentArea = playArea;
                break;
            case AreaType.Build:
                currentArea = buildArea;
                break;
            case AreaType.Cook:
                currentArea = cookArea;
                break;
        }
    }

    public void RestoreArea()
    {
        currentArea = tempArea;
    }

    public Placable PlaceAt(PlacementArea area, PlacableData placable, Vector3 position)
    {
        return PlaceAt(area, placable, position, Vector3.zero);
    }

    public Placable PlaceAt(PlacementArea area, PlacableData placable, Vector3 position, Vector3 rotation)
    {
        Placable newPlacable = Instantiate(placable.Prefab).GetComponent<Placable>();
        newPlacable.Data = placable;
        newPlacable.transform.SetParent(area.Contents);
        newPlacable.transform.localPosition = position;
        newPlacable.transform.localRotation = Quaternion.Euler(rotation);
        newPlacable.transform.localScale = Vector3.one;

        area.AddToArea(newPlacable);

        return newPlacable;
    }

    public void Remove(Placable oldPlacable)
    {
        Remove(currentArea, oldPlacable);
    }

    public void Remove(PlacementArea area, Placable oldPlacable)
    {
        area.RemoveFromArea(oldPlacable);
        Destroy(oldPlacable.gameObject);
    }

    public void Replace(Placable oldPlacable, PlacableData newData)
    {
        currentPlacing = Instantiate(newData.Prefab).GetComponent<Placable>();
        currentPlacing.Data = newData;
        currentPlacing.transform.SetParent(oldPlacable.transform.parent);
        currentPlacing.transform.localPosition = oldPlacable.transform.localPosition;
        currentPlacing.transform.localRotation = oldPlacable.transform.localRotation;
        currentPlacing.transform.localScale = oldPlacable.transform.localScale;

        currentArea.RemoveFromArea(oldPlacable);
        Destroy(oldPlacable.gameObject);
        currentArea.AddToArea(currentPlacing);
        currentPlacing = null;
    }

    public Vector3 GetRandomInArea()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 0.5f), Random.Range(-1f, 1f));
    }

    public Vector3 GetWorldNavPos(Vector3 areaPos)
    {
        Vector3 worldPos = currentArea.transform.TransformPoint(areaPos);
        NavMeshHit hit;
        NavMesh.SamplePosition(worldPos, out hit, 10f, NavMesh.AllAreas);
        return hit.position;
    }

    public Vector3 GetNavPos(Vector3 worldPos)
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(worldPos, out hit, 10f, NavMesh.AllAreas);
        return hit.position;
    }

    void PlaceCurrent()
    {
        RaycastHit hit;
        //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        if (Physics.Raycast(provider.GetPlaceRay(), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        {
            currentPlacing.gameObject.SetActive(true);
            currentPlacing.transform.SetParent(hit.transform.parent);
            currentPlacing.transform.position = hit.point;
            currentPlacing.transform.localRotation = Quaternion.identity;
            currentPlacing.transform.localScale = Vector3.one;
            placed = true;
            HelpUI.ShowPlaceItem();
        }
        else
        {
            currentPlacing.gameObject.SetActive(true);
            currentPlacing.transform.SetParent(provider.holdAttachPoint);
            currentPlacing.transform.localPosition = Vector3.zero;
            currentPlacing.transform.localRotation = Quaternion.identity;
            currentPlacing.transform.localScale = Vector3.one * 0.5f;
            placed = false;
            HelpUI.ShowLookingItem();
        }
    }

    bool placingDown;
    void Update()
    {
        if (provider == null)
            return;

        if (placingArea)
        {
            BoundedPlane plane;
            if (provider.GetPlane(out plane))
            {
                playArea.transform.position = plane.Center;
                playArea.transform.rotation = plane.Pose.rotation;
                float scale = Mathf.Min(1f, plane.Size.x, plane.Size.y);
                playArea.transform.localScale = Vector3.one * scale;
                playArea.gameObject.SetActive(true);

                Vector3 forword = -playArea.transform.forward;
                forword.y = 0f;
                Vector3 look = Camera.main.transform.position - playArea.transform.position;
                look.y = 0f;
                float angle = Vector3.SignedAngle(look, forword, Vector3.up);
                if (angle > 135f || angle < -135f)
                    playArea.transform.Rotate(playArea.transform.up, 180f);
                else if (angle > 45f)
                    playArea.transform.Rotate(playArea.transform.up, -90f);
                else if (angle < -45f)
                    playArea.transform.Rotate(playArea.transform.up, 90f);

                HelpUI.ShowPlace();

                if (provider.GetClickDown())
                {
                    placingArea = false;
                    provider.holdAttachPoint.localPosition = provider.holdAttachPoint.localPosition * scale;
                    provider.holdAttachPoint.localScale = Vector3.one * scale;
                    //UIManager.Instance.GoBackToUI(MainUI);
                    LoadingUI.SetActive(false);
                    HelpUI.gameObject.SetActive(false);
                    playArea.SetArea();
                    provider.FinishInit();
                }
            }
            else
            {
                playArea.gameObject.SetActive(false);
                HelpUI.ShowLooking();
            }
        }
        else if (heldBook != null)
        {
            if (provider.GetClickUp())
            {
                heldBook.transform.SetParent(playArea.transform);
                heldBook.PlaceBook();
                heldBook = null;
                HelpUI.gameObject.SetActive(false);
            }
        }
        else if (currentPlacing == null)
        {
            if (currentClickable == null)
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                if (provider.GetClickDown())
#else
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#endif
                {
                    RaycastHit hit;
                    if (Physics.Raycast(provider.GetClickRay(), out hit, Mathf.Infinity, (1 << LayerMask.NameToLayer("Placable")) | (1 << LayerMask.NameToLayer("UI"))))
                    {
                        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Placable"))
                        {
                            currentClickable = hit.transform.GetComponentInParent<Clickable>();

                            if (currentClickable == null && currentArea.AllowMovement)
                            {
                                currentPlacing = hit.transform.GetComponentInParent<Placable>();
                                lastPos = currentPlacing.transform.localPosition;
                                placingDown = true;
                            }
                        }
                    }
                }
            }
            else
            {
                if (provider.GetClickUp())
                {
                    RaycastHit hit;
                    if (Physics.Raycast(provider.GetClickRay(), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Placable")))
                    {
                        Clickable upClickable = hit.transform.GetComponentInParent<Clickable>();
                        if (upClickable == currentClickable)
                            currentClickable.Click(hit);
                    }
                    currentClickable = null;
                }
            }
        }
        else
        {
            PlaceCurrent();
            if (provider.GetClickDown())
            {
                placingDown = true;
            }
            else if (placingDown && provider.GetClickUp())
            {
                if (placed)
                {
                    if (lastPos.HasValue)
                    {
                        if (currentArea != null)
                            currentArea.MoveInArea(currentPlacing);
                    }
                    else
                    {
                        if (currentArea != null)
                        {
                            currentArea.AddToArea(currentPlacing);
                            PlayerManager.Instance.RemoveInventory(currentPlacing.Data);
                        }
                    }
                    currentPlacing = null;
                    HelpUI.gameObject.SetActive(false);
                    playArea.ShowPlacing(false);
                    if (HelpManager.Instance.CurrentStep == TutorialStep.PlaceTreat)
                        HelpManager.Instance.CompleteTutorialStep(TutorialStep.PlaceTreat);
                    else if (HelpManager.Instance.CurrentStep == TutorialStep.PlaceToy)
                        HelpManager.Instance.CompleteTutorialStep(TutorialStep.PlaceToy);
                    placingDown = false;
                    SoundManager.Instance.PlayGroup("PlaceItem");
                    if (IsNonXR)
                        GrabBook(BookController.Instance);
                }
                /*else
                {
                    /*if (lastPos.HasValue)
                        currentPlacing.transform.localPosition = lastPos.Value;
                    else*
                    if (currentArea != null)
                        currentArea.RemoveFromArea(currentPlacing);
                    PlayerManager.Instance.AddInventory(currentPlacing.Data);
                    Destroy(currentPlacing.gameObject);
                }*/
            }
        }

        if (currentClickable == null)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (provider.GetClickDown())
#else
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#endif
            {
                RaycastHit hit;
                if (Physics.Raycast(provider.GetClickRay(), out hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Base"))
                    {
                        BookController.Instance.ReturnBook();
                    }
                }
            }
        }
    }
}