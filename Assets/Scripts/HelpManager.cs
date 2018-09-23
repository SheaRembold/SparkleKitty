using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum TutorialStep { Start, Mail, GrabBook, PlaceTreat, CraftToy, PlaceToy, Done }

public class HelpManager : MonoBehaviour
{
    public static HelpManager Instance;

    public delegate void OnCompleteTutorialStep(TutorialStep currentStep);
    public OnCompleteTutorialStep onCompleteTutorialStep;

    [SerializeField]
    TutorialStep firstStep;
    [SerializeField]
    PlacableData[] firstGifts;

    public TutorialStep CurrentStep { get; private set; }
    
    List<string> shownHelp = new List<string>();

    private void Awake()
    {
        Instance = this;
        
        if (File.Exists(Application.persistentDataPath + "/help_" + DataManager.saveVersion + ".txt"))
        {
            string[] helpNames = File.ReadAllLines(Application.persistentDataPath + "/help_" + DataManager.saveVersion + ".txt");
            try
            {
                CurrentStep = (TutorialStep)System.Enum.Parse(typeof(TutorialStep), helpNames[0]);
                for (int i = 1; i < helpNames.Length; i++)
                    shownHelp.Add(helpNames[i]);
            }
            catch (System.Exception)
            {
                shownHelp.AddRange(helpNames);
            }
        }
        if (firstStep > CurrentStep)
            CurrentStep = firstStep;
    }

    public void ShowHelp(string name)
    {
        if (!shownHelp.Contains(name))
        {
            shownHelp.Add(name);

            SaveHelp();
        }
    }

    void SaveHelp()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        builder.AppendLine(CurrentStep.ToString());
        for (int i = 0; i < shownHelp.Count; i++)
        {
            builder.AppendLine(shownHelp[i]);
        }
        File.WriteAllText(Application.persistentDataPath + "/help_" + DataManager.saveVersion + ".txt", builder.ToString());
    }

    public bool HasShownHelp(string name)
    {
        return shownHelp.Contains(name);
    }

    public void CompleteTutorialStep(TutorialStep step)
    {
        if (step == CurrentStep)
        {
            CurrentStep++;
            if (CurrentStep == TutorialStep.Mail)
            {
                Dictionary<PlacableData, int> gifts = new Dictionary<PlacableData, int>();
                for (int i = 0; i < firstGifts.Length; i++)
                {
                    if (gifts.ContainsKey(firstGifts[i]))
                        gifts[firstGifts[i]]++;
                    else
                        gifts.Add(firstGifts[i], 1);
                }
                MailboxManager.Instance.AddLetter(new Letter(DataManager.Instance.Cats[0], gifts));
                MailboxManager.Instance.TurnOnHelp();
            }
            if (onCompleteTutorialStep != null)
                onCompleteTutorialStep(CurrentStep);
            SaveHelp();
        }
    }
}