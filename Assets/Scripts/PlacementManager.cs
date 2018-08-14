using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum AreaType { None, Play, Build, Cook }

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;
    
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
    public delegate void OnFinishedPlacing(Placable placable);
    public OnFinishedPlacing onFinishedPlacing;
    public List<IChasable> chasables = new List<IChasable>();

    private void Awake()
    {
        Instance = this;
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
#if UNITY_EDITOR
            provider = new TestPlacementProvider();
            StartPlacement();
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

        GoogleARCore.AsyncTask<GoogleARCore.ApkAvailabilityStatus> availTask = GoogleARCore.Session.CheckApkAvailability();
        yield return availTask.WaitForCompletion();

        if (availTask.Result == GoogleARCore.ApkAvailabilityStatus.SupportedInstalled)
        {
            provider = new ARPlacementProvider();
        }
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
        }

        yield return new WaitUntil(() => provider.IsReady());

        yield return new WaitForEndOfFrame();

        StartPlacement();
    }

    private void StartPlacement()
    {
        playArea = Instantiate(PlayAreaPrefab).GetComponent<PlayArea>();
        currentArea = playArea;
        currentAreaType = AreaType.Play;
        buildArea = BuildAreaPrefab.GetComponent<BuildArea>();
        cookArea = CookAreaPrefab.GetComponent<CookArea>();
        //UIManager.Instance.ShowUI(HelpUI.gameObject);
        LoadingUI.SetActive(false);
        HelpUI.gameObject.SetActive(true);
    }
    
    public void SetAttached(PlacableData placable)
    {
        RemoveAttached();

        CurrentAttached = Instantiate(placable.Prefab).GetComponent<Placable>();
        CurrentAttached.Data = placable;
        CurrentAttached.transform.SetParent(provider.attachPoint);
        CurrentAttached.transform.localPosition = Vector3.zero;
        CurrentAttached.transform.localRotation = placable.Prefab.transform.rotation;
        CurrentAttached.transform.localScale = Vector3.one;
    }

    public void RemoveAttached()
    {
        if (CurrentAttached != null)
        {
            Destroy(CurrentAttached.gameObject);
            CurrentAttached = null;
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
        lastPos = null;
        HelpUI.gameObject.SetActive(true);
        PlaceCurrent();
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

    public Placable PlaceAt(PlacableData placable, Vector3 position)
    {
        return PlaceAt(placable, position, Vector3.zero);
    }

    public Placable PlaceAt(PlacableData placable, Vector3 position, Vector3 rotation)
    {
        Placable newPlacable = currentPlacing = Instantiate(placable.Prefab).GetComponent<Placable>();
        currentPlacing.Data = placable;
        currentPlacing.transform.SetParent(currentArea.Contents);
        currentPlacing.transform.localPosition = position;
        currentPlacing.transform.localRotation = Quaternion.Euler(rotation);
        currentPlacing.transform.localScale = Vector3.one;
        
        currentArea.AddToArea(currentPlacing);
        currentPlacing = null;

        return newPlacable;
    }

    public void Remove(Placable oldPlacable)
    {
        currentArea.RemoveFromArea(oldPlacable);
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
        return new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
    }

    public Vector3 GetWorldPos(Vector3 areaPos)
    {
        return currentArea.transform.TransformPoint(areaPos);
    }

    void PlaceCurrent()
    {
        RaycastHit hit;
        //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
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
            currentPlacing.transform.SetParent(null);
            currentPlacing.gameObject.SetActive(false);
            placed = false;
            HelpUI.ShowLookingItem();
        }
    }

    void Update()
    {
        if (provider == null)
            return;

        if (placingArea)
        {
            UnityARInterface.BoundedPlane plane;
            if (provider.GetPlane(out plane))
            {
                playArea.transform.position = plane.center;
                playArea.transform.rotation = plane.rotation;
                float scale = Mathf.Min(1f, plane.extents.x, plane.extents.y);
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

                if (Input.GetMouseButtonDown(0))
                {
                    placingArea = false;
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
        else if (currentPlacing == null)
        {
            if (currentClickable == null)
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
#else
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#endif
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Placable")))
                    {
                        currentClickable = hit.transform.GetComponentInParent<Clickable>();

                        if (currentClickable == null && currentArea.AllowMovement)
                        {
                            currentPlacing = hit.transform.GetComponentInParent<Placable>();
                            lastPos = currentPlacing.transform.localPosition;
                        }
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Placable")))
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
            if (Input.GetMouseButtonUp(0))
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
                            if (onFinishedPlacing != null)
                                onFinishedPlacing(currentPlacing);
                        }
                    }
                    currentPlacing = null;
                    HelpUI.gameObject.SetActive(false);
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
    }
}