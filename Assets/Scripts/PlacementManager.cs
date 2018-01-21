using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;
    public TestSceneManager TestScene;
    public VuforiaSceneManager VuforiaScene;
    SceneManager sceneManager;
    GameObject currentPlacing;
    Vector3? lastPos;
    bool placed;

    private void Awake()
    {
        Instance = this;
#if UNITY_EDITOR
        sceneManager = TestScene;
        VuforiaScene.gameObject.SetActive(false);
        TestScene.gameObject.SetActive(true);
#else
        sceneManager = VuforiaScene;
        TestScene.gameObject.SetActive(false);
        VuforiaScene.gameObject.SetActive(true);
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
        if (currentPlacing == null)
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