using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechUI : MonoBehaviour
{
    Transform target;
    
    public void ShowSpeech(Transform target)
    {
        this.target = target;
        gameObject.SetActive(true);
    }

    public void NextSpeech()
    {
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(target.transform.position);
        if (screenPos.x >= 0f && screenPos.x <= 1f && screenPos.y >= 0f && screenPos.y <= 1f && screenPos.z > 0f)
        {
            screenPos.y *= 1080f / Screen.width * Screen.height;
            (transform as RectTransform).anchoredPosition = new Vector2(0f, screenPos.y);
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}