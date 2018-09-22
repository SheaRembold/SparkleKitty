using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashUI : MonoBehaviour
{
    [SerializeField]
    Image Scaling;
    [SerializeField]
    Image Fading;
    [SerializeField]
    float FlashSpeed = 1f;

    Vector2 startSize;

    private void Awake()
    {
        startSize = Scaling.rectTransform.sizeDelta;
    }

    private void OnEnable()
    {
        Scaling.gameObject.SetActive(true);
        Fading.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        Scaling.gameObject.SetActive(false);
        Fading.gameObject.SetActive(false);
    }

    private void Update()
    {
        Scaling.rectTransform.sizeDelta = Vector2.Lerp(Fading.rectTransform.sizeDelta, startSize, (Mathf.Sin(Time.time * FlashSpeed) + 3f) * 0.25f);
        //Fading.color = Color.Lerp(Color.white, Scaling.color, Mathf.Lerp(0f, 1f, (Mathf.Sin(Time.time * FlashSpeed) + 1f) * 0.25f));
    }
}