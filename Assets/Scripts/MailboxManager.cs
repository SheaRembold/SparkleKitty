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
    Mailbox[] mailboxes;
    [SerializeField]
    LetterController letterRoot;
    [SerializeField]
    LetterUI letterUI;
    [SerializeField]
    GameObject helpParticles;
    [SerializeField]
    Outline helpOutline;

    Mailbox currentMailbox;
    Animator animator;
    AudioSource audioSource;
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
            mailboxes[i].gameObject.SetActive(i == (int)type);
        currentMailbox = mailboxes[(int)type];
        animator = currentMailbox.GetComponent<Animator>();
        audioSource = currentMailbox.GetComponent<AudioSource>();
        letterRoot.transform.SetParent(currentMailbox.transform);
        animator.SetBool("IsFull", letters.Count > 0);
        animator.SetBool("IsOpen", IsOpen);
    }

    private void OnEnable()
    {
        animator.SetBool("IsFull", letters.Count > 0);

        if (letters.Count > 0)
        {
            TurnOnHelp();
        }
        else
        {
            TurnOffHelp();
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            AddRandomLetter();
        }
    }

    void TurnOnHelp()
    {
        if (HelpManager.Instance.CurrentStep == TutorialStep.Mail)
        {
            //helpParticles.SetActive(true);
            helpOutline.enabled = true;
        }
        else
        {
            currentMailbox.flagOutline.enabled = true;
        }
    }

    void TurnOffHelp()
    {
        //helpParticles.SetActive(false);
        helpOutline.enabled = false;
        currentMailbox.flagOutline.enabled = false;
    }

    public override void Click(RaycastHit hit)
    {
        if (letters.Count > 0 && !IsOpen)
        {
            letterUI.SetLetter(letters[0], this);
            animator.SetBool("IsOpen", true);
            IsOpen = true;
            audioSource.Play();

            TurnOffHelp();
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
    
    void AddWeight(Dictionary<PlacableData, int> weights, PlacableData item, int weight, ref int totalWeight)
    {
        if (weights.ContainsKey(item))
            weights[item] += weight;
        else
            weights.Add(item, weight);
        totalWeight += weight;
    }
    
    public void AddWeightedRandomLetter(CatData cat)
    {
        List<CatData> unfoundCats = new List<CatData>();
        for (int i = 0; i < DataManager.Instance.Cats.Length; i++)
        {
            if (!CatManager.Instance.IsFound(DataManager.Instance.Cats[i]))
                unfoundCats.Add(DataManager.Instance.Cats[i]);
        }
        Dictionary<PlacableData, int> weights = new Dictionary<PlacableData, int>();
        int totalWeight = 0;
        for (int i = 0; i < DataManager.Instance.Cats.Length; i++)
        {
            int weight = 1;
            if (unfoundCats.Count > 0 && DataManager.Instance.Cats[i] == unfoundCats[0])
                weight = 20;
            else if (unfoundCats.Count > 1 && DataManager.Instance.Cats[i] == unfoundCats[1])
                weight = 7;
            else if (unfoundCats.Count > 2 && DataManager.Instance.Cats[i] == unfoundCats[2])
                weight = 2;
            AddWeight(weights, DataManager.Instance.TowerComponents[(int)DataManager.Instance.Cats[i].RequiredTowerType], weight, ref totalWeight);
            for (int j = 0; j < DataManager.Instance.Cats[i].OtherRequirements.Length; j++)
            {
                BuildableData buildable = DataManager.Instance.Cats[i].OtherRequirements[j] as BuildableData;
                if (!buildable.Unlimited)
                {
                    if (!PlayerManager.Instance.HasRecipe(buildable) && !HasRecipe(buildable))
                    {
                        for (int k = 0; k < DataManager.Instance.ToyRecipes.Length; k++)
                        {
                            if (DataManager.Instance.ToyRecipes[k].Product == buildable)
                            {
                                AddWeight(weights, DataManager.Instance.ToyRecipes[k], weight, ref totalWeight);
                                break;
                            }
                        }
                        for (int k = 0; k < DataManager.Instance.TreatRecipes.Length; k++)
                        {
                            if (DataManager.Instance.TreatRecipes[k].Product == buildable)
                            {
                                AddWeight(weights, DataManager.Instance.TreatRecipes[k], weight, ref totalWeight);
                                break;
                            }
                        }
                    }
                    for (int k = 0; k < buildable.BuildRequirements.Length; k++)
                    {
                        AddWeight(weights, buildable.BuildRequirements[k], weight, ref totalWeight);
                    }
                }
            }
        }
        Dictionary<PlacableData, int> gifts = new Dictionary<PlacableData, int>();
        for (int i = 0; i < 4; i++)
        {
            int randWeight = Random.Range(0, totalWeight);
            int currWeight = 0;
            foreach (KeyValuePair<PlacableData, int> pair in weights)
            {
                currWeight += pair.Value;
                if (currWeight > randWeight)
                {
                    if (gifts.ContainsKey(pair.Key))
                    {
                        if (pair.Key.DataType != PlacableDataType.ToyRecipe && pair.Key.DataType != PlacableDataType.TreatRecipe)
                            gifts[pair.Key]++;
                    }
                    else
                    {
                        gifts.Add(pair.Key, 1);
                    }
                    break;
                }
            }
        }
        AddLetter(new Letter(cat, gifts));
    }

    public void AddLetter(Letter letter)
    {
        letters.Add(letter);
        animator.SetBool("IsFull", true);
        Save();
        TurnOnHelp();
    }

    public void RemoveLetter(Letter letter)
    {
        letters.Remove(letter);
        animator.SetBool("IsFull", letters.Count > 0);
        animator.SetBool("IsOpen", false);
        letterRoot.AcceptLetter();
        IsOpen = false;
        HelpManager.Instance.CompleteTutorialStep(TutorialStep.Mail);
        Save();
        if (letters.Count > 0)
        {
            TurnOnHelp();
        }
    }

    public bool HasRecipe(PlacableData data)
    {
        for (int i = 0; i < letters.Count; i++)
        {
            foreach (KeyValuePair<PlacableData, int> pair in letters[i].Gifts)
            {
                RecipeData recipe = pair.Key as RecipeData;
                if (recipe != null && recipe.Product == data)
                {
                    return true;
                }
            }
        }
        return false;
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
