using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public struct PlacedInst
{
    public PlacableData Placable;
    public Vector3 Position;
}


public class PlacementArea : MonoBehaviour
{
    [SerializeField]
    PlacedInst[] startingInArea;

    protected List<Placable> placedInArea = new List<Placable>();
    bool areaDirty;

    public virtual void AddToArea(Placable placable)
    {
        if (!placedInArea.Contains(placable))
            placedInArea.Add(placable);
        areaDirty = true;
    }

    public virtual void RemoveFromArea(Placable placable)
    {
        placedInArea.Remove(placable);
        areaDirty = true;
    }

    public virtual void MoveInArea(Placable placable)
    {
        areaDirty = true;
    }
    
    public virtual void SetArea()
    {
        if (File.Exists(Application.persistentDataPath + "/" + gameObject.name + ".txt"))
        {
            string[] placedNames = File.ReadAllLines(Application.persistentDataPath + "/" + gameObject.name + ".txt");
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
        //UIManager.Instance.ShowSpeechUI(GetInArea("SparkleKitty").transform);
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
        File.WriteAllText(Application.persistentDataPath + "/" + gameObject.name + ".txt", builder.ToString());

        areaDirty = false;
    }
}