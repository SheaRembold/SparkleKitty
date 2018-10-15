using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : Clickable
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
            if (Random.value < controller.MeowProbability)
                controller.MeowMaker();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            for (int i = 0; i < PlacementManager.Instance.chasables.Count; i++)
            {
                if (PlacementManager.Instance.chasables[i].Attraction >= 1f
                    && (controller.chaseTarget == null || PlacementManager.Instance.chasables[i].Attraction > controller.chaseTarget.Attraction))
                {
                    controller.chaseTarget = PlacementManager.Instance.chasables[i];
                }
            }
            if (controller.chaseTarget != null)
            {
                controller.SetState<ChaseState>();
                return;
            }

            if (stateTime > controller.SitTime)
            {
                if (Random.value < controller.InteractProbability)
                {
                    List<Placable> favorites = controller.playArea.GetInArea(controller.catData.OtherRequirements);
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
            
            for (int i = 0; i < PlacementManager.Instance.chasables.Count; i++)
            {
                if (PlacementManager.Instance.chasables[i].Attraction >= 1f
                    && (controller.chaseTarget == null || PlacementManager.Instance.chasables[i].Attraction > controller.chaseTarget.Attraction))
                {
                    controller.chaseTarget = PlacementManager.Instance.chasables[i];
                }
            }
            if (controller.chaseTarget != null)
            {
                controller.SetState<ChaseState>();
                return;
            }

            if (controller.target != null)
                controller.targetPos = controller.target.transform.localPosition;
            Vector3 targetWorldPos = PlacementManager.Instance.GetWorldPos(controller.targetPos);
            controller.transform.LookAt(targetWorldPos, PlacementManager.Instance.GetPlayArea().transform.up);
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

    public class TutorialWalkState : CatState
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
            controller.transform.LookAt(targetWorldPos, PlacementManager.Instance.GetPlayArea().transform.up);
            float distToTarget = Vector3.Distance(controller.transform.position, targetWorldPos);
            float worldStopDist = controller.StopDistance * controller.transform.lossyScale.x;
            if (Mathf.Abs(distToTarget - worldStopDist) < 0.01f)
            {
                controller.SetState<TutorialSitState>();
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

    public class TutorialSitState : CatState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            controller.animator.SetTrigger("Sit");
            if (Random.value < controller.MeowProbability)
                controller.MeowMaker();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
        }
    }

    public class PetState : CatState
    {
        bool hasBeenPet;

        public override void OnEnter()
        {
            base.OnEnter();
            controller.animator.SetTrigger("Pet");
            hasBeenPet = false;
            //added audio
            //controller.StartEatingAudio();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (controller.animator.GetCurrentAnimatorStateInfo(0).IsName("Pet"))
            {
                hasBeenPet = true;
            }
            else if (hasBeenPet)
            {
                controller.AddMood(1f / (CatManager.Instance.MoodColors.Length - 1));
                //if (controller.HelpParticles != null)
                    //controller.HelpParticles.SetActive(false);
                if (controller.HelpOutline != null)
                    controller.HelpOutline.enabled = false;
                if (HelpManager.Instance.CurrentStep == TutorialStep.Start)
                {
                    HelpManager.Instance.CompleteTutorialStep(TutorialStep.Start);
                }
                else
                {
                    controller.SetState<SitState>();
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            //controller.StopEatingAudio();
        }
    }

    public class EatState : CatState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            controller.animator.SetTrigger("Eat");
            //added audio
            controller.StartEatingAudio();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            TreatController treatController = controller.target == null ? null : controller.target.GetComponent<TreatController>();
            if (treatController == null || !treatController.AnyLeft() || stateTime > controller.InteractTime)
            {
                if (stateTime > controller.InteractTime)
                    controller.AddMood(1f / (CatManager.Instance.MoodColors.Length - 1));
                PlacementManager.Instance.GetPlayArea().MarkAsDirty();
                controller.SetState<SitState>();
            }
            else
            {
                treatController.Eat();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            controller.StopEatingAudio();
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
                controller.AddMood(1f / (CatManager.Instance.MoodColors.Length - 1));
                controller.SetState<SitState>();
            }
        }
    }

    public class ChaseState : CatState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            controller.animator.SetTrigger("Run");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (controller.chaseTarget == null || controller.chaseTarget.ChasePosition == Vector3.zero)
            {
                controller.SetState<SitState>();
            }
            else
            {
                Vector3 targetWorldPos = controller.chaseTarget.ChasePosition;
                controller.transform.LookAt(targetWorldPos, PlacementManager.Instance.GetPlayArea().transform.up);
                float distToTarget = Vector3.Distance(controller.transform.position, targetWorldPos);
                float worldStopDist = controller.StopDistance * controller.transform.lossyScale.x;
                if (Mathf.Abs(distToTarget - worldStopDist) < 0.01f)
                {
                    controller.SetState<ChasePlayState>();
                }
                else
                {
                    float movement = controller.RunSpeed * controller.transform.lossyScale.x * Time.deltaTime;
                    if (movement < distToTarget - worldStopDist)
                        controller.transform.position += controller.transform.forward * movement;
                    else
                        controller.transform.position += controller.transform.forward * (distToTarget - worldStopDist);
                }
            }
        }
    }

    public class ChasePlayState : CatState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            controller.animator.SetTrigger("Play");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (controller.chaseTarget == null || controller.chaseTarget.ChasePosition == Vector3.zero || stateTime > controller.InteractTime)
            {
                if (stateTime > controller.InteractTime)
                    controller.AddMood(1f / (CatManager.Instance.MoodColors.Length - 1));
                controller.chaseTarget = null;
                controller.SetState<SitState>();
            }
            else
            {
                Vector3 targetWorldPos = controller.chaseTarget.ChasePosition;
                controller.transform.LookAt(targetWorldPos, PlacementManager.Instance.GetPlayArea().transform.up);
                float distToTarget = Vector3.Distance(controller.transform.position, targetWorldPos);
                float worldStopDist = controller.StopDistance * controller.transform.lossyScale.x;
                if (distToTarget > 2f * worldStopDist)
                {
                    controller.SetState<ChaseState>();
                }
            }
        }
    }

    public float WalkSpeed = 1f;
    public float RunSpeed = 2f;
    public float SitTime = 5f;
    public float StopDistance = 1f;
    public float InteractTime = 5f;
    public float InteractProbability = 1f;
    public float MeowProbability = 1f;
    public bool StayForever;
    public Renderer[] MoodRenderers;
    public GameObject HelpParticles;
    public Outline HelpOutline;

    protected CatData catData;
    protected Animator animator;
    protected PlayArea playArea;
    protected Placable target;
    protected Vector3 targetPos;
    protected IChasable chaseTarget;
    protected bool isLeaving;
    List<CatState> states = new List<CatState>();
    CatState currentState;
    Material moodMat;
    AudioSource ASource;
    float moodValue = 0.5f;
    
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
        ASource = GetComponent<AudioSource>();

        moodMat = new Material(MoodRenderers[0].sharedMaterial);
        for (int i=0;i<MoodRenderers.Length;i++)
        {
            MoodRenderers[i].material = moodMat;
        }
    }
    
    protected virtual void Start()
    {
        playArea = GetComponentInParent<PlayArea>();
        catData = Data as CatData;

        AddState<SitState>();
        AddState<WalkState>();
        AddState<TutorialSitState>();
        AddState<TutorialWalkState>();
        AddState<EatState>();
        AddState<PetState>();
        AddState<PlayState>();
        AddState<ChaseState>();
        AddState<ChasePlayState>();
        
        CatManager.Instance.MarkFound(catData);
        moodValue = CatManager.Instance.GetMoodValue(catData);
        moodMat.color = CatManager.Instance.MoodColors[CatManager.Instance.GetMood(catData)];

        if (HelpManager.Instance.CurrentStep == TutorialStep.Start)
        {
            //if (HelpParticles != null)
                //HelpParticles.SetActive(true);
            if (HelpOutline != null)
                HelpOutline.enabled = true;
        }
        if (HelpManager.Instance.CurrentStep == TutorialStep.Mail)
        {
            targetPos = MailboxManager.Instance.transform.localPosition;
            SetState<TutorialWalkState>();
        }
        else if (HelpManager.Instance.CurrentStep == TutorialStep.GrabBook)
        {
            targetPos = BookController.Instance.transform.localPosition;
            SetState<TutorialWalkState>();
        }
        else
        {
            SetState<SitState>();
        }

        if (StayForever)
            HelpManager.Instance.onCompleteTutorialStep += OnCompleteTutorialStep;
    }

    void OnCompleteTutorialStep(TutorialStep currentStep)
    {
        if (currentStep == TutorialStep.Mail)
        {
            targetPos = MailboxManager.Instance.transform.localPosition;
            SetState<TutorialWalkState>();
        }
        else if (currentStep == TutorialStep.GrabBook)
        {
            targetPos = BookController.Instance.transform.localPosition;
            SetState<TutorialWalkState>();
        }
        else if (currentStep == TutorialStep.PlaceTreat)
        {
            SetState<SitState>();
        }
        else if (currentStep == TutorialStep.CraftToy)
        {
            target = playArea.GetInArea(PlacableDataType.Treat)[0];
            SetState<WalkState>();
        }
    }

    protected virtual void Update()
    {
        if (currentState != null)
            currentState.OnUpdate();
    }

    protected void MeowMaker()
    {
        //int i = Random.Range(0, data.CatSounds.Count);
        //ASource.PlayOneShot(data.CatSounds[i]);
    }

    protected void StartEatingAudio()
    {
        ASource.loop = true;
        ASource.clip = catData.EatingSound;
        ASource.Play();
    }

    protected void StopEatingAudio()
    {
        ASource.Play();
    }

    protected void AddMood(float amount)
    {
        moodValue = Mathf.Clamp01(moodValue + amount);
        CatManager.Instance.UpdateMood(catData, moodValue);
        moodMat.color = CatManager.Instance.MoodColors[CatManager.Instance.GetMood(catData)];
    }
    
    public override void Click(RaycastHit hit)
    {
        if (currentState is SitState || currentState is WalkState)
        {
            SetState<PetState>();
        }
    }
}