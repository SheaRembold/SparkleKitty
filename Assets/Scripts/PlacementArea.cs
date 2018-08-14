using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public struct PlacedInst
{
    public PlacableData Placable;
    public Vector3 Position;
    public Vector3 Rotation;
}


public class PlacementArea : MonoBehaviour
{
    protected static int saveVersion = 2;
    [SerializeField]
    protected PlacedInst[] startingInArea;
    public Transform Contents;
    public bool AllowMovement = true;

    protected List<Placable> placedInArea = new List<Placable>();
    protected bool areaDirty;

    public void MarkAsDirty()
    {
        areaDirty = true;
    }

    public virtual void AddToArea(Placable placable)
    {
        if (!placedInArea.Contains(placable))
        {
            placedInArea.Add(placable);
            placable.AddedToArea();
        }
        areaDirty = true;
    }

    public virtual void RemoveFromArea(Placable placable)
    {
        if (placedInArea.Contains(placable))
        {
            placedInArea.Remove(placable);
            placable.RemovedFromArea();
        }
        areaDirty = true;
    }

    public virtual void MoveInArea(Placable placable)
    {
        areaDirty = true;
    }
    
    public virtual void SetArea()
    {
        if (File.Exists(Application.persistentDataPath + "/" + gameObject.name + "_" + saveVersion + ".txt"))
        {
            string[] placedNames = File.ReadAllLines(Application.persistentDataPath + "/" + gameObject.name + "_" + saveVersion + ".txt");
            for (int i = 0; i < placedNames.Length; i+= 4)
            {
                PlacableData item = DataManager.Instance.GetData(placedNames[i]);
                if (item != null)
                {
                    Vector3 pos = new Vector3(float.Parse(placedNames[i + 1]), float.Parse(placedNames[i + 2]), float.Parse(placedNames[i + 3]));
                    PlacementManager.Instance.PlaceAt(item, pos);
                }
            }
        }
        else
        {
            for (int i = 0; i < startingInArea.Length; i++)
            {
                PlacementManager.Instance.PlaceAt(startingInArea[i].Placable, startingInArea[i].Position);
            }
        }
    }

    protected virtual void LateUpdate()
    {
        if (areaDirty)
            Save();
    }

    protected virtual void Save()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        for (int i = 0; i < placedInArea.Count; i++)
        {
            builder.AppendLine(placedInArea[i].Data.name);
            builder.AppendLine(placedInArea[i].transform.localPosition.x.ToString());
            builder.AppendLine(placedInArea[i].transform.localPosition.y.ToString());
            builder.AppendLine(placedInArea[i].transform.localPosition.z.ToString());
        }
        File.WriteAllText(Application.persistentDataPath + "/" + gameObject.name + "_" + saveVersion + ".txt", builder.ToString());

        areaDirty = false;
    }
}