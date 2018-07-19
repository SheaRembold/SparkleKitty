using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ListPageUI : MonoBehaviour
{
    [SerializeField]
    GameObject ButtonPrefab;
    [SerializeField]
    Transform Content;
    [SerializeField]
    ItemPageUI ItemPage;
    [SerializeField]
    PlacableDataType PlacableDataType;
    
    [SerializeField]
    UIManager uiManager;

    private void OnEnable()
    {
        for (int i = 0; i < Content.childCount; i++)
        {
            Destroy(Content.GetChild(i).gameObject);
        }

        PlacableData[] allData = DataManager.Instance.GetAllData(PlacableDataType);
        for (int i = 0; i < allData.Length; i++)
        {
            PlacableData itemData = allData[i];
            GameObject obj = Instantiate(ButtonPrefab);
            obj.transform.SetParent(Content, false);
            (obj.transform as RectTransform).anchoredPosition = new Vector2(120 + (i % 4) * 200, -120 - (i / 4) * 200);
            PlacableButton image = obj.GetComponent<PlacableButton>();
            image.SetPlacable(itemData);
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(new UnityEngine.Events.UnityAction(() => { ShowItem(itemData); }));
        }
    }

    void ShowItem(PlacableData item)
    {
        ItemPage.SetItem(item);
        uiManager.ShowUI(ItemPage.gameObject);
    }

    public void PrevPage()
    {

    }

    public void NextPage()
    {

    }
}