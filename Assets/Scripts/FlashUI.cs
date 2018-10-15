using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashUI : MonoBehaviour
{
    [SerializeField]
    Image Scaling;
    [SerializeField]
    Image ScalingInner;
    [SerializeField]
    float FlashSpeed = 1f;
    [SerializeField]
    Vector2 sizeMinOffset = new Vector2(20, 20);
    [SerializeField]
    Vector2 sizeMaxOffset = new Vector2(20, 20);

    Graphic target;

    public void SetTarget(Image target)
    {
        this.target = target;
        transform.SetParent(target.transform.parent, false);
        transform.SetSiblingIndex(0);
        transform.localPosition = target.transform.localPosition;
        Scaling.rectTransform.sizeDelta = target.rectTransform.sizeDelta + sizeMinOffset;
        Scaling.sprite = target.sprite;
        ScalingInner.sprite = target.sprite;
        Scaling.type = target.type;
        ScalingInner.type = target.type;
        Scaling.preserveAspect = target.preserveAspect;
        ScalingInner.preserveAspect = target.preserveAspect;
    }
    
    private void Update()
    {
        if (target != null)
            Scaling.rectTransform.sizeDelta = Vector2.Lerp(target.rectTransform.sizeDelta + sizeMinOffset, target.rectTransform.sizeDelta + sizeMaxOffset, (Mathf.Sin(Time.time * FlashSpeed) + 1f) * 0.5f);
        else
            Scaling.rectTransform.sizeDelta = Vector2.zero;
    }
}