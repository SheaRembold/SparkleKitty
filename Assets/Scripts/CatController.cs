using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    public class CatState
    {
        protected CatController controller;
        protected float stateTime;
        
        public virtual void Init(CatController controller)
        {
            this.controller = controller;
        }

        public virtual void OnEnter()
        {
            stateTime = 0f;
        }

        public virtual void OnUpdate()
        {
            stateTime += Time.deltaTime;
        }

        public virtual void OnExit()
        {
        }
    }

    public class SitState : CatState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            controller.animator.SetTrigger("Sit");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (stateTime > controller.SitTime)
            {
                controller.target = PlayerManager.Instance.GetRandomInArea<BuildableData>();
                if (controller.target != null)
                    controller.targetPos = controller.target.transform.localPosition;
                else
                    controller.targetPos = PlacementManager.Instance.GetRandomInArea();
                controller.SetState<WalkState>();
            }
        }
    }

    public class WalkState : CatState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            controller.animator.SetTrigger("Walk");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (controller.target != null)
                controller.targetPos = controller.target.transform.localPosition;
            Vector3 targetWorldPos = PlacementManager.Instance.GetWorldPos(controller.targetPos);
            controller.transform.LookAt(targetWorldPos, controller.transform.up);
            float distToTarget = Vector3.Distance(controller.transform.position, targetWorldPos);
            float worldStopDist = controller.StopDistance * controller.transform.lossyScale.x;
            if (Mathf.Abs(distToTarget - worldStopDist) < 0.01f)
            {
                if (controller.target != null)
                    controller.SetState<InteractState>();
                else
                    controller.SetState<SitState>();
            }
            else
            {
                float movement = controller.WalkSpeed * controller.transform.lossyScale.x * Time.deltaTime;
                if (movement < distToTarget - worldStopDist)
                    controller.transform.position += controller.transform.forward * movement;
                else
                    controller.transform.position += controller.transform.forward * (distToTarget - worldStopDist);
            }
        }
    }

    public class InteractState : CatState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            controller.animator.SetTrigger("Interact");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (stateTime > controller.InteractTime)
            {
                controller.SetState<SitState>();
            }
        }
    }

    public float WalkSpeed = 1f;
    public float SitTime = 5f;
    public float StopDistance = 1f;
    public float InteractTime = 5f;

    protected CatData data;
    protected Animator animator;
    protected Placable target;
    protected Vector3 targetPos;
    List<CatState> states = new List<CatState>();
    CatState currentState;

    protected void AddState<T>() where T : CatState, new()
    {
        T newState = new T();
        states.Add(newState);
        newState.Init(this);
    }

    protected void SetState<T>() where T : CatState
    {
        if (currentState != null)
            currentState.OnExit();
        currentState = states.Find((x) => { return x is T; });
        if (currentState != null)
            currentState.OnEnter();
    }

    protected virtual void Awake()
    {
        data = GetComponent<Placable>().Data as CatData;
        animator = GetComponent<Animator>();

        AddState<SitState>();
        AddState<WalkState>();
        AddState<InteractState>();

        SetState<SitState>();
    }

    protected virtual void Update()
    {
        if (currentState != null)
            currentState.OnUpdate();
    }
}