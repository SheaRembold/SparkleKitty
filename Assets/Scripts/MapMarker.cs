using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine.EventSystems;

public class MapMarker : MonoBehaviour, IPointerClickHandler
{
    public ResourceType resourceType;
    Places _places;
    private AbstractMap _map;
    Vector2d _latLng;
    string _name;
    bool _isInitialized;

    public void Init(Places places, AbstractMap map, Vector2d latLng, string name)
    {
        _places = places;
        _map = map;
        _latLng = latLng;
        _name = name;
        _isInitialized = true;
    }

    void LateUpdate()
    {
        if (_isInitialized)
        {
            transform.localPosition = _map.GeoToWorldPosition(_latLng);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _places.ShowInteraction(this);
    }
}