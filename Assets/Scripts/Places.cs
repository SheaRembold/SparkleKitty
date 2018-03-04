using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mapbox.Unity.Map;
using Mapbox.Unity.Location;

public class Places : MonoBehaviour
{
    [SerializeField]
    private string apiKey;
    [SerializeField]
    private AbstractMap _map;
    [SerializeField]
    private GameObject markerPrefab;

    bool _isInitialized;
    bool searchHasRun;

    public class Response
    {
        public Result[] results;
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

    void Start()
    {
        _map.OnInitialized += () => _isInitialized = true;
    }

    void LateUpdate()
    {
        Mapbox.Unity.Location.Location loc = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;
        if (_isInitialized && loc.Timestamp > 0 && !searchHasRun)
        {
            StartCoroutine(RunSearch());
            searchHasRun = true;
        }
    }

    IEnumerator RunSearch()
    {
        Mapbox.Unity.Location.Location loc = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;
        string url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + loc.LatitudeLongitude.ToStringInv() + "&radius=500&key=" + apiKey;
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
            GameObject obj = Instantiate(markerPrefab);
            obj.transform.SetParent(transform.parent);
            obj.GetComponent<MapMarker>().Init(_map, new Mapbox.Utils.Vector2d(response.results[i].geometry.location.lat, response.results[i].geometry.location.lng), response.results[i].name);
        }
    }
}