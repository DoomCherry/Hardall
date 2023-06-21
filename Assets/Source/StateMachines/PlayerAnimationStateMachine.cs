using DG.Tweening;
using StateMachineCore;
using System;
using UnityEngine;

public class PlayerAnimationStateMachine : StateMachine
{
    private class State : IState
    {
        private int _tickCount = 0;
        private PlayerAnimatorControllerData? _data;
        private StateMachine _stateMachine;




        public int StateCycleCount => _tickCount;
        public PlayerAnimatorControllerData? Data
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
        public IPlayerAnimatable PlayerAnimatable
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
                if (PlayerAnimatable == null)
                    return Data.Value._idleAnimationName;

                return PlayerAnimatable.IsOnStreet ? Data.Value._idleInStreetAnimationName : Data.Value._idleAnimationName;
            }
        }
        public string CurrentStairAnimation
        {
            get
            {
                if (PlayerAnimatable.MoveStairDirection.x < 0)
                    return Data.Value.stairWalkBackwardName;

                return PlayerAnimatable.MoveStairDirection.y > 0 ? Data.Value.stairWalkUpForwardName : Data.Value.stairWalkDownForwardName;
            }
        }
        public string WalkAnimationName
        {
            get
            {
                return Data.Value._walkAnimationName;
            }
        }
        public string RunAnimationName
        {
            get
            {
                return Data.Value._runAnimationName;
            }
        }
        public AnimationTripletNames CurrentJumpAnimationNameTriplet
        {
            get
            {
                if (PlayerAnimatable == null)
                    return Data.Value._jump;

                return PlayerAnimatable.Speed > 0 ? Data.Value._jumpInRun : Data.Value._jump;
            }
        }




        public event Action OnStartState;
        public event Action OnEndState;



        public State(StateMachine stateMachine, PlayerAnimatorControllerData? data)
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
        public NormalState(StateMachine stateMachine, PlayerAnimatorControllerData data) : base(stateMachine, data)
        {

        }

        public override void Start(Action startState)
        {
            base.Start(startState);
            startState?.Invoke();
        }

        public override void Tick()
        {
            if (PlayerAnimatable == null)
                return;

            SpriteRenderer.flipX = PlayerAnimatable.MoveDirection.x == 0 ? SpriteRenderer.flipX : PlayerAnimatable.MoveDirection.x < 0;

            if (!PlayerAnimatable.IsGrounded)
                StateMachine.StartState<FallState>();

            if (PlayerAnimatable.IsJump)
                StateMachine.StartState<JumpState>();

            if (PlayerAnimatable.IsInStair)
                StateMachine.StartState<StairState>();

            if (PlayerAnimatable.Speed == 0)
                PlayIdle();

            if (PlayerAnimatable.Speed > 0 && PlayerAnimatable.Speed <= PlayerAnimatable.MaxWalkSpeed)
                PlayWalk();

            if (PlayerAnimatable.Speed > PlayerAnimatable.MaxWalkSpeed)
                PlayRun();

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

        private void PlayRun()
        {
            if (CurrentAnimationClip().name == RunAnimationName)
                return;

            Animator.ForcePlay(RunAnimationName);
        }
    }
    private class FallState : State
    {
        public FallState(StateMachine stateMachine, PlayerAnimatorControllerData data) : base(stateMachine, data)
        {

        }

        public override void Start(Action startState)
        {
            base.Start(startState);
            startState?.Invoke();
        }

        public override void Tick()
        {
            if (PlayerAnimatable == null)
                return;

            if (!PlayerAnimatable.IsGrounded)
            {
                PlayFall();
                return;
            }

            StateMachine.StartState<NormalState>();



            base.Tick();
        }

        public override void End(Action endState)
        {
            PlayFallEnd(OnEnded);
            base.End(endState);


            void OnEnded()
            {
                DOTween.Sequence().AppendInterval(CurrentAnimationClip().length).AppendCallback(endState.Invoke);
            }
        }

        private void PlayFall()
        {
            if (CurrentAnimationClip().name == CurrentJumpAnimationNameTriplet._repeatPartActionName)
                return;

            Animator.ForcePlay(CurrentJumpAnimationNameTriplet._repeatPartActionName);
        }

        private void PlayFallEnd(Action callback)
        {
            if (CurrentAnimationClip().name == CurrentJumpAnimationNameTriplet._endActionName)
                return;

            Animator.ForcePlay(CurrentJumpAnimationNameTriplet._endActionName);

            callback?.Invoke();
        }
    }
    private class JumpState : State
    {
        public JumpState(StateMachine stateMachine, PlayerAnimatorControllerData data) : base(stateMachine, data)
        {

        }

        public override void Start(Action startState)
        {
            base.Start(startState);
            startState?.Invoke();
        }

        public override void Tick()
        {
            base.Tick();
            PlayStartJump(JumpEnded);



            void JumpEnded()
            {
                DOTween.Sequence().AppendInterval(CurrentAnimationClip().length).AppendCallback(StateMachine.StartState<FallState>);
            }
        }

        public override void End(Action endState)
        {
            base.End(endState);
            endState?.Invoke();
        }

        private void PlayStartJump(Action onJumpEnded)
        {
            if (CurrentAnimationClip().name == CurrentJumpAnimationNameTriplet._startActionName)
                return;

            Animator.ForcePlay(CurrentJumpAnimationNameTriplet._startActionName);
            onJumpEnded?.Invoke();
        }
    }
    private class StairState : State
    {
        public StairState(StateMachine stateMachine, PlayerAnimatorControllerData data) : base(stateMachine, data)
        {

        }

        public override void Start(Action startState)
        {
            base.Start(startState);
            startState?.Invoke();
        }

        public override void Tick()
        {
            base.Tick();
            Animator.ForcePlay(CurrentStairAnimation);
            if (!PlayerAnimatable.IsInStair)
                OnStairEnded();



            void OnStairEnded()
            {
                StateMachine.StartState<NormalState>();
            }
        }

        public override void End(Action endState)
        {
            base.End(endState);
            endState?.Invoke();
        }
    }

    private IState _normal;
    private IState _fall;
    private IState _jump;
    private IState _stair;

    private const int ANIMATION_LAYER = 0;


    public PlayerAnimationStateMachine(PlayerAnimatorControllerData data)
    {
        _normal = new NormalState(this, data);
        _fall = new FallState(this, data);
        _jump = new JumpState(this, data);
        _stair = new StairState(this, data);

        AddState(_normal);
        AddState(_fall);
        AddState(_jump);
        AddState(_stair);

        Initialize<NormalState>();
    }

    public override void Tick()
    {
        base.Tick();
    }
}
