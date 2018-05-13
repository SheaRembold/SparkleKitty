using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public RectTransform ContentParent;
    public GameObject PlacableButtonPrefab;
    public PlacableDataType[] PlacableTypes;
    List<PlacableData> placables = new List<PlacableData>();

    private void Start()
    {
        PlayerManager.Instance.onInventoryChange += UpdateInventory;
        UpdateInventory();
    }

    void UpdateInventory()
    {
        for (int i = 0; i < ContentParent.childCount; i++)
        {
            Destroy(ContentParent.GetChild(i).gameObject);
        }
        placables.Clear();
        for (int i = 0; i < PlacableTypes.Length; i++)
        {
            placables.AddRange(PlayerManager.Instance.GetInventoryItems(PlacableTypes[i]));
        }
        for (int i = 0; i < placables.Count; i++)
        {
            GameObject obj = Instantiate(PlacableButtonPrefab);
            obj.transform.SetParent(ContentParent, false);
            (obj.transform as RectTransform).anchoredPosition = new Vector2(200 * i + 100, 0);
            obj.GetComponent<PlacableButton>().SetPlacable(placables[i]);
        }
        ContentParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200 * placables.Count);
        ContentParent.anchoredPosition = Vector2.zero;
    }
}