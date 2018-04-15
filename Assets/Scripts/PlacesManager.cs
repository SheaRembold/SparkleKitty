using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mapbox.Unity.Map;
using Mapbox.Unity.Location;
using Mapbox.Examples;
using Mapbox.Utils;
using UnityEngine.EventSystems;

//public enum ResourceType { Recipe, Tower, Toy, Treat }

public class PlacesManager : MonoBehaviour
{
    public static PlacesManager Instance;

    [System.Serializable]
    public class ResourceLocationData
    {
        public ResourceType ResourceType;
        public string[] LocTypes;
        public GameObject MarkerPrefab;
        public MapInteractUI InteractUI;
    }
    
    [SerializeField]
    private AbstractMap _map;
    [SerializeField]
    private List<ResourceLocationData> resourceLocationData;
    [SerializeField]
    private PlacesModifier modifier;

    PhysicsRaycaster raycaster;
    private QuadTreeCameraMovement _mapMovement;
    
    private void Awake()
    {
        Instance = this;

        raycaster = GetComponent<PhysicsRaycaster>();
        _mapMovement = _map.GetComponent<QuadTreeCameraMovement>();
    }

    void Start()
    {
        raycaster.eventMask = LayerMask.GetMask("MapMarker");
    }
    
    MapMarker activeMarker;
    public void ShowInteraction(MapMarker marker)
    {
        activeMarker = marker;
        raycaster.eventMask = LayerMask.GetMask();
        _mapMovement.enabled = false;
        resourceLocationData.Find((x) => { return x.ResourceType == marker.resourceType; }).InteractUI.Show();
    }

    public void CompleteInteraction()
    {
        if (activeMarker != null)
        {
            modifier.CollectMarker(activeMarker);
            activeMarker = null;
        }
    }

    public void HideInteraction()
    {
        raycaster.eventMask = LayerMask.GetMask("MapMarker");
        _mapMovement.enabled = true;
    }
}