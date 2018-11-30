using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField]
    protected GameObject destroyEffect;

    public float AmountLeft { get { return _amountLeft; } }

    protected Placable placable;
    protected float _amountLeft = 1f;

    protected virtual void Awake()
    {
        placable = GetComponent<Placable>();
    }

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
