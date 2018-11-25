using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class ARPlacementProvider : PlacementProvider
{
    GameObject cameraObj;
    GameObject lightObj;
    ARSessionOrigin origin;

    public ARPlacementProvider()
    {
        sceneObj = GameObject.Instantiate(Resources.Load<GameObject>("ARScene"));

        cameraObj = sceneObj.GetComponentInChildren<Camera>().gameObject;
        lightObj = sceneObj.GetComponentInChildren<Light>().gameObject;
        origin = sceneObj.GetComponentInChildren<ARSessionOrigin>();

        holdAttachPoint = cameraObj.transform.Find("AttachPoint");
        viewAttachPoint = holdAttachPoint;
    }

    public override bool IsReady()
    {
        return ARSubsystemManager.systemState >= ARSystemState.Ready;
    }

    public override Transform GetRoot()
    {
        return origin.transform;
    }

    Vector2 Divide(Vector2 num, Vector2 den)
    {
        return new Vector2(num.x / den.x, num.y / den.y);
    }

    public override bool GetPlane(out BoundedPlane plane)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("ARGameObject")))
        {
            ARPlane arPlane = hit.transform.GetComponent<ARPlane>();
            if (arPlane != null && arPlane.boundedPlane.Alignment == PlaneAlignment.Horizontal)
            {
                LineRenderer lineRenderer = arPlane.GetComponent<LineRenderer>();

                Vector3 camPos = lineRenderer.transform.InverseTransformPoint(Camera.main.transform.position);
                Vector2 dir = new Vector2(camPos.x, camPos.z);
                if (camPos.magnitude == 0)
                    camPos = Vector2.down;
                else
                    camPos.Normalize();

                Vector3[] boundary = new Vector3[lineRenderer.positionCount];
                lineRenderer.GetPositions(boundary);
                Vector2[] linePos = new Vector2[boundary.Length - 1];
                Vector2[] lineDir = new Vector2[boundary.Length - 1];
                float[] lineMag = new float[boundary.Length - 1];
                for (int i = 0; i < linePos.Length; i++)
                {
                    linePos[i] = new Vector2(boundary[i].x, boundary[i].z);
                    lineDir[i] = new Vector2(boundary[i + 1].x, boundary[i + 1].z) - linePos[i];
                    lineMag[i] = lineDir[i].magnitude;
                    lineDir[i].Normalize();
                }
                
                float shortest = float.MaxValue;
                for (int d = 0; d < 4; d++)
                {
                    float ang = (45f + 90f * d) * Mathf.Deg2Rad;
                    Vector2 dirRot = new Vector2(dir.x * Mathf.Cos(ang) - dir.y * Mathf.Sin(ang), dir.x * Mathf.Sin(ang) + dir.y * Mathf.Cos(ang));
                    for (int i = 0; i < linePos.Length; i++)
                    {
                        Vector2 inter = Divide(-linePos[i], lineDir[i] - dirRot);
                        float interDist = Vector2.Distance(inter, linePos[i]);
                        if (interDist >= 0f && interDist <= lineMag[i])
                        {
                            float dist = inter.magnitude;
                            if (dist < shortest)
                            {
                                shortest = dist;
                            }
                            break;
                        }
                    }
                }

                float size = Mathf.Cos(45f * Mathf.Deg2Rad) * shortest;
                Vector3 forward = lineRenderer.transform.rotation * new Vector3(dir.x, 0f, dir.y);
                plane = new BoundedPlane() { Center = lineRenderer.transform.position, Pose = new Pose(lineRenderer.transform.position, Quaternion.LookRotation(forward, lineRenderer.transform.up)), Size = new Vector2(size, size) };
                return true;
            }
        }

        plane = new BoundedPlane();
        return false;
    }

    public override void FinishInit()
    {
        base.FinishInit();

        ARPlaneManager planeManager = sceneObj.GetComponentInChildren<ARPlaneManager>();
        List<ARPlane> planes = new List<ARPlane>();
        planeManager.GetAllPlanes(planes);
        for (int i = 0; i < planes.Count; i++)
            GameObject.Destroy(planes[i].gameObject);
        planeManager.enabled = false;
        ARPointCloudManager cloudManager = sceneObj.GetComponentInChildren<ARPointCloudManager>();
        if (cloudManager.pointCloud != null)
            GameObject.Destroy(cloudManager.pointCloud.gameObject);
        cloudManager.enabled = false;
        sceneObj.GetComponentInChildren<ARReferencePointManager>().enabled = false;
    }

    public override void TurnOff()
    {
        cameraObj.SetActive(false);
        lightObj.SetActive(false);
    }

    public override void TurnOn()
    {
        cameraObj.SetActive(true);
        lightObj.SetActive(true);
    }
}