using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;

[RequireComponent(typeof(CapsuleCollider))]
public class EnemyStepMarker : MonoBehaviour
{
    [SerializeField] private GameObject _enemyAnimatable;

    [MinValue(0), MaxValue(1)]
    [SerializeField] private float _frequancy = 0.5f;
    [SerializeField] private float _viewDistance = 10;

    private Transform _selfTransform;
    private GameObject _selftGameObject;
    private CapsuleCollider _capsuleCollider;
    private IEnumerable<RaycastHit> _oldHits = new RaycastHit[0];
    private IEnemyAnimatable _animatable;
    private float _lastTime = 0;




    public Transform SelfTransform
    {
        get => _selfTransform = _selfTransform ??= transform;
    }
    public GameObject SelfGameObject
    {
        get => _selftGameObject = _selftGameObject ??= gameObject;
    }
    public CapsuleCollider Collider
    {
        get => _capsuleCollider = _capsuleCollider ??= GetComponent<CapsuleCollider>();
    }
    public IEnemyAnimatable EnemyAnimatable
    {
        get
        {
            if (_enemyAnimatable == null)
                return null;

            return _animatable = _animatable ??= _enemyAnimatable.GetComponent<IEnemyAnimatable>();
        }
    }




    private void OnValidate()
    {
        if (_enemyAnimatable != null && !_enemyAnimatable.TryGetComponent(out IEnemyAnimatable enemy))
            _enemyAnimatable = null;
    }

    public void OnEnable()
    {
        PlayerLoopExtentions.ModifyCurrentPlayerLoop((ref PlayerLoopSystem system) =>
        {
            system.AddSystem<EnemyStepMarker>(LockForward);
        });
    }

    public void LockForward()
    {
        if (EnemyAnimatable == null)
            return;

        Debug.DrawRay(SelfTransform.position, Vector3.forward * (EnemyAnimatable.IsForward ? -1 : 1), Color.yellow);
        if (!TimeIsForLock())
            return;

        IEnumerable<RaycastHit> newHits = Physics.RaycastAll(SelfTransform.position, Vector3.forward * (EnemyAnimatable.IsForward ? -1 : 1), _viewDistance);

        foreach (var item in newHits)
        {
            if (item.collider.gameObject.GetComponent<Player>())
            {
                StepsMarkHandler.Value.AddStepMark(SelfGameObject, item.point, 1, 0, StepMarkType.Visual, "I See player");
                continue;
            }
        }

        _oldHits = newHits;
    }

    public bool TimeIsForLock()
    {
        if (Time.time - _lastTime < _frequancy)
            return false;

        _lastTime = Time.time;
        return true;

    }

    public void OnDisable()
    {
        PlayerLoopExtentions.ModifyCurrentPlayerLoop((ref PlayerLoopSystem system) =>
        {
            system.RemoveSystem<EnemyStepMarker>(false);
        });
    }
}
