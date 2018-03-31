using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mapbox.Unity.Map;
using Mapbox.Unity.Location;
using Mapbox.Examples;
using Mapbox.Utils;
using UnityEngine.EventSystems;

public enum ResourceType { Recipe, Tower, Toy, Treat }

public class Places : MonoBehaviour
{
    [System.Serializable]
    public class ResourceLocationData
    {
        public ResourceType ResourceType;
        public string[] LocTypes;
        public GameObject MarkerPrefab;
        public MapInteractUI InteractUI;
    }

    [SerializeField]
    private string apiKey;
    [SerializeField]
    private AbstractMap _map;
    [SerializeField]
    int radius = 100;
    [SerializeField]
    private List<ResourceLocationData> resourceLocationData;

    PhysicsRaycaster raycaster;
    private QuadTreeCameraMovement _mapMovement;
    bool _isInitialized;
    bool searchHasRun;

    int resourceIndex;
    int locationIndex;

    List<GameObject> placeMarkers = new List<GameObject>();
    Vector2d lastLoc;

    public class Response
    {
        public Result[] results;
        public string next_page_token;
    }

    [System.Serializable]
    public class Result
    {
        public Geometry geometry;
        public string name;
    }

    [System.Serializable]
    public class Geometry
    {
        public Location location;
    }

    [System.Serializable]
    public class Location
    {
        public float lat;
        public float lng;
    }

    private void Awake()
    {
        raycaster = GetComponent<PhysicsRaycaster>();
        _mapMovement = _map.GetComponent<QuadTreeCameraMovement>();
    }

    void Start()
    {
        _map.OnInitialized += () => _isInitialized = true;
        raycaster.eventMask = LayerMask.GetMask("MapMarker");
    }

    void LateUpdate()
    {
        Mapbox.Unity.Location.Location loc = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;
        if (_isInitialized && loc.Timestamp > 0 && (!searchHasRun || Vector2d.Distance(lastLoc, Mapbox.Unity.Utilities.Conversions.LatLonToMeters(loc.LatitudeLongitude)) > radius / 2f))
        {
            StopAllCoroutines();
            for (int i = 0; i < placeMarkers.Count; i++)
                Destroy(placeMarkers[i]);
            placeMarkers.Clear();
            string url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + loc.LatitudeLongitude.ToStringInv() + "&radius=" + radius + "&type=" + resourceLocationData[resourceIndex].LocTypes[locationIndex] + "&key=" + apiKey;
            StartCoroutine(RunSearch(url));
            lastLoc = Mapbox.Unity.Utilities.Conversions.LatLonToMeters(loc.LatitudeLongitude);
            searchHasRun = true;
        }
    }

    IEnumerator RunSearch(string url)
    {
        Debug.Log(url);
        UnityWebRequest request = new UnityWebRequest(url, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        Debug.Log(request.responseCode);
        Debug.Log(request.downloadHandler.text);
        Response response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
        Debug.Log(response.results.Length);
        for (int i = 0; i < response.results.Length; i++)
        {
            GameObject obj = Instantiate(resourceLocationData[resourceIndex].MarkerPrefab);
            obj.transform.SetParent(transform.parent);
            obj.GetComponent<MapMarker>().Init(this, _map, new Mapbox.Utils.Vector2d(response.results[i].geometry.location.lat, response.results[i].geometry.location.lng), response.results[i].name);
            placeMarkers.Add(obj);
        }

        if (!string.IsNullOrEmpty(response.next_page_token))
        {
            yield return new WaitForSeconds(2);

            Debug.Log(response.next_page_token);
            Mapbox.Unity.Location.Location loc = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;
            string nextUrl = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + loc.LatitudeLongitude.ToStringInv() + "&radius=" + radius + "&type=" + resourceLocationData[resourceIndex].LocTypes[locationIndex] + "&pagetoken=" + response.next_page_token + "&key=" + apiKey;
            StartCoroutine(RunSearch(nextUrl));
        }
        else
        {
            locationIndex++;
            if (locationIndex >= resourceLocationData[resourceIndex].LocTypes.Length)
            {
                resourceIndex++;
                locationIndex = 0;
            }
            if (resourceIndex < resourceLocationData.Count)
            {
                Mapbox.Unity.Location.Location loc = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;
                string nextUrl = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + loc.LatitudeLongitude.ToStringInv() + "&radius=" + radius + "&type=" + resourceLocationData[resourceIndex].LocTypes[locationIndex] + "&key=" + apiKey;
                StartCoroutine(RunSearch(nextUrl));
            }
        }
    }

    MapMarker activeMarker;
    public void ShowInteraction(MapMarker marker)
    {
        activeMarker = marker;
        raycaster.eventMask = LayerMask.GetMask();
        _mapMovement.enabled = false;
        resourceLocationData.Find((x) => { return x.ResourceType == marker.resourceType; }).InteractUI.Show(this);
    }

    public void CompleteInteraction()
    {
        if (activeMarker != null)
        {
            placeMarkers.Remove(activeMarker.gameObject);
            Destroy(activeMarker.gameObject);
            activeMarker = null;
        }
    }

    public void HideInteraction()
    {
        raycaster.eventMask = LayerMask.GetMask("MapMarker");
        _mapMovement.enabled = true;
    }
}