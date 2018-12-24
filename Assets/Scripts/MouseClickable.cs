using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MouseClickable : Clickable, IChasable
{
    [SerializeField]
    float chargeRate = 2f;
    [SerializeField]
    float useRate = 1f;

    Animator animator;
    NavMeshAgent navAgent;
    bool charging;
    float charge;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        navAgent = GetComponentInChildren<NavMeshAgent>();
    }

    public override void ClickDown(RaycastHit hit)
    {
        animator.SetInteger("State", 1);
        navAgent.isStopped = true;
        navAgent.updatePosition = false;
        navAgent.updateRotation = false;
        charging = true;
    }

    public override void ClickUp(RaycastHit hit)
    {
        charging = false;
        if (charge > 0)
        {
            animator.SetInteger("State", 2);
            navAgent.isStopped = false;
            navAgent.updatePosition = true;
            navAgent.updateRotation = true;
            navAgent.SetDestination(PlacementManager.Instance.GetWorldNavPos(PlacementManager.Instance.GetRandomInArea()));
        }
        else
        {
            animator.SetInteger("State", 0);
        }
    }

    void Update()
    {
        if (charging)
        {
            charge += chargeRate * Time.deltaTime;
        }
        else
        {
            if (charge > 0)
            {
                charge -= useRate * Time.deltaTime;
                if (charge <= 0)
                {
                    charge = 0;
                    animator.SetInteger("State", 0);
                    navAgent.isStopped = true;
                    navAgent.updatePosition = false;
                    navAgent.updateRotation = false;
                }
                else if (Vector3.Distance(transform.position, navAgent.destination) < 0.01f * transform.lossyScale.x)
                {
                    navAgent.SetDestination(PlacementManager.Instance.GetWorldNavPos(PlacementManager.Instance.GetRandomInArea()));
                }
            }
        }
    }
    
    public override void AddedToArea()
    {
        navAgent.enabled = true;
        PlacementManager.Instance.chasables.Add(this);
    }

    public override void RemovedFromArea()
    {
        PlacementManager.Instance.chasables.Remove(this);
    }

    public Vector3 ChasePosition { get { return transform.position; } }
    public ToyController Controller { get { return GetComponent<ToyController>(); } }
    public float Attraction(CatController cat) { return navAgent.velocity.magnitude / (navAgent.speed); }
}