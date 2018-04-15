using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mapbox.Unity.Map;
using Mapbox.Unity.Location;
using Mapbox.Examples;
using Mapbox.Utils;
using UnityEngine.EventSystems;
using Mapbox.Unity.MeshGeneration.Modifiers;
using Mapbox.Unity.MeshGeneration.Data;
using Mapbox.Unity.MeshGeneration.Interfaces;


public enum ResourceType { Recipe, Tower, Toy, Treat }

[CreateAssetMenu(menuName = "Places Modifier")]
public class PlacesModifier : GameObjectModifier
{
    [System.Serializable]
    public class ResourceLocationData
    {
        public ResourceType ResourceType;
        public List<string> LocTypes;
        public MapInteractUI InteractUI;
        public Sprite Icon;
        public Color BackColor;
    }

    [SerializeField]
    private GameObject MarkerPrefab;
    [SerializeField]
    private List<ResourceLocationData> resourceLocationData;

    [SerializeField]
    private bool _scaleDownWithWorld = false;

    private Dictionary<GameObject, GameObject> _objects;
    List<ulong> collected = new List<ulong>();

    public override void Initialize()
    {
        if (_objects == null)
        {
            _objects = new Dictionary<GameObject, GameObject>();
        }
        collected.Clear();
    }

    public override void Run(VectorEntity ve, UnityTile tile)
    {
        int selpos = ve.Feature.Points[0].Count / 2;
        var met = ve.Feature.Points[0][selpos];

        IFeaturePropertySettable settable = null;
        GameObject go = null;

        if (_objects.ContainsKey(ve.GameObject))
        {
            go = _objects[ve.GameObject];
            settable = go.GetComponent<IFeaturePropertySettable>();
            if (settable != null)
            {
                go = (settable as MonoBehaviour).gameObject;
                bool usedType = false;
                for (int i = 0; i < resourceLocationData.Count; i++)
                {
                    if (resourceLocationData[i].LocTypes.Contains(ve.Feature.Properties["type"] as string))
                    {
                        go.GetComponent<MapMarker>().Init(ve.Feature.Data.Id, resourceLocationData[i].ResourceType, resourceLocationData[i].Icon, resourceLocationData[i].BackColor);
                        usedType = true;
                        break;
                    }
                }
                go.SetActive(usedType && !collected.Contains(ve.Feature.Data.Id));
                go.name = ve.Feature.Data.Id.ToString();
                go.transform.localPosition = met;
                go.transform.localScale = Vector3.one;
                settable.Set(ve.Feature.Properties);
                if (!_scaleDownWithWorld)
                {
                    go.transform.localScale = Vector3.one / tile.TileScale;
                }
                return;
            }
        }
        else
        {
            for (int i = 0; i < resourceLocationData.Count; i++)
            {
                if (resourceLocationData[i].LocTypes.Contains(ve.Feature.Properties["type"] as string))
                {
                    go = Instantiate(MarkerPrefab);
                    go.GetComponent<MapMarker>().Init(ve.Feature.Data.Id, resourceLocationData[i].ResourceType, resourceLocationData[i].Icon, resourceLocationData[i].BackColor);
                    _objects.Add(ve.GameObject, go);
                    break;
                }
            }
            //go = Instantiate(resourceLocationData[0].MarkerPrefab);
            //_objects.Add(ve.GameObject, go);
        }

        if (go != null && go.activeSelf)
        {
            go.SetActive(!collected.Contains(ve.Feature.Data.Id));
            go.name = ve.Feature.Data.Id.ToString();
            go.transform.position = met;
            go.transform.SetParent(ve.GameObject.transform, false);
            go.transform.localScale = Vector3.one;

            settable = go.GetComponent<IFeaturePropertySettable>();
            if (settable != null)
            {
                settable.Set(ve.Feature.Properties);
            }

            if (!_scaleDownWithWorld)
            {
                go.transform.localScale = Vector3.one / tile.TileScale;
            }
        }
    }

    public void CollectMarker(MapMarker marker)
    {
        marker.gameObject.SetActive(false);
        if (!collected.Contains(marker.featureID))
            collected.Add(marker.featureID);
    }
}