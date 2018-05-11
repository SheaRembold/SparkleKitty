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
                if (Random.value < controller.InteractProbability)
                {
                    List<Placable> favorites = controller.playArea.GetInArea(controller.data.OtherRequirements);
                    for (int i = 0; i < favorites.Count; i++)
                    {
                        if (favorites[i].GetComponent<ItemController>() != null && !favorites[i].GetComponent<ItemController>().AnyLeft())
                        {
                            favorites.RemoveAt(i);
                            i--;
                        }
                    }
                    if (favorites.Count > 0)
                    {
                        controller.target = favorites[Random.Range(0, favorites.Count)];
                        controller.targetPos = controller.target.transform.localPosition;
                    }
                    else
                    {
                        controller.target = null;
                        controller.isLeaving = !controller.StayForever;
                        if (controller.isLeaving)
                            controller.targetPos = controller.playArea.CatSpawnPoint.localPosition;
                        else
                            controller.targetPos = PlacementManager.Instance.GetRandomInArea();
                    }
                }
                else
                {
                    controller.target = null;
                    controller.isLeaving = false;
                    controller.targetPos = PlacementManager.Instance.GetRandomInArea();
                }
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
                {
                    if (controller.target.Data.DataType == PlacableDataType.Treat)
                        controller.SetState<EatState>();
                    else
                        controller.SetState<PlayState>();
                }
                else if (controller.isLeaving)
                {
                    controller.playArea.RemoveFromArea(controller.GetComponent<Placable>());
                    Destroy(controller.gameObject);
                }
                else
                {
                    controller.SetState<SitState>();
                }
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

    public class EatState : CatState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            controller.animator.SetTrigger("Eat");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            TreatController treatController = controller.target == null ? null : controller.target.GetComponent<TreatController>();
            if (treatController == null || !treatController.AnyLeft() || stateTime > controller.InteractTime)
            {
                PlacementManager.Instance.GetPlayArea().MarkAsDirty();
                controller.SetState<SitState>();
            }
            else
            {
                treatController.Eat();
            }
        }
    }

    public class PlayState : CatState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            controller.animator.SetTrigger("Play");
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
    public float InteractProbability = 1f;
    public bool StayForever;

    protected CatData data;
    protected Animator animator;
    protected PlayArea playArea;
    protected Placable target;
    protected Vector3 targetPos;
    protected bool isLeaving;
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
        animator = GetComponent<Animator>();

        AddState<SitState>();
        AddState<WalkState>();
        AddState<EatState>();
        AddState<PlayState>();

        SetState<SitState>();
    }

    protected virtual void Start()
    {
        playArea = GetComponentInParent<PlayArea>();
        data = GetComponent<Placable>().Data as CatData;
    }

    protected virtual void Update()
    {
        if (currentState != null)
            currentState.OnUpdate();
    }
}