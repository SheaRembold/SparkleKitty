using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
#if VUFORIA
using Vuforia;

public class VuforiaPlacementProvider : PlacementProvider
{
    PlaneFinderBehaviour planeFinder;
    PositionalDeviceTracker positionalDeviceTracker;

    BoundedPlane currentPlane;
    
    public VuforiaPlacementProvider()
    {
        sceneObj = GameObject.Instantiate(Resources.Load<GameObject>("VuforiaScene"));
        planeFinder = sceneObj.GetComponentInChildren<PlaneFinderBehaviour>();
        planeFinder.OnAutomaticHitTest.AddListener(HandleAutomaticHitTest);

        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnVuforiaPaused);
        DeviceTrackerARController.Instance.RegisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
    }

    ~VuforiaPlacementProvider()
    {
        VuforiaARController.Instance.UnregisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.UnregisterOnPauseCallback(OnVuforiaPaused);
        DeviceTrackerARController.Instance.UnregisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.UnregisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
    }
    
    public void HandleAutomaticHitTest(HitTestResult result)
    {
        if (string.IsNullOrEmpty(currentPlane.id))
            currentPlane.id = System.Guid.NewGuid().ToString();
        currentPlane.center = result.Position;
        currentPlane.rotation = result.Rotation;
        currentPlane.extents = new Vector2(10f, 10f);
    }

    public override bool GetPlane(out BoundedPlane plane)
    {
        if (!string.IsNullOrEmpty(currentPlane.id))
        {
            plane = currentPlane;
            return true;
        }

        plane = new BoundedPlane();
        return false;
    }

    void OnVuforiaStarted()
    {
        Debug.Log("OnVuforiaStarted() called.");
    }

    void OnVuforiaPaused(bool paused)
    {
        Debug.Log("OnVuforiaPaused(" + paused.ToString() + ") called.");
    }

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
}
#endif