using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterController : MonoBehaviour
{
    private void OnEnable()
    {
        PlacementManager.Instance.GrabLetter(this);
    }
}