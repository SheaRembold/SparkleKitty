using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterUI : MonoBehaviour
{
    [SerializeField]
    Image sender;
    [SerializeField]
    GameObject giftPrefab;
    [SerializeField]
    Transform content;
    [SerializeField]
    Button acceptButton;
    [SerializeField]
    GameObject flashPrefab;

    Letter letter;
    MailboxManager mailbox;
    List<GameObject> gifts = new List<GameObject>();

    GameObject acceptFlash;

    public void SetLetter(Letter letter, MailboxManager mailbox)
    {
        this.letter = letter;
        this.mailbox = mailbox;

        sender.sprite = letter.Sender.Icon;
        int giftIndex = 0;
        foreach (KeyValuePair<PlacableData, int> pair in letter.Gifts)
        {
            GameObject gift = null;
            if (giftIndex < gifts.Count)
            {
                gift = gifts[giftIndex];
                gift.SetActive(true);
            }
            else
            {
                gift = Instantiate(giftPrefab);
                gift.transform.SetParent(content, false);
                (gift.transform as RectTransform).anchoredPosition = new Vector2(195 + 230 * (giftIndex / 2), -450 - 230 * (giftIndex % 2));
                gifts.Add(gift);
            }
            gift.GetComponentInChildren<Image>().sprite = pair.Key.Icon;
            gift.GetComponentInChildren<Text>().text = pair.Value.ToString();
            giftIndex++;
        }
        for (int i = giftIndex; i < gifts.Count; i++)
            gifts[i].SetActive(false);
        acceptButton.interactable = true;
        if (HelpManager.Instance.CurrentStep == TutorialStep.Mail)
        {
            acceptFlash = Instantiate(flashPrefab);
            acceptFlash.GetComponent<FlashUI>().SetTarget(acceptButton.targetGraphic as Image);
        }
    }

    public void Accept()
    {
        foreach (KeyValuePair<PlacableData, int> pair in letter.Gifts)
        {
            for (int i = 0; i < pair.Value; i++)
                PlayerManager.Instance.AddInventory(pair.Key);
        }
        mailbox.RemoveLetter(letter);
        acceptButton.interactable = false;
        if (acceptFlash != null)
            Destroy(acceptFlash);
    }
}
