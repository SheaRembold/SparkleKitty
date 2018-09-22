using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mailbox : MonoBehaviour
{
    [SerializeField]
    Transform letterPosition;

    public void ShowLetter()
    {
        MailboxManager.Instance.ShowLetter(letterPosition);
    }
}
