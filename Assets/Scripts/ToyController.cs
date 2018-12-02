using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyController : ItemController
{
    [SerializeField]
    float damageRate = 0.1f;
    
    public void Play()
    {
        float oldAmount = _amountLeft;
        _amountLeft -= damageRate * Time.deltaTime;
        _amountLeft = Mathf.Clamp01(_amountLeft);
        PlayerManager.Instance.AddItemHealth(placable.Data, _amountLeft - oldAmount);
        UpdateAmountLeft();
    }

    protected override void UpdateAmountLeft()
    {
        base.UpdateAmountLeft();

        if (_amountLeft <= 0)
        {
            Instantiate(destroyEffect, transform.position, transform.rotation, transform.parent);
            if (placable.Data.Attached)
                PlacementManager.Instance.RemoveAttached();
            else
                PlacementManager.Instance.Remove(placable);
        }
    }
}
