using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroUI : MonoBehaviour
{
    [SerializeField]
    GameObject introScene;
    [SerializeField]
    GameObject sparkleKitty;

    private void OnEnable()
    {
        PlacementManager.Instance.SetArea(AreaType.None);
        introScene.SetActive(true);
        if (!PlayerManager.Instance.HasShownHelp("Intro"))
            UIManager.Instance.ShowSpeechUI(sparkleKitty.transform, "Intro", true);
    }

    private void OnDisable()
    {
        if (introScene != null)
        {
            introScene.SetActive(false);
        }
    }
}
