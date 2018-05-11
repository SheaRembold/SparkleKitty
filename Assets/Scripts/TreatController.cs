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
        _amountLeft -= eatRate * Time.deltaTime;
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
    }
}
