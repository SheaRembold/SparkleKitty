using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioInteractor : MonoBehaviour {

 
    public void UIClick()
    {
        MusicManager.instance.SimpleButtonClick();
    }

    public void MapClick()
    {
        MusicManager.instance.SimpleMapClick();
    }

}
