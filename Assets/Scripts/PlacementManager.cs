using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AreaType { None, Play, Build, Cook }

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;

    public delegate void OnAreaSet();
    public OnAreaSet onAreaSet;

    [SerializeField]
    GameObject Loading;
    [SerializeField]
    GameObject PlayAreaPrefab;
    [SerializeField]
    GameObject BuildAreaPrefab;
    [SerializeField]
    GameObject CookAreaPrefab;

    PlacementArea playArea;
    PlacementArea buildArea;
    PlacementArea cookArea;
    PlacementProvider provider;
    Placable currentPlacing;
    Vector3? lastPos;
    bool placed;
    bool placingArea = true;
    PlacementArea currentArea;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
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

    IEnumerator WaitToStart()
    {
        GoogleARCore.AsyncTask<GoogleARCore.ApkAvailabilityStatus> availTask = GoogleARCore.Session.CheckApkAvailability();
        yield return availTask.WaitForCompletion();

        if (availTask.Result == GoogleARCore.ApkAvailabilityStatus.SupportedInstalled)
        {
            provider = new ARPlacementProvider();
            StartPlacement();
        }
        else if (availTask.Result == GoogleARCore.ApkAvailabilityStatus.SupportedApkTooOld || availTask.Result == GoogleARCore.ApkAvailabilityStatus.SupportedNotInstalled)
        {
            GoogleARCore.AsyncTask<GoogleARCore.ApkInstallationStatus> installTask = GoogleARCore.Session.RequestApkInstallation(false);
            yield return installTask.WaitForCompletion();

            if (installTask.Result == GoogleARCore.ApkInstallationStatus.Success)
            {
                provider = new ARPlacementProvider();
                StartPlacement();
            }
            else
            {
                provider = new TestPlacementProvider();
                StartPlacement();
            }
        }
        else
        {
            provider = new TestPlacementProvider();
            StartPlacement();
        }
    }

    private void StartPlacement()
    {
        Loading.SetActive(false);
        playArea = Instantiate(PlayAreaPrefab).GetComponent<PlacementArea>();
        currentArea = playArea;
        buildArea = BuildAreaPrefab.GetComponent<PlacementArea>();
        cookArea = CookAreaPrefab.GetComponent<PlacementArea>();
    }

    public PlacementArea GetPlayArea()
    {
        return playArea;
    }

    public void SetArea(AreaType areaType)
    {
        if (currentArea != null)
            currentArea.gameObject.SetActive(false);
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
        if (currentArea != null)
            currentArea.gameObject.SetActive(true);
    }
    
    public void StartPlacing(PlacableData placable)
    {
        currentPlacing = Instantiate(placable.Prefab).GetComponent<Placable>();
        currentPlacing.Data = placable;
        lastPos = null;
        PlaceCurrent();
    }

    public void PlaceAt(PlacableData placable, Vector3 position)
    {
        currentPlacing = Instantiate(placable.Prefab).GetComponent<Placable>();
        currentPlacing.Data = placable;
        currentPlacing.transform.SetParent(playArea.transform);
        currentPlacing.transform.localPosition = position;
        currentPlacing.transform.localRotation = Quaternion.identity;
        currentPlacing.transform.localScale = Vector3.one;
        if (currentArea != null)
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
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        {
            currentPlacing.transform.SetParent(hit.transform.parent);
            currentPlacing.transform.position = hit.point;
            currentPlacing.transform.localRotation = Quaternion.identity;
            currentPlacing.transform.localScale = Vector3.one;
            placed = true;
        }
        else
        {
            Vector3 pos = Input.mousePosition;
            pos.z = 3f;
            pos = Camera.main.ScreenToWorldPoint(pos);
            currentPlacing.transform.position = pos;
            placed = false;
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
                if (Input.GetMouseButtonDown(0))
                {
                    placingArea = false;
                    UIManager.Instance.ResetToMainUI();
                    if (onAreaSet != null)
                        onAreaSet();
                }
            }
            else
            {
                playArea.gameObject.SetActive(false);
            }
        }
        else if (currentPlacing == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Placable")))
                {
                    currentPlacing = hit.transform.GetComponentInParent<Placable>();
                    lastPos = currentPlacing.transform.localPosition;
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                PlaceCurrent();
            }
            else
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
                            currentArea.AddToArea(currentPlacing);
                    }
                }
                else
                {
                    /*if (lastPos.HasValue)
                        currentPlacing.transform.localPosition = lastPos.Value;
                    else*/
                    if (currentArea != null)
                        currentArea.RemoveFromArea(currentPlacing);
                    PlayerManager.Instance.AddInventory(currentPlacing.Data);
                    Destroy(currentPlacing.gameObject);
                }
                currentPlacing = null;
            }
        }
    }
}