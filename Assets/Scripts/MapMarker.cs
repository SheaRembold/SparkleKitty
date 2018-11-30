using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine.EventSystems;
using Mapbox.Unity.MeshGeneration.Interfaces;

public class MapMarker : MonoBehaviour, IPointerClickHandler, IFeaturePropertySettable
{
    public SpriteRenderer icon;
    public SpriteRenderer back;
    [System.NonSerialized]
    public ResourceType resourceType;
    [System.NonSerialized]
    public ulong featureID;
    
    Dictionary<string, object> _props;
    public void Set(Dictionary<string, object> props)
    {
        _props = props;
    }

    public void Init(ulong id, ResourceType type, Sprite iconSprite, Color backColor)
    {
        featureID = id;
        resourceType = type;
        icon.sprite = iconSprite;
        //back.color = backColor;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        PlacesManager.Instance.ShowInteraction(this);
        SoundManager.Instance.PlayGroup("MapClick");

        /*foreach (var prop in _props)
        {
            Debug.Log(prop.Key + ":" + prop.Value);
        }*/
    }
}