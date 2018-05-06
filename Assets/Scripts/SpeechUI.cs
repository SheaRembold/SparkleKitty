using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechUI : MonoBehaviour
{
    [SerializeField]
    RectTransform bubble;
    [SerializeField]
    HelpTextData helpTextData;
    [SerializeField]
    Text text;
    Transform target;
    string speechName;
    string[] lines;
    int current;

    private void Awake()
    {
        helpTextData.Init();
    }

    public void ShowSpeech(Transform target, string speech)
    {
        gameObject.SetActive(true);
        this.target = target;
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
            UIManager.Instance.HideSpeechUI();
            gameObject.SetActive(false);
            PlayerManager.Instance.ShowHelp(speechName);
        }
    }

    private void LateUpdate()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(target.position);
        if (screenPos.x >= 0f && screenPos.x <= 1f && screenPos.y >= 0f && screenPos.y <= 1f && screenPos.z > 0f)
        {
            screenPos.y *= 1080f / Screen.width * Screen.height;
            bubble.anchoredPosition = new Vector2(0f, screenPos.y);
            bubble.gameObject.SetActive(true);
        }
        else
        {
            bubble.gameObject.SetActive(false);
        }
    }
}