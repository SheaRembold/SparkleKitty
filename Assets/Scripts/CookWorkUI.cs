using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookWorkUI : BuildWorkUI
{
    [SerializeField]
    protected float radius;

    protected override void HandleInput()
    {
        float currentDist = Vector2.Distance(currentPos, line.position);
        float lastDist = Vector2.Distance(lastPos, line.position);
        if (currentDist <= radius + tolerance && currentDist >= radius - tolerance
            && lastDist <= radius + tolerance && lastDist >= radius - tolerance)
        {
            amount += Vector2.Angle((currentPos - (Vector2)line.position).normalized, (lastPos - (Vector2)line.position).normalized);
        }
    }
}