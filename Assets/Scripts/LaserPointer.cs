using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    [SerializeField]
    Vector2 angleMin, angleMax;
    [SerializeField]
    float radius;
    [SerializeField]
    Transform laser;
    [SerializeField]
    float defaultDistance;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 thisPos = Camera.main.WorldToViewportPoint(transform.position);
            Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            Vector3 diff = mousePos - thisPos;
            diff.y = diff.y / Screen.width * Screen.height;
            transform.localRotation = Quaternion.Euler(Mathf.Lerp(angleMin.x, angleMax.x, diff.y / radius + 0.5f), 0f, Mathf.Lerp(angleMin.y, angleMax.y, diff.x / radius + 0.5f));
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.up, out hit))
        {
            laser.transform.position = (transform.position + hit.point) / 2f;
            laser.transform.localScale = new Vector3(laser.transform.localScale.x, hit.distance / 2f, laser.transform.localScale.z);
        }
        else
        {
            laser.transform.position = transform.position + transform.up * defaultDistance / 2f;
            laser.transform.localScale = new Vector3(laser.transform.localScale.x, defaultDistance / 2f, laser.transform.localScale.z);
        }
    }
}