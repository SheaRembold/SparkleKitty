using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AlbumUI : MonoBehaviour
{
    [SerializeField]
    GameObject ButtonPrefab;
    [SerializeField]
    Transform Content;
    [SerializeField]
    PhotoUI photoUI;

    private void OnEnable()
    {
        for (int i = 0; i < Content.childCount; i++)
        {
            Destroy(Content.GetChild(i).gameObject);
        }

        for (int i = 0; i < DataManager.Instance.Cats.Length; i++)
        {
            PlacableData catData = DataManager.Instance.Cats[i];
            GameObject obj = Instantiate(ButtonPrefab);
            obj.transform.SetParent(Content, false);
            (obj.transform as RectTransform).anchoredPosition = new Vector2(-220 + (i % 2) * 440, 500 - (i / 2) * 440);
            Image image = obj.transform.Find("Image").GetComponent<Image>();
            image.sprite = catData.Icon;
            Button button = obj.GetComponent<Button>();
            if (File.Exists(Application.persistentDataPath + "/" + catData.name + ".png"))
            {
                button.onClick.AddListener(new UnityEngine.Events.UnityAction(() => { ShowPhoto(catData); }));
                image.color = Color.white;
            }
            else
            {
                button.interactable = false;
                image.color = Color.black;
            }
        }
    }

    void ShowPhoto(PlacableData data)
    {
        photoUI.ShowPhoto(data);
        //UIManager.Instance.ShowUI(photoUI.gameObject);
    }
}