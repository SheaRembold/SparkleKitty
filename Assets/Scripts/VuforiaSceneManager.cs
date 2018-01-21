using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VuforiaSceneManager : SceneManager
{
    public override bool PlaceInScene(GameObject obj)
    {
        m_PlaneAugmentation = obj;
        m_PlaneFinder.PerformHitTest(Input.mousePosition);

        return true;
    }
    

    #region PUBLIC_MEMBERS
    public PlaneFinderBehaviour m_PlaneFinder;
    #endregion // PUBLIC_MEMBERS


    #region PRIVATE_MEMBERS
    GameObject m_PlaneAugmentation;
    PositionalDeviceTracker positionalDeviceTracker;
    ContentPositioningBehaviour contentPositioningBehaviour;
    GameObject m_PlaneAnchor;
    AnchorBehaviour[] anchorBehaviours;
    StateManager stateManager;
    Camera mainCamera;
    int AutomaticHitTestFrameCount;
    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS

    void Start()
    {
        Debug.Log("Start() called.");
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnVuforiaPaused);
        DeviceTrackerARController.Instance.RegisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
        
        mainCamera = Camera.main;
    }
    
    void LateUpdate()
    {
        if (AutomaticHitTestFrameCount == Time.frameCount)
        {
            SetSurfaceIndicatorVisible(true);
        }
        else
        {
            SetSurfaceIndicatorVisible(false);
        }
    }

    void OnDestroy()
    {
        Debug.Log("OnDestroy() called.");

        VuforiaARController.Instance.UnregisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.UnregisterOnPauseCallback(OnVuforiaPaused);
        DeviceTrackerARController.Instance.UnregisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.UnregisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
    }

    #endregion // MONOBEHAVIOUR_METHODS

    #region VUFORIA_CALLBACKS

    void OnVuforiaStarted()
    {
        Debug.Log("OnVuforiaStarted() called.");

        stateManager = TrackerManager.Instance.GetStateManager();
    }

    void OnVuforiaPaused(bool paused)
    {
        Debug.Log("OnVuforiaPaused(" + paused.ToString() + ") called.");
    }

    #endregion // VUFORIA_CALLBACKS


    #region PRIVATE_METHODS

    void SetSurfaceIndicatorVisible(bool isVisible)
    {
        Renderer[] renderers = m_PlaneFinder.PlaneIndicator.GetComponentsInChildren<Renderer>(true);
        Canvas[] canvas = m_PlaneFinder.PlaneIndicator.GetComponentsInChildren<Canvas>(true);

        foreach (Canvas c in canvas)
            c.enabled = isVisible;

        foreach (Renderer r in renderers)
            r.enabled = isVisible;
    }

    private void DestroyAnchors()
    {
        IEnumerable<TrackableBehaviour> trackableBehaviours = stateManager.GetActiveTrackableBehaviours();

        string destroyed = "Destroying: ";

        foreach (TrackableBehaviour behaviour in trackableBehaviours)
        {
            if (behaviour is AnchorBehaviour)
            {
                // First determine which mode (Plane or MidAir) and then delete only the anchors for that mode
                // Leave the other mode's anchors intact
                // PlaneAnchor_<GUID>
                // Mid AirAnchor_<GUID>

                if (behaviour.Trackable.Name.Contains("PlaneAnchor"))
                {
                    destroyed +=
                        "\nGObj Name: " + behaviour.name +
                       "\nTrackable Name: " + behaviour.Trackable.Name +
                       "\nTrackable ID: " + behaviour.Trackable.ID +
                       "\nPosition: " + behaviour.transform.position.ToString();

                    stateManager.DestroyTrackableBehavioursForTrackable(behaviour.Trackable);
                    stateManager.ReassociateTrackables();
                }
            }
        }

        Debug.Log(destroyed);
    }

    private void RotateTowardCamera(GameObject augmentation)
    {
        var lookAtPosition = mainCamera.transform.position - augmentation.transform.position;
        lookAtPosition.y = 0;
        var rotation = Quaternion.LookRotation(lookAtPosition);
        augmentation.transform.rotation = rotation;
    }

    #endregion // PRIVATE_METHODS

    #region PUBLIC_METHODS

    public void HandleAutomaticHitTest(HitTestResult result)
    {
        Debug.Log("Result: " + result.Position);

        AutomaticHitTestFrameCount = Time.frameCount;
    }

    public void HandleInteractiveHitTest(HitTestResult result)
    {
        Debug.Log("HandleInteractiveHitTest() called.");

        if (result == null)
        {
            Debug.LogError("Invalid hit test result!");
            return;
        }
        
        if (positionalDeviceTracker != null && positionalDeviceTracker.IsActive)
        {
            DestroyAnchors();

            contentPositioningBehaviour = m_PlaneFinder.GetComponent<ContentPositioningBehaviour>();
            contentPositioningBehaviour.PositionContentAtPlaneAnchor(result);
        }

        if (m_PlaneAugmentation == null)
        {
            Debug.Log("Plane Augmentation is null");
            return;
        }

        Debug.Log("Positioning Plane Augmentation at: " + result.Position);
        m_PlaneAugmentation.PositionAt(result.Position);
        RotateTowardCamera(m_PlaneAugmentation);
    }
    
    #endregion // PUBLIC_METHODS


    #region DEVICE_TRACKER_CALLBACKS

    void OnTrackerStarted()
    {
        Debug.Log("OnTrackerStarted() called.");

        positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();

        if (positionalDeviceTracker != null)
        {
            if (!positionalDeviceTracker.IsActive)
                positionalDeviceTracker.Start();

            Debug.Log("PositionalDeviceTracker is Active?: " + positionalDeviceTracker.IsActive);
        }
    }

    void OnDevicePoseStatusChanged(TrackableBehaviour.Status status)
    {
        Debug.Log("OnDevicePoseStatusChanged(" + status.ToString() + ")");
    }

    #endregion // DEVICE_TRACKER_CALLBACK_METHODS
}