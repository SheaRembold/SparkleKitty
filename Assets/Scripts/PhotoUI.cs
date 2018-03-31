using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PhotoUI : MonoBehaviour
{
    [SerializeField]
    RawImage Image;
    
    public void ShowPhoto(PlacableData data)
    {
        Image.texture = PlayerManager.Instance.GetCatPhoto(data);
    }
}