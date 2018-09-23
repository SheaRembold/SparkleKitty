using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatPageUI : ItemPageUI
{
    [SerializeField]
    List<Image> moods = new List<Image>();

    protected override void UpdateRecipe()
    {
        if (current < 0)
            current = DataManager.Instance.Cats.Length - 1;
        else if (current >= DataManager.Instance.Cats.Length)
            current = 0;

        bool found = CatManager.Instance.IsFound(DataManager.Instance.Cats[current]);
        NameLabel.text = found ? DataManager.Instance.Cats[current].Name : "???";
        ItemImage.sprite = DataManager.Instance.Cats[current].Icon;
        ItemImage.color = found ? Color.white : Color.black;
        PageLabel.text = (current + 1) + " / " + DataManager.Instance.Cats.Length;

        int currentMood = CatManager.Instance.GetMood(DataManager.Instance.Cats[current]);
        for (int i = 0; i < CatManager.Instance.MoodColors.Length; i++)
        {
            if (i == currentMood)
                moods[i].color = CatManager.Instance.MoodColors[i];
            else
                moods[i].color = CatManager.Instance.MoodColors[i] * new Color(0.25f, 0.25f, 0.25f, 1f);
        }
        
        SetRequButton(0, DataManager.Instance.GetTower(DataManager.Instance.Cats[current].RequiredTowerType, DataManager.Instance.Cats[current].RequiredTowerLevel));
        for (int i = 0; i < DataManager.Instance.Cats[current].OtherRequirements.Length; i++)
            SetRequButton(i + 1, DataManager.Instance.Cats[current].OtherRequirements[i]);
        for (int i = DataManager.Instance.Cats[current].OtherRequirements.Length + 1; i < requs.Count; i++)
            requs[i].SetActive(false);

        if (HelpManager.Instance.CurrentStep == TutorialStep.PlaceTreat)
        {
            if (buttonFlash == null)
                buttonFlash = Instantiate(Book.uiManager.flashPrefab);
            buttonFlash.GetComponent<FlashUI>().SetTarget(requs[1].GetComponent<Button>().targetGraphic);
            for (int i = 0; i < requs.Count; i++)
                requs[i].GetComponent<Button>().interactable = false;
            requs[1].GetComponent<Button>().interactable = true;
        }
        else
        {
            if (buttonFlash != null)
                Destroy(buttonFlash);
            for (int i = 0; i < requs.Count; i++)
                requs[i].GetComponent<Button>().interactable = true;
        }
    }

    void SetRequButton(int i, PlacableData itemData)
    {
        requs[i].SetActive(true);
        PlacableButton image = requs[i].GetComponent<PlacableButton>();
        image.SetPlacable(itemData);
        Button button = requs[i].GetComponent<Button>();
        button.onClick.AddListener(new UnityEngine.Events.UnityAction(() => { Book.ShowItem(itemData); }));
    }
}