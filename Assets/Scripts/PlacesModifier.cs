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
    List<string> allTypes = new List<string>() { "Memorial", "Construction", "Park", "Common", "Apartments", "Church", "Bus Station", "Police", "Public Building", "Hotel", "Bar", "Restaurant", "Gift", "Library", "Monument", "Museum", "Pub", "Bakery", "Yes", "Convenience", "Arts Centre", "Commercial", "Lawyer", "Marketplace", "Bank", "Government", "Drinking Water", "Sports Centre", "Place Of Worship", "Retail", "Theatre", "Clothes", "Fast Food", "Deli", "School", "Attraction", "Stadium", "Community Centre", "Mall", "Residential", "Industrial", "University", "College", "Playground", "House", "Farmyard", "Hairdresser", "Cafe", "Shelter", "Social Facility", "Cemetery", "Farmland", "Doityourself", "Department Store", "Supermarket", "Golf Course", "Jewelry", "Electronics", "Sports", "Pharmacy", "Fuel", "Books", "Beauty", "Stationery", "Houseware", "Made Mine", "Grave Yard", "Wood", "Second Hand", "Variety Store", "Alcohol", "Fire Station", "Hospital", };
    //"Common", "Apartments", "Bus Station", "Public Building", "Yes", "Commercial", "Lawyer", "Government", "Drinking Water", "Sports Centre", "Stadium", "Community Centre", "Residential", "Industrial", "House", "Farmyard", "Shelter", "Social Facility", "Farmland", "Doityourself", "Golf Course", "Made Mine", "Second Hand", "Alcohol", 

    public override void Initialize()
    {
        if (_objects == null)
        {
            _objects = new Dictionary<GameObject, GameObject>();
        }
    }

    public override void Run(VectorEntity ve, UnityTile tile)
    {
        int selpos = ve.Feature.Points[0].Count / 2;
        var met = ve.Feature.Points[0][selpos];

        IFeaturePropertySettable settable = null;
        GameObject go = null;

        if (!allTypes.Contains(ve.Feature.Properties["type"] as string))
            allTypes.Add(ve.Feature.Properties["type"] as string);

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
                        //go.GetComponent<MapMarker>().Set(ve.Feature.Properties);
                        go.GetComponent<MapMarker>().Init(ve.Feature.Data.Id, resourceLocationData[i].ResourceType, resourceLocationData[i].Icon, resourceLocationData[i].BackColor);
                        usedType = true;
                        break;
                    }
                }
                go.SetActive(usedType && PlacesManager.Instance.CanAddMarker(ve.Feature.Data.Id));
                go.name = ve.Feature.Data.Id.ToString();
                go.transform.localPosition = met;
                go.transform.localScale = Vector3.one;
                //settable.Set(ve.Feature.Properties);
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
                    //go.GetComponent<MapMarker>().Set(ve.Feature.Properties);
                    go.GetComponent<MapMarker>().Init(ve.Feature.Data.Id, resourceLocationData[i].ResourceType, resourceLocationData[i].Icon, resourceLocationData[i].BackColor);
                    _objects.Add(ve.GameObject, go);
                    break;
                }
            }
        }

        if (go != null && go.activeSelf)
        {
            go.SetActive(PlacesManager.Instance.CanAddMarker(ve.Feature.Data.Id));
            go.name = ve.Feature.Data.Id.ToString();
            go.transform.position = met;
            go.transform.SetParent(ve.GameObject.transform, false);
            go.transform.localScale = Vector3.one;

            /*settable = go.GetComponent<IFeaturePropertySettable>();
            if (settable != null)
            {
                settable.Set(ve.Feature.Properties);
            }*/

            if (!_scaleDownWithWorld)
            {
                go.transform.localScale = Vector3.one / tile.TileScale;
            }
        }
    }

    public void CollectMarker(MapMarker marker)
    {
        marker.gameObject.SetActive(false);
    }

    bool anyContain(string type)
    {
        for (int i = 0; i < resourceLocationData.Count; i++)
        {
            if (resourceLocationData[i].LocTypes.Contains(type))
            {
                return true;
            }
        }
        return false;
    }

    public void LogAllTypes()
    {
        string allStr = "";
        for (int i = 0; i < allTypes.Count; i++)
            allStr += "\"" + allTypes[i] + "\", ";
        Debug.Log(allTypes.Count);
        Debug.Log(allStr);

        allStr = "";
        for (int i = 0; i < allTypes.Count; i++)
            if (!anyContain(allTypes[i]))
                allStr += "\"" + allTypes[i] + "\", ";
        Debug.Log(allStr);


        allStr = "";
        for (int i = 0; i < resourceLocationData.Count; i++)
            for (int j = 0; j < resourceLocationData[i].LocTypes.Count; j++)
                if (!allTypes.Contains(resourceLocationData[i].LocTypes[j]))
                    allStr += "\"" + resourceLocationData[i].LocTypes[j] + "\", ";
        Debug.Log(allStr);
    }
}