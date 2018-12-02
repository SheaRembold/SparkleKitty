using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreatController : ItemController
{
    [SerializeField]
    Transform piecesParent;
    [SerializeField]
    float eatRate = 0.1f;
    
    public void Eat()
    {
        float oldAmount = _amountLeft;
        _amountLeft -= eatRate * Time.deltaTime;
        _amountLeft = Mathf.Clamp01(_amountLeft);
        PlayerManager.Instance.AddItemHealth(placable.Data, _amountLeft - oldAmount);
        UpdateAmountLeft();
    }

    protected override void UpdateAmountLeft()
    {
        base.UpdateAmountLeft();

        for (int i = 0; i < piecesParent.childCount; i++)
        {
            if ((float)i / piecesParent.childCount < _amountLeft)
                piecesParent.GetChild(i).gameObject.SetActive(true);
            else
                piecesParent.GetChild(i).gameObject.SetActive(false);
        }

        if (_amountLeft <= 0)
        {
            Instantiate(destroyEffect, transform.position, transform.rotation, transform.parent);
            PlacementManager.Instance.Remove(placable);
        }
    }
}
