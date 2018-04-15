using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayArea : PlacementArea
{
    public Transform CatSpawnPoint;
    public float SpawnDelay = 5f;

    List<CatData> validCats = new List<CatData>();
    CatData waitingCat;

    bool finishedLoading;

    public override void SetArea()
    {
        base.SetArea();

        finishedLoading = true;
        CheckForCats();
    }

    public override void AddToArea(Placable placable)
    {
        base.AddToArea(placable);

        if (finishedLoading)
            CheckForCats();
    }

    void CheckForCats()
    {
        if (GetInArea(PlacableDataType.Cat).Count < 2 && waitingCat == null)
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
        yield return new WaitForSeconds(SpawnDelay);

        PlacementManager.Instance.PlaceAt(waitingCat, CatSpawnPoint.localPosition);
        waitingCat = null;
    }

    void UpdateValidCats()
    {
        validCats.Clear();
        for (int i = 1; i < DataManager.Instance.Cats.Length; i++)
        {
            if (HasRequ(DataManager.Instance.Cats[i]))
            {
                validCats.Add(DataManager.Instance.Cats[i]);
            }
        }
    }

    bool HasRequ(CatData cat)
    {
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
            for (int j = 0; j < placedInArea.Count; j++)
            {
                if (placedInArea[j].Data == cat.OtherRequirements[i])
                {
                    hasRequ = true;
                    break;
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
}