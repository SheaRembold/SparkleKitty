using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Mapbox.Unity.Map;
using Mapbox.Unity.Location;
using Mapbox.Examples;
using Mapbox.Utils;
using UnityEngine.EventSystems;
using Mapbox.Unity.MeshGeneration.Data;

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
    private Transform player;
    [SerializeField]
    private List<ResourceLocationData> resourceLocationData;
    [SerializeField]
    private PlacesModifier modifier;
    [SerializeField]
    private float maxDistance;

    PhysicsRaycaster raycaster;
    private QuadTreeCameraMovement _mapMovement;

    [SerializeField]
    GameObject Help;

    List<ulong> collected = new List<ulong>();
    List<DateTime> collectedTime = new List<DateTime>();

    private void Awake()
    {
        Instance = this;

        raycaster = GetComponent<PhysicsRaycaster>();
        _mapMovement = _map.GetComponent<QuadTreeCameraMovement>();

        Load();
    }

    void Start()
    {
        raycaster.eventMask = LayerMask.GetMask("MapMarker");
    }

    public void UpdateTile(UnityTile tile)
    {
        for (int i = 0; i < collectedTime.Count; i++)
        {
            if (DateTime.UtcNow.Subtract(collectedTime[i]).TotalDays >= 1.0)
            {
                collected.RemoveAt(i);
                collectedTime.RemoveAt(i);
                i--;
            }
        }
    }

    public bool CanAddMarker(ulong featureID)
    {
        return !collected.Contains(featureID);
    }
    
    MapMarker activeMarker;
    public void ShowInteraction(MapMarker marker)
    {
        if (Vector3.Distance(player.position, marker.transform.position) / _map.WorldRelativeScale > maxDistance)
        {
            //UIManager.Instance.ShowSpeechUI(_map.transform, "TooFar", false);
        }
        else
        {
            activeMarker = marker;
            raycaster.eventMask = LayerMask.GetMask();
            _mapMovement.enabled = false;
            resourceLocationData.Find((x) => { return x.ResourceType == marker.resourceType; }).InteractUI.Show();
            if (!HelpManager.Instance.HasShownHelp("MapInteract"))
                Help.SetActive(true);
        }
    }

    public void CompleteStep()
    {
        Help.SetActive(false);
        HelpManager.Instance.ShowHelp("MapInteract");
    }

    public void CompleteInteraction()
    {
        if (activeMarker != null)
        {
            modifier.CollectMarker(activeMarker);

            if (!collected.Contains(activeMarker.featureID))
            {
                collected.Add(activeMarker.featureID);
                collectedTime.Add(DateTime.UtcNow);
                Save();
            }

            activeMarker = null;
        }
    }

    public void HideInteraction()
    {
        raycaster.eventMask = LayerMask.GetMask("MapMarker");
        _mapMovement.enabled = true;
        activeMarker = null;
        Help.SetActive(false);
    }

    void Save()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        for (int i = 0; i < collected.Count; i++)
        {
            builder.AppendLine(collected[i].ToString());
            builder.AppendLine(collectedTime[i].ToFileTime().ToString());
        }
        File.WriteAllText(Application.persistentDataPath + "/places.txt", builder.ToString());
    }

    void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/places.txt"))
        {
            string[] collectedData = File.ReadAllLines(Application.persistentDataPath + "/places.txt");
            for (int i = 0; i < collectedData.Length; i += 2)
            {
                collected.Add(ulong.Parse(collectedData[i]));
                collectedTime.Add(DateTime.FromFileTime(long.Parse(collectedData[i + 1])));
            }
        }
    }
}