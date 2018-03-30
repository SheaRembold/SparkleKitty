using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildArea : PlacementArea
{
    List<Placable> available = new List<Placable>();
    GameObject builtObj;

    public virtual void TryBuild()
    {
        CheckBuildables(DataManager.Instance.Toys);
    }

    protected void CheckBuildables(BuildableData[] buildables)
    {
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
                Build(buildables[i]);
                break;
            }
        }
    }

    void Build(BuildableData buildable)
    {
        Vector3 avgPos = Vector3.zero;
        for (int i = 0; i < placedInArea.Count; i++)
        {
            PlayerManager.Instance.RemoveInventory(placedInArea[i].Data);
            avgPos += placedInArea[i].transform.position;
            Destroy(placedInArea[i].gameObject);
        }
        avgPos /= placedInArea.Count;
        placedInArea.Clear();
        PlayerManager.Instance.AddInventory(buildable);
        builtObj = Instantiate(buildable.Prefab, avgPos, Quaternion.identity, transform);
        StartCoroutine(StoreBuilt());
    }

    IEnumerator StoreBuilt()
    {
        yield return new WaitForSeconds(0.2f);
        float time = 0;
        float length = 0.5f;
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

    private void OnDisable()
    {
        if (builtObj != null)
            Destroy(builtObj);
    }
}