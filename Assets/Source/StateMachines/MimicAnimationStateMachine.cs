using StateMachineCore;
using System;
using UnityEngine;

public class MimicAnimationStateMachine : StateMachine
{
    private class State : IState
    {
        private int _tickCount = 0;
        private MimicAnimatorControllerData? _data;
        private StateMachine _stateMachine;




        public int StateCycleCount => _tickCount;
        public MimicAnimatorControllerData? Data
        {
            get => _data;
        }
        public Animator Animator
        {
            get => Data.Value._animator;
        }
        public SpriteRenderer SpriteRenderer
        {
            get => Data.Value._spriteRenderer;
        }
        public IMimicAnimatable EnemyAnimatable
        {
            get => Data.Value._animatable;
        }
        public StateMachine StateMachine
        {
            get => _stateMachine;
        }
        public string CurrentIdleAnimationName
        {
            get
            {
                return Data.Value._idleAnimationName;
            }
        }
        public string WalkAnimationName
        {
            get
            {
                return Data.Value._walkAnimationName;
            }
        }




        public event Action OnStartState;
        public event Action OnEndState;



        public State(StateMachine stateMachine, MimicAnimatorControllerData? data)
        {
            _data = data;
            _stateMachine = stateMachine;
        }

        public virtual void End(Action endState)
        {
            OnEndState?.Invoke();
        }

        public virtual void Start(Action startState)
        {
            OnStartState?.Invoke();
        }

        public virtual void Tick()
        {
            _tickCount++;
        }

        public AnimationClip CurrentAnimationClip()
        {
            AnimatorClipInfo[] infos = _data.Value._animator.GetCurrentAnimatorClipInfo(ANIMATION_LAYER);
            return infos[0].clip;
        }
    }
    private class NormalState : State
    {
        public NormalState(StateMachine stateMachine, MimicAnimatorControllerData data) : base(stateMachine, data)
        {

        }

        public override void Start(Action startState)
        {
            base.Start(startState);
            startState?.Invoke();
        }

        public override void Tick()
        {
            if (EnemyAnimatable == null)
                return;

            SpriteRenderer.flipX = EnemyAnimatable.MoveDirection.x == 0 ? SpriteRenderer.flipX : EnemyAnimatable.MoveDirection.x < 0;


            if (EnemyAnimatable.Speed == 0)
                PlayIdle();

            if (EnemyAnimatable.Speed > 0 && EnemyAnimatable.Speed <= EnemyAnimatable.MaxWalkSpeed)
                PlayWalk();

            base.Tick();
        }

        public override void End(Action endState)
        {
            endState.Invoke();
            base.End(endState);
        }

        private void PlayIdle()
        {
            if (CurrentAnimationClip().name == CurrentIdleAnimationName)
                return;

            Animator.Play(CurrentIdleAnimationName);
        }

        private void PlayWalk()
        {
            if (CurrentAnimationClip().name == WalkAnimationName)
                return;

            Animator.ForcePlay(WalkAnimationName);
        }
    }

    private IState _normal;

    private const int ANIMATION_LAYER = 0;


    public MimicAnimationStateMachine(MimicAnimatorControllerData data)
    {
        _normal = new NormalState(this, data);

        AddState(_normal);

        Initialize<NormalState>();
    }

    public override void Tick()
    {
        base.Tick();
    }
}
