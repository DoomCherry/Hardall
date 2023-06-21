using NaughtyAttributes;
using UnityEngine;

public class PlayerStepMarker : MonoBehaviour
{
    [BoxGroup("Main")]
    [SerializeField] private GameObject _playerAnimatable;

    [BoxGroup("Run")]
    [SerializeField] private float _runTickTime = 2;
    [BoxGroup("Run")]
    [SerializeField] private float _runStepMarkTime = 1;
    [BoxGroup("Run")]
    [SerializeField] private float _runStepMarkRadius = 1;

    [BoxGroup("Jump")]
    [SerializeField] private float _jumpStepMarkTime = 1;
    [BoxGroup("Jump")]
    [SerializeField] private float _jumpStepMarkRadius = 1;

    [BoxGroup("Grounded")]
    [SerializeField] private float _groundedStepMarkTime = 1;
    [BoxGroup("Grounded")]
    [SerializeField] private float _groundedStepMarkRadius = 1;

    private IPlayerAnimatable _playerAnimatableRef;
    public struct PlayerAnimatableBuffer
    {
        public bool isGrounded;
        public bool isJump;
        public float speed;
    }
    private PlayerAnimatableBuffer _animatableLastState;
    private Timer _runTimer;
    private Transform _selfTransform;
    private GameObject _selftGameObject;




    public IPlayerAnimatable PlayerAnimatable
    {
        get => _playerAnimatableRef = _playerAnimatableRef ??= _playerAnimatable.GetComponent<IPlayerAnimatable>();
    }
    public Transform SelfTransform
    {
        get => _selfTransform = _selfTransform ??= transform;
    }
    public GameObject SelfGameObject
    {
        get => _selftGameObject = _selftGameObject ??= gameObject;
    }




    private void OnValidate()
    {
        if (_playerAnimatable != null && !_playerAnimatable.TryGetComponent(out IPlayerAnimatable playerAnimatable))
            _playerAnimatable = null;
    }

    private void Start()
    {
        _runTimer = new Timer(_runTickTime);
    }

    private void Update()
    {
        if (StepsMarkHandler.Value == null)
            return;

        if (IsRun() && _runTimer.IsTicked())
            StepsMarkHandler.Value.AddStepMark(SelfGameObject, SelfTransform.position, _runStepMarkTime, _runStepMarkRadius, StepMarkType.Sound, "Run");

        if(IsStartJump())
            StepsMarkHandler.Value.AddStepMark(SelfGameObject, SelfTransform.position, _jumpStepMarkTime, _jumpStepMarkRadius, StepMarkType.Sound, "Jump");

        if (IsStartGrounded())
            StepsMarkHandler.Value.AddStepMark(SelfGameObject, SelfTransform.position, _groundedStepMarkTime, _groundedStepMarkRadius, StepMarkType.Sound, "Grounded");

        BufferisationAnimatable();
    }

    private bool IsRun() => PlayerAnimatable.Speed > PlayerAnimatable.MaxWalkSpeed;

    private bool IsStartJump() => PlayerAnimatable.IsJump == true && _animatableLastState.isJump == false;

    private bool IsStartGrounded() => PlayerAnimatable.IsGrounded == true && _animatableLastState.isGrounded == false;

    private void BufferisationAnimatable()
    {
        _animatableLastState.isGrounded = PlayerAnimatable.IsGrounded;
        _animatableLastState.isJump = PlayerAnimatable.IsJump;
        _animatableLastState.speed = PlayerAnimatable.Speed;
    }
}
