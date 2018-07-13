using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechUI : MonoBehaviour
{
    [SerializeField]
    RectTransform bubble;
    [SerializeField]
    RectTransform arrow;
    [SerializeField]
    HelpTextData helpTextData;
    [SerializeField]
    Text text;
    Transform target;
    string speechName;
    string[] lines;
    int current;
    bool isOnscreen;

    private void Awake()
    {
        helpTextData.Init();
    }

    public void ShowSpeech(Transform target, string speech, bool isOnscreen)
    {
        gameObject.SetActive(true);
        arrow.gameObject.SetActive(isOnscreen);
        this.target = target;
        this.isOnscreen = isOnscreen;
        speechName = speech;
        lines = helpTextData.GetHelpText(speech);
        current = 0;
        text.text = lines[current];
    }

    public void NextSpeech()
    {
        current++;
        if (current < lines.Length)
        {
            text.text = lines[current];
        }
        else
        {
            //UIManager.Instance.HideSpeechUI();
            gameObject.SetActive(false);
            PlayerManager.Instance.ShowHelp(speechName);
        }
    }

    private void LateUpdate()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(target.position);
        bubble.anchoredPosition = new Vector2(0f, Mathf.Clamp(screenPos.y* 1080f / Screen.width * Screen.height, -200f, 1420f));
        if (isOnscreen && screenPos.x >= 0f && screenPos.x <= 1f && screenPos.y >= 0f && screenPos.y <= 1f && screenPos.z > 0f)
        {
            arrow.anchoredPosition = new Vector2(Mathf.Clamp(screenPos.x * 1080f - 540f, -310, 310), arrow.anchoredPosition.y);
            arrow.gameObject.SetActive(true);
        }
        else
        {
            arrow.gameObject.SetActive(false);
        }
    }
}