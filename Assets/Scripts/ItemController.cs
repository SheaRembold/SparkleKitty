using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    protected float _amountLeft = 1f;
    public float AmountLeft { get { return _amountLeft; } }

    public void SetAmountLeft(float amount)
    {
        _amountLeft = amount;
        UpdateAmountLeft();
    }
    
    protected virtual void UpdateAmountLeft()
    {
    }

    public bool AnyLeft()
    {
        return _amountLeft > 0f;
    }
}
