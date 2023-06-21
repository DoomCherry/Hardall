using System;
using UnityEngine;

[Serializable]
public struct AnimationTripletNames
{
    public string _startActionName;
    public string _repeatPartActionName;
    public string _endActionName;
}

public interface IPlayerAnimatable
{
    float Speed { get; }
    float MaxWalkSpeed { get; }
    float FallingDirection { get; }
    bool IsOnStreet { get; }
    bool IsJump { get; }
    bool IsGrounded { get; }
    bool IsInStair { get; }
    Vector2 MoveStairDirection { get; }
    Vector2 MoveDirection { get; }
}

public struct PlayerAnimatorControllerData
{
    public string _idleAnimationName;
    public string _walkAnimationName;
    public string _runAnimationName;

    public string stairWalkUpForwardName;
    public string stairWalkDownForwardName;
    public string stairWalkBackwardName;

    public string _idleInStreetAnimationName;

    public AnimationTripletNames _jump;
    public AnimationTripletNames _jumpInRun;

    public Animator _animator;
    public SpriteRenderer _spriteRenderer;
    public IPlayerAnimatable _animatable;




    public PlayerAnimatorControllerData(Animator animator, SpriteRenderer spriteRenderer, IPlayerAnimatable animatable,
        string stairUF, string stairDF, string stairB,
        AnimationTripletNames idleWalkRun, AnimationTripletNames jump, AnimationTripletNames jumpInRun, AnimationTripletNames idleWalkRunInStreet)
    {
        stairWalkUpForwardName = stairUF;
        stairWalkDownForwardName = stairDF;
        stairWalkBackwardName = stairB;

        _animator = animator;
        _spriteRenderer = spriteRenderer;
        _animatable = animatable;

        _jump = jump;
        _jumpInRun = jumpInRun;

        _idleAnimationName = idleWalkRun._startActionName;
        _walkAnimationName = idleWalkRun._repeatPartActionName;
        _runAnimationName = idleWalkRun._endActionName;

        _idleInStreetAnimationName = idleWalkRunInStreet._startActionName;
    }
}

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField] private string _idleAnimationName = "Idle";
    [SerializeField] private string _walkAnimationName = "Walk";
    [SerializeField] private string _runAnimationName = "Run";

    [SerializeField] private string _stairWalkUpForwardName = "StairUpForward";
    [SerializeField] private string _stairWalkDownForwardName = "StairDownForward";
    [SerializeField] private string _stairWalkBackwardName = "StairBackward";

    [SerializeField] private string _idleInStreetAnimationName = "IdleInStreet";

    [SerializeField] private AnimationTripletNames _jump;
    [SerializeField] private AnimationTripletNames _jumpInRun;

    [SerializeField] private Animator _customAnimator;
    [SerializeField] private SpriteRenderer _customSpriteRenderer;
    private Animator _animator;

    [SerializeField] private GameObject _playerAnimatable;
    private IPlayerAnimatable _animatable;

    private SpriteRenderer _spriteRenderer;
    private PlayerAnimatorControllerData _data;
    private PlayerAnimationStateMachine _playerAnimationStateMachine;




    public Animator Animator
    {
        get
        {
            if (_customAnimator)
                return _customAnimator;

            return _animator = _animator ??= GetComponent<Animator>();
        }
    }
    public SpriteRenderer SpriteRenderer
    {
        get
        {
            if (_customSpriteRenderer)
                return _customSpriteRenderer;

            return _spriteRenderer = _spriteRenderer ??= GetComponent<SpriteRenderer>();
        }
    }
    public IPlayerAnimatable PlayerAnimatable
    {
        get => _animatable = _animatable ??= GetComponent<IPlayerAnimatable>();
    }




    private void OnValidate()
    {
        if (_playerAnimatable && !_playerAnimatable.TryGetComponent(out IPlayerAnimatable playerAnimatable))
            _playerAnimatable = null;
    }

    private void Start()
    {
        _data = new PlayerAnimatorControllerData(Animator, SpriteRenderer, PlayerAnimatable,
            _stairWalkUpForwardName, _stairWalkDownForwardName, _stairWalkBackwardName,
            new AnimationTripletNames() {_startActionName = _idleAnimationName, _repeatPartActionName = _walkAnimationName, _endActionName = _runAnimationName },
            _jump, 
            _jumpInRun, 
            new AnimationTripletNames() {_startActionName = _idleInStreetAnimationName, _repeatPartActionName = "", _endActionName = ""});

        _playerAnimationStateMachine = new PlayerAnimationStateMachine(_data);
    }

    private void Update()
    {
        _playerAnimationStateMachine.Tick();
    }
}
