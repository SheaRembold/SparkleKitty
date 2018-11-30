using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringToy : MonoBehaviour, IChasable
{
    [SerializeField]
    Transform rope;
    [SerializeField]
    Transform target;

    private void Start()
    {
        rope.SetParent(null);
        /*CharacterJoint[] joints = rope.GetComponentsInChildren<CharacterJoint>();
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].autoConfigureConnectedAnchor = false;
            joints[i].connectedAnchor = Vector3.Scale(joints[i].connectedAnchor, rope.transform.localScale);
        }*/
        PlacementManager.Instance.chasables.Add(this);
    }
    
    private void OnDestroy()
    {
        if (rope != null)
            Destroy(rope.gameObject);
        if (PlacementManager.Instance != null)
            PlacementManager.Instance.chasables.Remove(this);
    }

    public Vector3 ChasePosition { get { return target.position; } }
    public ToyController Controller { get { return GetComponent<ToyController>(); } }
    public float Attraction(CatController cat) { return (1.1f - Vector3.Distance(target.position, cat.transform.position) / cat.transform.lossyScale.y); }
}