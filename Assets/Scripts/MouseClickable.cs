using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickable : Clickable, IChasable
{
    [SerializeField]
    float speed = 1f;

    Rigidbody rigidbody;
    Animator animator;

    private void Awake()
    {
        rigidbody = GetComponentInChildren<Rigidbody>();
        rigidbody.isKinematic = true;
        animator = GetComponentInChildren<Animator>();
    }

    public override void Click(RaycastHit hit)
    {
        Vector3 dir = PlacementManager.Instance.GetPlayArea().transform.position - rigidbody.position;
        dir.y = 0f;
        if (dir.magnitude == 0)
            dir = new Vector3(Random.value, 0f, Random.value);
        rigidbody.velocity = dir.normalized * speed;
    }

    void Update()
    {
        if (rigidbody.velocity.magnitude > 0.001f)
            animator.SetInteger("State", 2);
        else
            animator.SetInteger("State", 0);
    }
    
    public override void AddedToArea()
    {
        rigidbody.isKinematic = false;
        PlacementManager.Instance.chasables.Add(this);
    }

    public override void RemovedFromArea()
    {
        PlacementManager.Instance.chasables.Remove(this);
    }

    public Vector3 ChasePosition { get { return rigidbody.position; } }
    public float Attraction(CatController cat) { return rigidbody.velocity.magnitude / (speed / 2f); }
}