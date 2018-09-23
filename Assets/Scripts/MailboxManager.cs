using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Letter
{
    public CatData Sender;
    public Dictionary<PlacableData, int> Gifts;

    public Letter()
    {
    }

    public Letter(CatData sender, Dictionary<PlacableData, int> gifts)
    {
        Sender = sender;
        Gifts = gifts;
    }
}

public class MailboxManager : Clickable
{
    public static MailboxManager Instance;

    [SerializeField]
    GameObject[] mailboxes;
    [SerializeField]
    LetterController letterRoot;
    [SerializeField]
    LetterUI letterUI;
    [SerializeField]
    GameObject helpParticles;

    Animator animator;
    List<Letter> letters = new List<Letter>();
    bool IsOpen;

    private void Awake()
    {
        Instance = this;

        letterRoot.gameObject.SetActive(false);

        Load();
    }

    public void SetMailbox(MaterialType type)
    {
        for (int i = 0; i < mailboxes.Length; i++)
            mailboxes[i].SetActive(i == (int)type);
        animator = mailboxes[(int)type].GetComponent<Animator>();
        letterRoot.transform.SetParent(mailboxes[(int)type].transform);
        animator.SetBool("IsFull", letters.Count > 0);
        animator.SetBool("IsOpen", IsOpen);
    }

    private void OnEnable()
    {
        animator.SetBool("IsFull", letters.Count > 0);

        if (HelpManager.Instance.CurrentStep == TutorialStep.Mail)
        {
            TurnOnHelp();
        }
    }

    public void TurnOnHelp()
    {
        helpParticles.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            AddRandomLetter();
        }
    }

    public override void Click(RaycastHit hit)
    {
        if (letters.Count > 0 && !IsOpen)
        {
            letterUI.SetLetter(letters[0], this);
            animator.SetBool("IsOpen", true);
            IsOpen = true;
        }
    }

    private void AddRandomLetter()
    {
        AddRandomLetter(DataManager.Instance.Cats[Random.Range(0, DataManager.Instance.Cats.Length)]);
    }

    public void AddRandomLetter(CatData cat)
    {
        AddLetter(new Letter(cat,
            new Dictionary<PlacableData, int>() { { DataManager.Instance.TowerComponents[Random.Range(0, DataManager.Instance.TowerComponents.Length)], Random.Range(1, 4) },
                { DataManager.Instance.ToyComponents[Random.Range(0, DataManager.Instance.ToyComponents.Length)], Random.Range(1, 4) },
                { DataManager.Instance.TreatComponents[Random.Range(0, DataManager.Instance.TreatComponents.Length)], Random.Range(1, 4) }}));
    }

    public void AddLetter(Letter letter)
    {
        letters.Add(letter);
        animator.SetBool("IsFull", true);
        Save();
    }

    public void RemoveLetter(Letter letter)
    {
        letters.Remove(letter);
        animator.SetBool("IsFull", letters.Count > 0);
        animator.SetBool("IsOpen", false);
        letterRoot.AcceptLetter();
        IsOpen = false;
        helpParticles.SetActive(false);
        HelpManager.Instance.CompleteTutorialStep(TutorialStep.Mail);
        Save();
    }

    public void ShowLetter(Transform from)
    {
        letterRoot.transform.position = from.position;
        letterRoot.transform.rotation = from.rotation;
        letterRoot.gameObject.SetActive(true);
        letterRoot.ShowLetter();
    }

    private void Save()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        for (int i = 0; i < letters.Count; i++)
        {
            builder.AppendLine(letters[i].Sender.name);
            foreach(KeyValuePair<PlacableData, int> pair in letters[i].Gifts)
                builder.AppendLine(pair.Key.name + "," + pair.Value);
            builder.AppendLine("----");
        }
        File.WriteAllText(Application.persistentDataPath + "/mailbox_" + DataManager.saveVersion + ".txt", builder.ToString());
    }

    private void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/mailbox_" + DataManager.saveVersion + ".txt"))
        {
            string[] lettersFile = File.ReadAllLines(Application.persistentDataPath + "/mailbox_" + DataManager.saveVersion + ".txt");
            int letterIndex = 0;
            Letter letter = null;
            for (int i = 0; i < lettersFile.Length; i++)
            {
                if (letterIndex == 0)
                {
                    letter = new Letter();
                    letter.Sender = DataManager.Instance.GetData(lettersFile[i]) as CatData;
                    letter.Gifts = new Dictionary<PlacableData, int>();
                    letterIndex++;
                }
                else
                {
                    if (lettersFile[i] == "----")
                    {
                        letters.Add(letter);
                        letterIndex = 0;
                    }
                    else
                    {
                        string[] gift = lettersFile[i].Split(',');
                        letter.Gifts.Add(DataManager.Instance.GetData(gift[0]), int.Parse(gift[1]));
                        letterIndex++;
                    }
                }
            }
        }
    }
}
