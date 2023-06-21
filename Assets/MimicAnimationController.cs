using System;
using UnityEngine;

[Serializable]
public struct MimicAnimatorControllerData
{
    public string _idleAnimationName;
    public string _walkAnimationName;

    public string _wallIdleAnimationName;
    public string _wallWalkAnimationName;

    public string _floreToWallAnimationName;
    public string _wallToFloreAnimationName;

    public string _mimicryStart;
    public string _mimicryEnd;

    public string _chair, _endChair;

    public string _attack;

    public Animator _animator;
    public SpriteRenderer _spriteRenderer;
    public IMimicAnimatable _animatable;




    public MimicAnimatorControllerData(Animator animator, SpriteRenderer spriteRenderer, IMimicAnimatable animatable,
        string idle, string walk, string floreToWall,
        string wallIdle, string wallWalk, string wallToFlore,
        string mimicryStart, string mimicryEnd,
        string chair, string endChair,
        string attack)
    {
        _animator = animator;
        _spriteRenderer = spriteRenderer;
        _animatable = animatable;

        _idleAnimationName = idle;
        _walkAnimationName = walk;

        _wallIdleAnimationName = wallIdle;
        _wallWalkAnimationName = wallWalk;

        _floreToWallAnimationName = floreToWall;
        _wallToFloreAnimationName = wallToFlore;

        _mimicryStart = mimicryStart;
        _mimicryEnd = mimicryEnd;

        _chair = chair;
        _endChair = endChair;

        _attack = attack;
    }
}

public class MimicAnimationController : MonoBehaviour
{
    [SerializeField] private MimicAnimatorControllerData _data;
    [SerializeField] private GameObject _animatable;

    private MimicAnimationStateMachine _enemyAnimationStateMachine;




    public Animator Animator
    {
        get
        {
            return _data._animator = _data._animator ??= GetComponent<Animator>();
        }
    }
    public SpriteRenderer SpriteRenderer
    {
        get
        {
            return _data._spriteRenderer = _data._spriteRenderer ??= GetComponent<SpriteRenderer>();
        }
    }
    public IMimicAnimatable EnemyAnimatable
    {
        get => _data._animatable = _data._animatable ??= GetComponent<IMimicAnimatable>();
    }




    private void OnValidate()
    {
        IMimicAnimatable enemyAnimatable = null;

        if (_animatable && !_animatable.TryGetComponent(out enemyAnimatable))
            _animatable = null;
        
        if(enemyAnimatable != null)
        {
            _data._animatable = enemyAnimatable;
        }
    }

    private void Start()
    {
        _enemyAnimationStateMachine = new MimicAnimationStateMachine(_data);
    }

    private void Update()
    {
        _enemyAnimationStateMachine.Tick();
    }
}
