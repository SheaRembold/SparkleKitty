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
            obj.transform.SetParent(Content);
            (obj.transform as RectTransform).anchoredPosition = new Vector2(0, 675 - 125 * i);
            Text text = obj.GetComponentInChildren<Text>();
            text.text = catData.Name;
            Button button = obj.GetComponent<Button>();
            if (File.Exists(Application.persistentDataPath + "/" + catData.name + ".png"))
                button.onClick.AddListener(new UnityEngine.Events.UnityAction(() => { ShowPhoto(catData); }));
            else
                button.interactable = false;
        }
    }

    void ShowPhoto(PlacableData data)
    {
        photoUI.ShowPhoto(data);
        UIManager.Instance.ShowUI(photoUI.gameObject);
    }
}