using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

        public virtual void HitToy()
        {
        }
    }

    public class SitState : CatState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            controller.animator.SetTrigger("Sit");
            if (Random.value < controller.MeowProbability[controller.mood])
                controller.Meow();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            for (int i = 0; i < PlacementManager.Instance.chasables.Count; i++)
            {
                if (PlacementManager.Instance.chasables[i].Attraction(controller) >= 1f
                    && (controller.chaseTarget == null || PlacementManager.Instance.chasables[i].Attraction(controller) > controller.chaseTarget.Attraction(controller)))
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
                controller.AddMood(controller.MoodDec);
                if (controller.StayForever || CatManager.Instance.TimeSinceEnter(controller.catData) < controller.StayLength[controller.mood])
                {
                    controller.isLeaving = false;

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
                        int pick = Random.Range(0, favorites.Count);
                        if (controller.justEntered || (favorites[pick].Data.DataType == PlacableDataType.Treat && Random.value < controller.EatProbability)
                            || (favorites[pick].Data.DataType == PlacableDataType.Toy && Random.value < controller.PlayProbability))
                        {
                            controller.target = favorites[pick];
                            controller.targetPos = controller.target.transform.localPosition;
                            controller.justEntered = false;
                        }
                        else
                        {
                            controller.target = null;
                            controller.targetPos = PlacementManager.Instance.GetRandomInArea();
                        }
                    }
                    else
                    {
                        controller.target = null;
                        controller.targetPos = PlacementManager.Instance.GetRandomInArea();
                    }
                }
                else
                {
                    controller.isLeaving = true;
                    controller.target = null;
                    controller.targetPos = controller.playArea.CatSpawnPoint.localPosition;
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
            controller.navAgent.speed = controller.WalkSpeed * controller.transform.lossyScale.x;
            //controller.navAgent.stoppingDistance = controller.StopDistance * controller.transform.lossyScale.x;
            controller.navAgent.isStopped = false;
            controller.navAgent.updatePosition = true;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            for (int i = 0; i < PlacementManager.Instance.chasables.Count; i++)
            {
                if (PlacementManager.Instance.chasables[i].Attraction(controller) >= 1f
                    && (controller.chaseTarget == null || PlacementManager.Instance.chasables[i].Attraction(controller) > controller.chaseTarget.Attraction(controller)))
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
            Vector3 targetWorldPos = PlacementManager.Instance.GetWorldNavPos(controller.targetPos);
            controller.navAgent.SetDestination(targetWorldPos);
            float distToTarget = Vector3.Distance(controller.transform.position, targetWorldPos);
            float worldStopDist = controller.StopDistance * controller.transform.lossyScale.x;
            if (distToTarget <= worldStopDist)
            {
                controller.navAgent.isStopped = true;
                controller.navAgent.updatePosition = false;
                controller.transform.LookAt(targetWorldPos, PlacementManager.Instance.GetPlayArea().transform.up);
                if (controller.target != null)
                {
                    if (controller.target.Data.DataType == PlacableDataType.Treat)
                        controller.SetState<EatState>();
                    else
                        controller.SetState<PlayState>();
                }
                else if (controller.isLeaving)
                {
                    CatManager.Instance.LeaveArea(controller.catData);
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
                /*float movement = controller.WalkSpeed * controller.transform.lossyScale.x * Time.deltaTime;
                if (movement < distToTarget - worldStopDist)
                    controller.transform.position += controller.transform.forward * movement;
                else
                    controller.transform.position += controller.transform.forward * (distToTarget - worldStopDist);*/
                if (controller.navAgent.desiredVelocity.magnitude > 0)
                    controller.transform.rotation = Quaternion.LookRotation(controller.navAgent.desiredVelocity.normalized, PlacementManager.Instance.GetPlayArea().transform.up);
            }
        }
    }

    public class TutorialWalkState : CatState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            controller.animator.SetTrigger("Walk");
            controller.navAgent.speed = controller.WalkSpeed * controller.transform.lossyScale.x;
            //controller.navAgent.stoppingDistance = controller.StopDistance * controller.transform.lossyScale.x;
            controller.navAgent.isStopped = false;
            controller.navAgent.updatePosition = true;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            if (controller.target != null)
                controller.targetPos = controller.target.transform.localPosition;
            Vector3 targetWorldPos = PlacementManager.Instance.GetWorldNavPos(controller.targetPos);
            controller.navAgent.SetDestination(targetWorldPos);
            float distToTarget = Vector3.Distance(controller.transform.position, targetWorldPos);
            float worldStopDist = controller.StopDistance * controller.transform.lossyScale.x;
            if (distToTarget <= worldStopDist)
            {
                controller.navAgent.isStopped = true;
                controller.navAgent.updatePosition = false;
                controller.transform.LookAt(targetWorldPos, PlacementManager.Instance.GetPlayArea().transform.up);
                controller.SetState<TutorialSitState>();
            }
            else
            {
                /*float movement = controller.WalkSpeed * controller.transform.lossyScale.x * Time.deltaTime;
                if (movement < distToTarget - worldStopDist)
                    controller.transform.position += controller.transform.forward * movement;
                else
                    controller.transform.position += controller.transform.forward * (distToTarget - worldStopDist);*/
                if (controller.navAgent.desiredVelocity.magnitude > 0)
                    controller.transform.rotation = Quaternion.LookRotation(controller.navAgent.desiredVelocity.normalized, PlacementManager.Instance.GetPlayArea().transform.up);
            }
        }
    }

    public class TutorialSitState : CatState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            controller.animator.SetTrigger("Sit");
            if (HelpManager.Instance.CurrentStep == TutorialStep.Start)
            {
                //if (controller.HelpParticles != null)
                //controller.HelpParticles.SetActive(true);
                if (controller.HelpOutline != null)
                    controller.HelpOutline.enabled = true;
            }
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
            if (Random.value < controller.PurrProbability)
                controller.Purr();

            //if (controller.HelpParticles != null)
            //controller.HelpParticles.SetActive(false);
            if (controller.HelpOutline != null)
                controller.HelpOutline.enabled = false;
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
                if (HelpManager.Instance.CurrentStep == TutorialStep.Start)
                {
                controller.AddMood(1f / (CatManager.Instance.MoodColors.Length - 1));
                    HelpManager.Instance.CompleteTutorialStep(TutorialStep.Start);
                }
                else
                {
                    controller.AddMood(controller.PetMoodInc);
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
            if (treatController == null || !treatController.AnyLeft() || stateTime > controller.EatTime)
            {
                if (stateTime > controller.EatTime)
                    controller.AddMood(controller.EatMoodInc);
                PlacementManager.Instance.GetPlayArea().MarkAsDirty();
                PlayerManager.Instance.MarkAsDirty();
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
            ToyController toyController = controller.target == null ? null : controller.target.GetComponent<ToyController>();
            if (toyController == null || !toyController.AnyLeft() || stateTime > controller.PlayTime)
            {
                if (stateTime > controller.PlayTime)
                    controller.AddMood(controller.PlayMoodInc);
                PlacementManager.Instance.GetPlayArea().MarkAsDirty();
                PlayerManager.Instance.MarkAsDirty();
                controller.SetState<SitState>();
            }
            else
            {
                toyController.Play();
            }
        }

        public override void HitToy()
        {
            base.HitToy();
            if (controller.target != null && controller.target.GetComponent<AnimatedToy>() != null)
            {
                controller.target.GetComponent<AnimatedToy>().HitToy();
            }
        }
    }

    public class ChaseState : CatState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            controller.animator.SetTrigger("Run");
            controller.navAgent.speed = controller.RunSpeed * controller.transform.lossyScale.x;
            //controller.navAgent.stoppingDistance = controller.StopDistance * controller.transform.lossyScale.x;
            controller.navAgent.isStopped = false;
            controller.navAgent.updatePosition = true;
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
                Vector3 targetWorldPos = PlacementManager.Instance.GetNavPos(controller.chaseTarget.ChasePosition);
                controller.navAgent.SetDestination(targetWorldPos);
                float distToTarget = Vector3.Distance(controller.transform.position, targetWorldPos);
                float worldStopDist = controller.StopDistance * controller.transform.lossyScale.x;
                if (distToTarget <= worldStopDist)
                {
                    controller.navAgent.isStopped = true;
                    controller.navAgent.updatePosition = false;
                    controller.transform.LookAt(targetWorldPos, PlacementManager.Instance.GetPlayArea().transform.up);
                    controller.SetState<ChasePlayState>();
                }
                else
                {
                    /*float movement = controller.RunSpeed * controller.transform.lossyScale.x * Time.deltaTime;
                    if (movement < distToTarget - worldStopDist)
                        controller.transform.position += controller.transform.forward * movement;
                    else
                        controller.transform.position += controller.transform.forward * (distToTarget - worldStopDist);*/
                    if (controller.navAgent.desiredVelocity.magnitude > 0)
                        controller.transform.rotation = Quaternion.LookRotation(controller.navAgent.desiredVelocity.normalized, PlacementManager.Instance.GetPlayArea().transform.up);
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
            ToyController toyController = controller.chaseTarget == null ? null : controller.chaseTarget.Controller;
            if (toyController == null || controller.chaseTarget.ChasePosition == Vector3.zero || !toyController.AnyLeft() || stateTime > controller.PlayTime)
            {
                if (stateTime > controller.PlayTime)
                    controller.AddMood(controller.PlayMoodInc);
                PlacementManager.Instance.GetPlayArea().MarkAsDirty();
                PlayerManager.Instance.MarkAsDirty();
                controller.chaseTarget = null;
                controller.SetState<SitState>();
            }
            else
            {
                Vector3 targetWorldPos = PlacementManager.Instance.GetNavPos(controller.chaseTarget.ChasePosition);
                controller.transform.LookAt(targetWorldPos, PlacementManager.Instance.GetPlayArea().transform.up);
                float distToTarget = Vector3.Distance(controller.transform.position, targetWorldPos);
                float worldStopDist = controller.StopDistance * controller.transform.lossyScale.x;
                if (distToTarget > 2f * worldStopDist)
                {
                    controller.SetState<ChaseState>();
                }
                else
                {
                    //controller.transform.LookAt(targetWorldPos, PlacementManager.Instance.GetPlayArea().transform.up);
                    toyController.Play();
                }
            }
        }
    }

    public float WalkSpeed = 1f;
    public float RunSpeed = 2f;
    public float StopDistance = 1f;
    public float SitTime = 5f;
    public float EatTime = 5f;
    public float PlayTime = 5f;
    public float EatProbability = 1f;
    public float PlayProbability = 1f;
    public float[] MeowProbability;
    public float PurrProbability = 1f;
    public bool StayForever;
    public float[] StayLength;
    public float[] SpawnWait;
    public float[] AwayLength;
    public float PetMoodInc;
    public float EatMoodInc;
    public float PlayMoodInc;
    public float MoodDec;
    public AudioClip[] MeowSounds;
    public AudioClip[] PurrSounds;
    public AudioClip EatingSound;
    public Renderer[] MoodRenderers;
    public GameObject HelpParticles;
    public Outline HelpOutline;
    public ParticleSystem MoodUpParticles;
    public ParticleSystem MoodDownParticles;

    protected CatData catData;
    protected Animator animator;
    protected AudioSource ASource;
    protected NavMeshAgent navAgent;
    protected PlayArea playArea;
    protected Placable target;
    protected Vector3 targetPos;
    protected IChasable chaseTarget;
    protected bool isLeaving;
    List<CatState> states = new List<CatState>();
    CatState currentState;
    Material moodMat;
    float moodValue = 0.5f;
    int mood = 0;
    bool justEntered;
    
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

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;

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
        mood = CatManager.Instance.GetMood(catData);
        moodMat.color = CatManager.Instance.MoodColors[mood];

        if (HelpManager.Instance.CurrentStep == TutorialStep.Start)
        {
            targetPos = Vector3.zero;
            SetState<TutorialWalkState>();
        }
        else if (HelpManager.Instance.CurrentStep == TutorialStep.Mail)
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

    public void EnterArea()
    {
        justEntered = true;
    }

    protected void Meow()
    {
        int i = Random.Range(0, MeowSounds.Length);
        ASource.PlayOneShot(MeowSounds[i]);
    }

    protected void Purr()
    {
        int i = Random.Range(0, PurrSounds.Length);
        ASource.PlayOneShot(PurrSounds[i]);
    }

    protected void StartEatingAudio()
    {
        ASource.loop = true;
        ASource.clip = EatingSound;
        ASource.Play();
    }

    protected void StopEatingAudio()
    {
        ASource.Stop();
    }

    protected void AddMood(float amount)
    {
        moodValue = Mathf.Clamp01(moodValue + amount);
        CatManager.Instance.UpdateMood(catData, moodValue);
        int newMood = CatManager.Instance.GetMood(catData);
        if (newMood != mood)
        {
            moodMat.color = CatManager.Instance.MoodColors[newMood];
            if (newMood > mood)
            {
                ParticleSystem.MainModule mainModule = MoodUpParticles.main;
                mainModule.startColor = CatManager.Instance.MoodColors[newMood];
                MoodUpParticles.Emit(20);
            }
            else
            {
                ParticleSystem.MainModule mainModule = MoodDownParticles.main;
                mainModule.startColor = CatManager.Instance.MoodColors[newMood];
                MoodDownParticles.Emit(20);
            }
            mood = newMood;
        }
    }
    
    public override void Click(RaycastHit hit)
    {
        if (currentState is SitState || currentState is WalkState || currentState is TutorialSitState || currentState is TutorialWalkState)
        {
            SetState<PetState>();
        }
    }

    public void HitToy()
    {
        currentState.HitToy();
    }
}