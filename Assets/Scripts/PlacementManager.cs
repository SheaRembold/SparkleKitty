using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;

    public delegate void OnPlaced(Placable placable);
    public OnPlaced onPlaced;
    public delegate void OnMoved(Placable placable);
    public OnMoved onMoved;
    public delegate void OnRemoved(Placable placable);
    public OnMoved onRemoved;

    [SerializeField]
    GameObject PlayAreaPrefab;

    GameObject playArea;
    PlacementProvider provider;
    Placable currentPlacing;
    Vector3? lastPos;
    bool placed;
    bool placingArea = true;

    private void Awake()
    {
        Instance = this;
#if UNITY_EDITOR
        provider = new TestPlacementProvider();
#elif VUFORIA
        provider = new VuforiaPlacementProvider();
#else
        provider = new ARPlacementProvider();
#endif
    }

    private void Start()
    {
        playArea = Instantiate(PlayAreaPrefab);
    }

    public void StartPlacing(PlacableData placable)
    {
        currentPlacing = Instantiate(placable.Prefab).GetComponent<Placable>();
        currentPlacing.Data = placable;
        lastPos = null;
        PlaceCurrent();
    }

    void PlaceCurrent()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        {
            currentPlacing.transform.position = hit.point;
            currentPlacing.transform.rotation = hit.transform.rotation;
            currentPlacing.transform.localScale = hit.transform.parent.localScale;
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
        if (placingArea)
        {
            UnityARInterface.BoundedPlane plane;
            if (provider.GetPlane(out plane))
            {
                playArea.transform.position = plane.center;
                playArea.transform.rotation = plane.rotation;
                float scale = Mathf.Min(1f, plane.extents.x, plane.extents.y);
                playArea.transform.localScale = Vector3.one * scale;
                playArea.SetActive(true);
                if (Input.GetMouseButtonDown(0))
                {
                    placingArea = false;
                    UIManager.Instance.ResetToMainUI();
                }
            }
            else
            {
                playArea.SetActive(false);
            }
        }
        else if (currentPlacing == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Placable")))
                {
                    currentPlacing = hit.transform.root.GetComponent<Placable>();
                    lastPos = currentPlacing.transform.position;
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
                        if (onMoved != null)
                            onMoved(currentPlacing);
                    }
                    else
                    {
                        if (onPlaced != null)
                            onPlaced(currentPlacing);
                    }
                }
                else
                {
                    if (lastPos.HasValue)
                        currentPlacing.transform.position = lastPos.Value;
                    else
                        Destroy(currentPlacing);
                }
                currentPlacing = null;
            }
        }
    }
}