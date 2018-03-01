using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class MapMarker : MonoBehaviour
{
    private AbstractMap _map;
    Vector2d _latLng;
    string _name;
    bool _isInitialized;

    public void Init(AbstractMap map, Vector2d latLng, string name)
    {
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
}