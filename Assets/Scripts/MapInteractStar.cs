using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapInteractStar : MonoBehaviour, IDragHandler, IEndDragHandler
{
    MapInteractUI _interactUI;
    Vector3 startPos;
    bool isOnTarget;
    bool _initialized;

    public void Init(MapInteractUI interactUI)
    {
        if(!_initialized)
        {
            startPos = transform.localPosition;
            _initialized = true;
        }
        _interactUI = interactUI;
        transform.localPosition = startPos;
        isOnTarget = false;
        gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        isOnTarget = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        isOnTarget = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isOnTarget)
        {
            _interactUI.AddStar();
            gameObject.SetActive(false);
        }
        else
        {
            transform.localPosition = startPos;
        }
    }
}