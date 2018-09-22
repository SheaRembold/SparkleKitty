using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterController : MonoBehaviour
{
    [SerializeField]
    BookController book;
    [SerializeField]
    GameObject acceptParticle;

    public void ShowLetter()
    {
        PlacementManager.Instance.GrabLetter(this);
    }

    public void AcceptLetter()
    {
        GameObject obj = Instantiate(acceptParticle, transform.position, transform.rotation);
        obj.transform.localScale = transform.lossyScale;
        obj.GetComponent<MoveParticles>().StartMoving(book.transform);
        gameObject.SetActive(false);
    }
}