using UnityEngine;

public struct EnemyAnimatorControllerData
{
    public string _idleAnimationName;
    public string _walkAnimationName;
    public string _runAnimationName;

    public AnimationTripletNames _jump;

    public Animator _animator;
    public SpriteRenderer _spriteRenderer;
    public IEnemyAnimatable _animatable;




    public EnemyAnimatorControllerData(Animator animator, SpriteRenderer spriteRenderer, IEnemyAnimatable animatable,
        AnimationTripletNames idleWalkRun, AnimationTripletNames jump)
    {
        _animator = animator;
        _spriteRenderer = spriteRenderer;
        _animatable = animatable;

        _jump = jump;

        _idleAnimationName = idleWalkRun._startActionName;
        _walkAnimationName = idleWalkRun._repeatPartActionName;
        _runAnimationName = idleWalkRun._endActionName;
    }
}

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] private string _idleAnimationName = "Idle";
    [SerializeField] private string _walkAnimationName = "Walk";
    [SerializeField] private string _runAnimationName = "Run";

    [SerializeField] private AnimationTripletNames _jump;

    [SerializeField] private Animator _customAnimator;
    [SerializeField] private SpriteRenderer _customSpriteRenderer;
    private Animator _animator;

    [SerializeField] private GameObject _enemyAnimatable;
    private IEnemyAnimatable _animatable;

    private SpriteRenderer _spriteRenderer;
    private EnemyAnimatorControllerData _data;
    private EnemyAnimationStateMachine _enemyAnimationStateMachine;




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
    public IEnemyAnimatable EnemyAnimatable
    {
        get => _animatable = _animatable ??= GetComponent<IEnemyAnimatable>();
    }




    private void OnValidate()
    {
        if (_enemyAnimatable && !_enemyAnimatable.TryGetComponent(out IEnemyAnimatable enemyAnimatable))
            _enemyAnimatable = null;
    }

    private void Start()
    {
        _data = new EnemyAnimatorControllerData(Animator, SpriteRenderer, EnemyAnimatable,
            new AnimationTripletNames() { _startActionName = _idleAnimationName, _repeatPartActionName = _walkAnimationName, _endActionName = _runAnimationName },
            _jump);

        _enemyAnimationStateMachine = new EnemyAnimationStateMachine(_data);
    }

    private void Update()
    {
        _enemyAnimationStateMachine.Tick();
    }
}
