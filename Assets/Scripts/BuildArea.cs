using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildArea : PlacementArea
{
    [SerializeField]
    BuildWorkUI buildWorkUI;
    [SerializeField]
    BuildWorkUI upgradeWorkUI;
    [SerializeField]
    Transform upgradeLocation;
    List<Placable> available = new List<Placable>();
    GameObject builtObj;
    UpgradableData upgrading;
    GameObject upgradingObj;
    PlacementLocation placementLocation;

    bool areaSet;
    
    protected void OnEnable()
    {
        if (!areaSet)
        {
            SetArea();
            areaSet = true;
        }

        for (int i = 0; i < placedInArea.Count; i++)
        {
            PlayerManager.Instance.AddInventory(placedInArea[i].Data);
            Destroy(placedInArea[i].gameObject);
        }
        placedInArea.Clear();
        areaDirty = true;
    }

    public void SetPlacementLocation(PlacementLocation placementLocation)
    {
        this.placementLocation = placementLocation;
    }

    public void SetUpgrading(UpgradableData upgradable)
    {
        upgrading = upgradable;

        if (upgradingObj != null)
        {
            Destroy(upgradingObj);
            upgradingObj = null;
        }

        if (upgrading != null)
        {
            upgradingObj = Instantiate(upgrading.Prefab);
            upgradingObj.GetComponent<Placable>().Data = upgrading;
            SetLayer(upgradingObj.transform, 0);
            upgradingObj.transform.SetParent(upgradeLocation);
            upgradingObj.transform.localPosition = Vector3.zero;
            upgradingObj.transform.localRotation = Quaternion.identity;
            upgradingObj.transform.localScale = Vector3.one;
        }
    }

    void SetLayer(Transform obj, int layer)
    {
        obj.gameObject.layer = layer;
        for (int i = 0; i < obj.childCount; i++)
            SetLayer(obj.GetChild(i), layer);
    }

    public virtual void TryBuild()
    {
        CheckBuildables(DataManager.Instance.Toys);
    }

    public virtual void TryUpgrade()
    {
        bool validFound = false;
        if (upgrading.Upgrade != null)
        {
            available.Clear();
            available.AddRange(placedInArea);
            bool requMissing = false;
            for (int j = 0; j < upgrading.UpgradeRequirements.Length; j++)
            {
                int index = available.FindIndex((x) => { return x.Data == upgrading.UpgradeRequirements[j]; });
                if (index != -1)
                {
                    available.RemoveAt(index);
                }
                else
                {
                    requMissing = true;
                    break;
                }
            }
            if (!requMissing && available.Count == 0)
            {
                upgradeWorkUI.Show(this, upgrading.Upgrade);
                validFound = true;
            }
        }
        if (!validFound)
        {
            //UIManager.Instance.ShowSpeechUI(upgradeLocation, "WrongRecipe", false);
        }
    }

    protected void CheckBuildables(BuildableData[] buildables)
    {
        bool validFound = false;
        for (int i = 0; i < buildables.Length; i++)
        {
            available.Clear();
            available.AddRange(placedInArea);
            bool requMissing = false;
            for (int j = 0; j < buildables[i].BuildRequirements.Length; j++)
            {
                int index = available.FindIndex((x) => { return x.Data == buildables[i].BuildRequirements[j]; });
                if (index != -1)
                {
                    available.RemoveAt(index);
                }
                else
                {
                    requMissing = true;
                    break;
                }
            }
            if (!requMissing && available.Count == 0)
            {
                buildWorkUI.Show(this, buildables[i]);
                validFound = true;
                break;
            }
        }
        if (!validFound)
        {
            //UIManager.Instance.ShowSpeechUI(upgradeLocation, "WrongRecipe", false);
        }
    }
    
    public void Build(BuildableData buildable)
    {
        Vector3 avgPos = Vector3.zero;
        for (int i = 0; i < placedInArea.Count; i++)
        {
            avgPos += placedInArea[i].transform.position;
            Destroy(placedInArea[i].gameObject);
        }
        avgPos /= placedInArea.Count;
        placedInArea.Clear();
        areaDirty = true;
        if (placementLocation != null)
        {
            PlacementManager.Instance.SetTempArea(AreaType.Play);
            placementLocation.SetPlacable(buildable);
            PlacementManager.Instance.RestoreArea();
        }
        else
        {
            PlayerManager.Instance.AddInventory(buildable);
        }
        if (upgrading != null)
        {
            SetUpgrading(buildable as UpgradableData);
            builtObj = null;
        }
        else
        {
            builtObj = Instantiate(buildable.Prefab, avgPos, Quaternion.identity, transform);
        }
        StartCoroutine(StoreBuilt());
    }

    IEnumerator StoreBuilt()
    {
        yield return new WaitForSeconds(0.2f);
        float time = 0;
        float length = 0.5f;
        if (builtObj != null)
        {
            Vector3 startPos = builtObj.transform.position;
            Vector3 endPos = startPos + new Vector3(0f, 0f, -0.5f);
            while (time < length)
            {
                time += Time.deltaTime;
                builtObj.transform.localScale = Vector3.one * Mathf.Lerp(1f, 0f, time / length);
                builtObj.transform.position = Vector3.Lerp(startPos, endPos, time / length);
                yield return null;
            }
            Destroy(builtObj);
        }
        else
        {
            yield return new WaitForSeconds(length);
        }
        if (buildWorkUI.gameObject.activeSelf)
            buildWorkUI.Hide();
        if (upgradeWorkUI.gameObject.activeSelf)
            upgradeWorkUI.Hide();
        //if (placementLocation != null)
            //UIManager.Instance.GetCurrentBuildUI().Hide();
    }

    private void OnDisable()
    {
        if (builtObj != null)
            Destroy(builtObj);
    }
}