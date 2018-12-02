using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class CatManager : MonoBehaviour
{
    public static CatManager Instance;

    public Color[] MoodColors;
    [SerializeField]
    float[] GiftFrequency;
    
    public class CatStatus
    {
        public bool Found;
        public float MoodValue;
        public int Mood;
        public TimeSpan LastGift;
        public TimeSpan LastLeave;
        public TimeSpan LastEnter;
    }

    Dictionary<CatData, CatStatus> statuses = new Dictionary<CatData, CatStatus>();
    bool statusDirty;
    
    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < DataManager.Instance.Cats.Length; i++)
            statuses.Add(DataManager.Instance.Cats[i], new CatStatus() { MoodValue = 0.5f, Mood = Mathf.RoundToInt(0.5f * (MoodColors.Length - 1)) });
        Load();
    }

    private void Update()
    {
        if (PlacementManager.Instance.IsReady && HelpManager.Instance.CurrentStep > TutorialStep.Start)
        {
            TimeSpan now = new TimeSpan(DateTime.UtcNow.Ticks);
            foreach (KeyValuePair<CatData, CatStatus> pair in statuses)
            {
                if (pair.Value.Found && pair.Value.Mood >= MoodColors.Length / 2 && now.Subtract(pair.Value.LastGift).TotalSeconds >= GiftFrequency[pair.Value.Mood])
                {
                    MailboxManager.Instance.AddWeightedRandomLetter(pair.Key);
                    pair.Value.LastGift = now;
                    statusDirty = true;
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (statusDirty)
        {
            //if (onInventoryChange != null)
                //onInventoryChange();
            Save();
        }
    }

    public void UpdateMood(CatData cat, float value)
    {
        if (statuses[cat].Mood < MoodColors.Length / 2)
            statuses[cat].LastGift = new TimeSpan(DateTime.UtcNow.Ticks);
        statuses[cat].MoodValue = value;
        statuses[cat].Mood = Mathf.RoundToInt(value * (MoodColors.Length - 1));
        statusDirty = true;
    }

    public float GetMoodValue(CatData cat)
    {
        return statuses[cat].MoodValue;
    }

    public int GetMood(CatData cat)
    {
        return statuses[cat].Mood;
    }

    public void MarkFound(CatData cat)
    {
        if (!statuses[cat].Found)
        {
            statuses[cat].LastGift = new TimeSpan(DateTime.UtcNow.Ticks);
            statuses[cat].Found = true;
            statusDirty = true;
        }
    }

    public bool IsFound(CatData cat)
    {
        return statuses[cat].Found;
    }

    public void LeaveArea(CatData cat)
    {
        statuses[cat].LastLeave = new TimeSpan(DateTime.UtcNow.Ticks);
        statusDirty = true;
    }
    
    public void LeftAreaAfter(CatData cat, float length)
    {
        statuses[cat].LastLeave = statuses[cat].LastEnter.Add(TimeSpan.FromSeconds(length));
        statusDirty = true;
    }

    public float TimeSinceLeave(CatData cat)
    {
        return (float)(new TimeSpan(DateTime.UtcNow.Ticks).Subtract(statuses[cat].LastLeave).TotalSeconds);
    }

    public void EnterArea(CatData cat)
    {
        statuses[cat].LastEnter = new TimeSpan(DateTime.UtcNow.Ticks);
        statusDirty = true;
    }

    public float TimeSinceEnter(CatData cat)
    {
        return (float)(new TimeSpan(DateTime.UtcNow.Ticks).Subtract(statuses[cat].LastEnter).TotalSeconds);
    }

    void Save()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        foreach (KeyValuePair<CatData, CatStatus> pair in statuses)
        {
            builder.AppendLine(pair.Key.name);
            builder.AppendLine(pair.Value.Found.ToString());
            builder.AppendLine(pair.Value.MoodValue.ToString());
            builder.AppendLine(pair.Value.LastGift.Ticks.ToString());
            builder.AppendLine(pair.Value.LastLeave.Ticks.ToString());
            builder.AppendLine(pair.Value.LastEnter.Ticks.ToString());
        }
        File.WriteAllText(Application.persistentDataPath + "/cats_" + DataManager.saveVersion + ".txt", builder.ToString());
        statusDirty = false;
    }

    void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/cats_" + DataManager.saveVersion + ".txt"))
        {
            string[] catsData = File.ReadAllLines(Application.persistentDataPath + "/cats_" + DataManager.saveVersion + ".txt");
            for (int i = 0; i < catsData.Length; i += 6)
            {
                CatData cat = DataManager.Instance.GetData(catsData[i]) as CatData;
                statuses[cat].Found = bool.Parse(catsData[i + 1]);
                statuses[cat].MoodValue = float.Parse(catsData[i + 2]);
                statuses[cat].LastGift = new TimeSpan(long.Parse(catsData[i + 3]));
                statuses[cat].LastLeave = new TimeSpan(long.Parse(catsData[i + 4]));
                statuses[cat].LastEnter = new TimeSpan(long.Parse(catsData[i + 5]));
                statuses[cat].Mood = Mathf.RoundToInt(statuses[cat].MoodValue * (MoodColors.Length - 1));
            }
        }
    }
}