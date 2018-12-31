using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class PlayArea : PlacementArea
{
    [SerializeField]
    PlacementLocation[] placementLocations;
    public Transform CatSpawnPoint;
    //public float SpawnDelay = 5f;
    [SerializeField]
    GameObject placingGrid;
    [SerializeField]
    GameObject[] fences;
    [SerializeField]
    MailboxManager mailbox;
    [SerializeField]
    GameObject book;
    [SerializeField]
    GameObject placeArrow;

    List<CatData> validCats = new List<CatData>();
    CatData waitingCat;

    bool finishedLoading;
    bool isPlaced;

    NavMeshData m_NavMesh;
    NavMeshDataInstance m_Instance;
    List<NavMeshBuildSource> m_Sources = new List<NavMeshBuildSource>();

    [SerializeField]
    MeshFilter navMeshVis;


    private void Awake()
    {
        for (int i = 0; i < placementLocations.Length; i++)
            placementLocations[i].Owner = this;
        placingGrid.SetActive(false);
        Load();
        for (int i = 0; i < placedInArea.Count; i++)
            placedInArea[i].gameObject.SetActive(false);
        book.SetActive(false);
        mailbox.gameObject.SetActive(false);
        UpgradableData currentTower = placementLocations[0].CurrentPlacable.Data as UpgradableData;
        DataManager.Instance.SetTowerLevel(currentTower.MaterialType, currentTower.Level);
        SetTower(currentTower);
    }
    
    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/" + gameObject.name + "_" + DataManager.saveVersion + ".txt"))
        {
            string[] placedNames = File.ReadAllLines(Application.persistentDataPath + "/" + gameObject.name + "_" + DataManager.saveVersion + ".txt");
            for (int i = 0; i < placementLocations.Length; i++)
            {
                PlacableData item = DataManager.Instance.GetData(placedNames[2 * i]);
                if (item != null)
                {
                    placementLocations[i].SetPlacable(item);
                    if (placementLocations[i].CurrentPlacable.GetComponent<ItemController>() != null)
                        placementLocations[i].CurrentPlacable.GetComponent<ItemController>().SetAmountLeft(float.Parse(placedNames[2 * i + 1]));
                }
            }
            for (int i = placementLocations.Length * 2; i < placedNames.Length; i += 4)
            {
                PlacableData item = DataManager.Instance.GetData(placedNames[i]);
                if (item != null)
                {
                    CatController cat = item.Prefab.GetComponent<CatController>();
                    if (cat == null || cat.StayForever || CatManager.Instance.TimeSinceEnter(item as CatData) < cat.StayLength[CatManager.Instance.GetMood(item as CatData)])
                    {
                        Vector3 pos = new Vector3(float.Parse(placedNames[i + 1]), float.Parse(placedNames[i + 2]), float.Parse(placedNames[i + 3]));
                        PlacementManager.Instance.PlaceAt(this, item, pos);
                    }
                    else
                    {
                        CatManager.Instance.LeftAreaAfter(item as CatData, cat.StayLength[CatManager.Instance.GetMood(item as CatData)]);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < placementLocations.Length; i++)
            {
                placementLocations[i].SetPlacable(placementLocations[i].StartingPlacable);
            }
            for (int i = 0; i < startingInArea.Length; i++)
            {
                PlacementManager.Instance.PlaceAt(this, startingInArea[i].Placable, startingInArea[i].Position, startingInArea[i].Rotation);
            }
        }
        
        finishedLoading = true;
    }

    public override void SetArea()
    {
        for (int i = 0; i < placedInArea.Count; i++)
            placedInArea[i].gameObject.SetActive(true);
        book.SetActive(true);
        mailbox.gameObject.SetActive(true);
        placeArrow.SetActive(false);

        m_NavMesh = new NavMeshData();
        m_Instance = NavMesh.AddNavMeshData(m_NavMesh, transform.position, transform.rotation);
        UpdateNavMesh();

        isPlaced = true;
        CheckForCats();
        if (!HelpManager.Instance.HasShownHelp("IntroAR"))
        {
            //UIManager.Instance.ShowSpeechUI(GetInArea("SparkleKitty").transform, "IntroAR", true);
        }
    }

    public PlacementLocation GetPlacementLocation()
    {
        return placementLocations[0];
    }

    public void SetTower(UpgradableData tower, bool showEffect = false)
    {
        placementLocations[0].SetPlacable(tower, showEffect);
        for (int i = 0; i < fences.Length; i++)
            fences[i].SetActive(i == (int)tower.MaterialType);
        mailbox.SetMailbox(tower.MaterialType);

        if (m_NavMesh != null)
            UpdateNavMesh();
    }

    void UpdateNavMesh()
    {
        NavMeshSourceTag.Collect(ref m_Sources, transform);
        NavMeshBuildSettings defaultBuildSettings = NavMesh.GetSettingsByID(0);
        defaultBuildSettings.agentClimb *= transform.lossyScale.x;
        defaultBuildSettings.agentHeight *= transform.lossyScale.x;
        defaultBuildSettings.agentRadius *= transform.lossyScale.x;
        defaultBuildSettings.minRegionArea *= transform.lossyScale.x;
        defaultBuildSettings.overrideVoxelSize = true;
        defaultBuildSettings.voxelSize = defaultBuildSettings.agentRadius / 3f;
        Bounds bounds = new Bounds(Vector3.zero, Vector3.Scale(new Vector3(3f, 3f, 3f), transform.lossyScale));
        NavMeshBuilder.UpdateNavMeshData(m_NavMesh, defaultBuildSettings, m_Sources, bounds);
        /*
        navMeshVis.transform.SetParent(null, false);
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
        Mesh mesh = navMeshVis.mesh;
        mesh.Clear();
        mesh.vertices = triangulation.vertices;
        mesh.triangles = triangulation.indices;
        mesh.RecalculateNormals();
        */
        for (int i = 0; i < placedInArea.Count; i++)
        {
            if (placedInArea[i].GetComponent<NavMeshSourceTag>() == null)
            {
                placedInArea[i].transform.position = PlacementManager.Instance.GetNavPos(placedInArea[i].transform.position);
                MoveInArea(placedInArea[i]);
            }
        }
    }

    public override void AddToArea(Placable placable)
    {
        base.AddToArea(placable);

        if (isPlaced)
            CheckForCats();
    }

    public void ShowPlacing(bool show)
    {
        placingGrid.SetActive(show);
    }

    public void CheckForCats()
    {
        if (/*GetInArea(PlacableDataType.Cat).Count < 2 &&*/ waitingCat == null)
        {
            UpdateValidCats();
            if (validCats.Count > 0)
            {
                waitingCat = validCats[Random.Range(0, validCats.Count)];
                StartCoroutine(WaitToSpawnCat());
            }
        }
    }

    public override void RemoveFromArea(Placable placable)
    {
        base.RemoveFromArea(placable);

        CheckWaitingCat();
    }

    public void CheckWaitingCat()
    {
        if (waitingCat != null)
        {
            UpdateValidCats();
            if (!validCats.Contains(waitingCat))
            {
                if (validCats.Count > 0)
                {
                    waitingCat = validCats[Random.Range(0, validCats.Count)];
                }
                else
                {
                    StopAllCoroutines();
                }
            }
        }
    }

    IEnumerator WaitToSpawnCat()
    {
        if (HelpManager.Instance.CurrentStep == TutorialStep.PlaceToy)
            yield return new WaitForSeconds(1f);
        else
            yield return new WaitForSeconds(waitingCat.Prefab.GetComponent<CatController>().SpawnWait[CatManager.Instance.GetMood(waitingCat)]);

        Placable cat = PlacementManager.Instance.PlaceAt(this, waitingCat, CatSpawnPoint.localPosition, true);
        CatManager.Instance.EnterArea(waitingCat);
        cat.GetComponent<CatController>().EnterArea();
        waitingCat = null;
        CheckForCats();
    }

    void UpdateValidCats()
    {
        validCats.Clear();
        for (int i = 1; i < DataManager.Instance.Cats.Length; i++)
        {
            if (GetInArea(DataManager.Instance.Cats[i]).Count == 0 && HasRequ(DataManager.Instance.Cats[i]))
            {
                validCats.Add(DataManager.Instance.Cats[i]);
            }
        }
    }

    bool HasRequ(CatData cat)
    {
        if (CatManager.Instance.TimeSinceLeave(cat) < cat.Prefab.GetComponent<CatController>().AwayLength[CatManager.Instance.GetMood(cat)])
            return false;

        List<Placable> towers = GetInArea(PlacableDataType.Tower);
        bool hasTower = false;
        for (int i = 0; i < towers.Count; i++)
        {
            if ((towers[i].Data as UpgradableData).MaterialType == cat.RequiredTowerType
                && (towers[i].Data as UpgradableData).Level >= cat.RequiredTowerLevel)
            {
                hasTower = true;
                break;
            }
        }
        if (!hasTower)
            return false;

        for (int i = 0; i < cat.OtherRequirements.Length; i++)
        {
            bool hasRequ = false;
            if (PlacementManager.Instance.CurrentAttached != null && PlacementManager.Instance.CurrentAttached.Data == cat.OtherRequirements[i])
            {
                hasRequ = true;
            }
            else
            {
                for (int j = 0; j < placedInArea.Count; j++)
                {
                    if (placedInArea[j].Data == cat.OtherRequirements[i]
                        && (placedInArea[i].GetComponent<ItemController>() == null || placedInArea[i].GetComponent<ItemController>().AnyLeft()))
                    {
                        hasRequ = true;
                        break;
                    }
                }
            }
            if (!hasRequ)
                return false;
        }

        return true;
    }

    Placable GetInArea(string name)
    {
        for (int i = 0; i < placedInArea.Count; i++)
        {
            if (placedInArea[i].Data.name == name)
                return placedInArea[i];
        }
        return null;
    }

    List<Placable> tempPlacables = new List<Placable>();
    public List<Placable> GetInArea(PlacableDataType dataType)
    {
        tempPlacables.Clear();
        for (int i = 0; i < placedInArea.Count; i++)
        {
            if (placedInArea[i].Data.DataType == dataType)
                tempPlacables.Add(placedInArea[i]);
        }
        return tempPlacables;
    }

    public List<Placable> GetInArea(PlacableData[] datas)
    {
        tempPlacables.Clear();
        for (int i = 0; i < placedInArea.Count; i++)
        {
            for (int j = 0; j < datas.Length; j++)
            {
                if (datas[j] == placedInArea[i].Data)
                {
                    tempPlacables.Add(placedInArea[i]);
                    break;
                }
            }
        }
        return tempPlacables;
    }

    public List<Placable> GetInArea(PlacableData data)
    {
        tempPlacables.Clear();
        for (int i = 0; i < placedInArea.Count; i++)
        {
            if (data == placedInArea[i].Data)
            {
                tempPlacables.Add(placedInArea[i]);
            }
        }
        return tempPlacables;
    }

    bool IsAtLoc(Placable placable)
    {
        for (int i = 0; i < placementLocations.Length; i++)
        {
            if (placementLocations[i].CurrentPlacable == placable)
                return true;
        }
        return false;
    }
    
    protected override void Save()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        for (int i = 0; i < placementLocations.Length; i++)
        {
            builder.AppendLine(placementLocations[i].CurrentPlacable == null ? "NULL" : placementLocations[i].CurrentPlacable.Data.name);
            if (placementLocations[i].CurrentPlacable == null || placementLocations[i].CurrentPlacable.GetComponent<ItemController>() == null)
                builder.AppendLine("1");
            else
                builder.AppendLine(placementLocations[i].CurrentPlacable.GetComponent<ItemController>().AmountLeft.ToString());
        }
        for (int i = 0; i < placedInArea.Count; i++)
        {
            if (!IsAtLoc(placedInArea[i]))
            {
                builder.AppendLine(placedInArea[i].Data.name);
                builder.AppendLine(placedInArea[i].transform.localPosition.x.ToString());
                builder.AppendLine(placedInArea[i].transform.localPosition.y.ToString());
                builder.AppendLine(placedInArea[i].transform.localPosition.z.ToString());
            }
        }
        File.WriteAllText(Application.persistentDataPath + "/" + gameObject.name + "_" + DataManager.saveVersion + ".txt", builder.ToString());

        areaDirty = false;
    }
}