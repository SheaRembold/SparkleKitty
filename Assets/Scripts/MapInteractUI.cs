using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapInteractUI : MonoBehaviour
{
    [SerializeField]
    MapInteractStar[] stars;
    [SerializeField]
    Image outlineImage;
    [SerializeField]
    Image fillImage;
    [SerializeField]
    Image itemImage;
    [SerializeField]
    PlacableData[] resources; 
    
    int starsAdded;
    float percentComplete;
    float percentShown;
    bool awarded;

    public void Show()
    {
        for (int i = 0; i < stars.Length; i++)
            stars[i].Init(this);
        starsAdded = 0;
        percentComplete = 0f;
        percentShown = 0f;
        fillImage.fillAmount = 0f;
        outlineImage.gameObject.SetActive(true);
        fillImage.gameObject.SetActive(true);
        itemImage.gameObject.SetActive(false);
        awarded = false;
        gameObject.SetActive(true);
    }

    public void AddStar()
    {
        starsAdded++;
        percentComplete = (float)starsAdded / stars.Length;
        PlacesManager.Instance.CompleteStep();
    }

    private void Update()
    {
        if (!awarded)
        {
            percentShown = Mathf.MoveTowards(percentShown, percentComplete, Time.deltaTime * 0.5f);
            fillImage.fillAmount = percentShown;
            if (percentShown >= 1f)
            {
                outlineImage.gameObject.SetActive(false);
                fillImage.gameObject.SetActive(false);
                PlacableData resource = resources[Random.Range(0, resources.Length)];
                itemImage.sprite = resource.Icon;
                itemImage.gameObject.SetActive(true);
                PlayerManager.Instance.AddInventory(resource);
                PlacesManager.Instance.CompleteInteraction();
                awarded = true;
            }
        }
    }

    public void CollectItem()
    {
        PlacesManager.Instance.HideInteraction();
        gameObject.SetActive(false);
    }
}