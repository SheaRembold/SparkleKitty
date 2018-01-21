﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacableUI : MonoBehaviour
{
    public Transform ContentParent;
    public GameObject PlacableButtonPrefab;
    public PlacableData[] Placables;

    private void Start()
    {
        for (int i = 0; i < Placables.Length; i++)
        {
            GameObject obj = Instantiate(PlacableButtonPrefab);
            obj.transform.SetParent(ContentParent);
            (obj.transform as RectTransform).anchoredPosition = new Vector2(200 * i + 100, 0);
            obj.GetComponent<PlacableButton>().SetPlacable(Placables[i]);
        }
    }
}