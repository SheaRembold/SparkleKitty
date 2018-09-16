using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    Transform laser;
    [SerializeField]
    float defaultDistance = 10f;

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Placable")) | (1 << LayerMask.NameToLayer("UI"))))
        {
            laser.transform.position = (transform.position + hit.point) / 2f;
            laser.transform.localScale = new Vector3(laser.transform.localScale.x, hit.distance / 2f / laser.transform.parent.lossyScale.y, laser.transform.localScale.z);
        }
        else
        {
            laser.transform.position = transform.position + transform.forward * defaultDistance / 2f;
            laser.transform.localScale = new Vector3(laser.transform.localScale.x, defaultDistance / 2f / laser.transform.parent.lossyScale.y, laser.transform.localScale.z);
        }
    }
}