using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashUI : MonoBehaviour
{
    [SerializeField]
    Image Scaling;
    [SerializeField]
    float FlashSpeed = 1f;
    [SerializeField]
    Vector2 sizeOffset = new Vector2(20, 20);

    Graphic target;

    public void SetTarget(Graphic target)
    {
        this.target = target;
        transform.SetParent(target.transform.parent, false);
        transform.SetSiblingIndex(target.transform.GetSiblingIndex());
        transform.localPosition = target.transform.localPosition;
        Scaling.rectTransform.sizeDelta = target.rectTransform.sizeDelta;
    }
    
    private void Update()
    {
        if (target != null)
            Scaling.rectTransform.sizeDelta = Vector2.Lerp(target.rectTransform.sizeDelta, target.rectTransform.sizeDelta + sizeOffset, (Mathf.Sin(Time.time * FlashSpeed) + 3f) * 0.25f);
        else
            Scaling.rectTransform.sizeDelta = Vector2.zero;
    }
}