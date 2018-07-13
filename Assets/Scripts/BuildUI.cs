using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildUI : MonoBehaviour
{
    [SerializeField]
    RecipeUI recipeUI;
    [SerializeField]
    PlacableDataType recipeType;

    public virtual void Show(PlacementLocation placementLocation)
    {
        PlacementManager.Instance.SetArea(AreaType.Build);
        PlacementManager.Instance.GetBuildArea().SetPlacementLocation(placementLocation);
        PlacementManager.Instance.GetBuildArea().SetUpgrading(null);
        //UIManager.Instance.ShowUI(gameObject);
    }

    public virtual void ShowUpgrade(PlacementLocation placementLocation, UpgradableData upgradable)
    {
        PlacementManager.Instance.SetArea(AreaType.Build);
        PlacementManager.Instance.GetBuildArea().SetPlacementLocation(placementLocation);
        PlacementManager.Instance.GetBuildArea().SetUpgrading(upgradable);
        //UIManager.Instance.ShowUI(gameObject);
    }

    public virtual void Hide()
    {
        PlacementManager.Instance.SetArea(AreaType.Play);
        //UIManager.Instance.GoBack();
    }

    public void ShowRecipes()
    {
        recipeUI.SetRecipeType(recipeType);
        //UIManager.Instance.ShowUI(recipeUI.gameObject);
    }
}