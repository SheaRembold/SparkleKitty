using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;
    public GameObject PlayArea;
    public TestSceneManager TestScene;
    public ARSceneManager ARScene;
#if VUFORIA
    public VuforiaSceneManager VuforiaScene;
#endif
    SceneController sceneManager;
    GameObject currentPlacing;
    Vector3? lastPos;
    bool placed;
    bool placingArea = true;

    private void Awake()
    {
        Instance = this;
#if UNITY_EDITOR
        sceneManager = TestScene;
        ARScene.gameObject.SetActive(false);
#if VUFORIA
        VuforiaScene.gameObject.SetActive(false);
#endif
        TestScene.gameObject.SetActive(true);
#elif VUFORIA
        sceneManager = VuforiaScene;
        TestScene.gameObject.SetActive(false);
        ARScene.gameObject.SetActive(false);
        VuforiaScene.gameObject.SetActive(true);
#else
        sceneManager = ARScene;
        TestScene.gameObject.SetActive(false);
#if VUFORIA
        VuforiaScene.gameObject.SetActive(false);
#endif
        ARScene.gameObject.SetActive(true);
#endif
    }

    public void StartPlacing(PlacableData placable)
    {
        currentPlacing = Instantiate(placable.Prefab);
        lastPos = null;
        PlaceCurrent();
    }

    void PlaceCurrent()
    {
        placed = sceneManager.PlaceInScene(currentPlacing);
        if (!placed)
        {
            Vector3 pos = Input.mousePosition;
            pos.z = 3f;
            pos = Camera.main.ScreenToWorldPoint(pos);
            currentPlacing.transform.position = pos;
        }
    }

    void Update()
    {
        if (placingArea)
        {
            UnityARInterface.BoundedPlane plane;
            if (sceneManager.GetPlane(out plane))
            {
                PlayArea.transform.position = plane.center;
                PlayArea.transform.rotation = plane.rotation;
                float scale = Mathf.Min(1f, plane.extents.x, plane.extents.y);
                PlayArea.transform.localScale = Vector3.one * scale;
                PlayArea.SetActive(true);
                if (Input.GetMouseButtonDown(0))
                {
                    placingArea = false;
                }
            }
            else
            {
                PlayArea.SetActive(false);
            }
        }
        else if (currentPlacing == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Placable")))
                {
                    currentPlacing = hit.transform.root.gameObject;
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
                if (!placed)
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