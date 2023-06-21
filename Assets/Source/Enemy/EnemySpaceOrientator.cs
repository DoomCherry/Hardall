using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.LowLevel;

public enum EnemyOrientationState
{
    Rest,
    Chasing,
    Finding,
    Sleep
}

[RequireComponent(typeof(BoxCollider))]
public class EnemySpaceOrientator : MonoBehaviour
{
    [Required, BoxGroup("Rest state")]
    [SerializeField] private InterestingPointCollector _interestingPointCollector;

    [MinMaxSlider(0, 30), BoxGroup("Rest state")]
    [SerializeField] private Vector2 _waitRange;


    [MinMaxSlider(0, 30), BoxGroup("Find state")]
    [SerializeField] private Vector2 _findRange;

    [BoxGroup("Find state")]
    [SerializeField] private Vector3 _findPointRadius = new Vector3(3, 3, 0);

    [BoxGroup("Find state")]
    [SerializeField] private Vector3[] _findCenterOffsets = new Vector3[0];

    private BoxCollider _boxCollider;
    private Transform _selfTransform;
    private float _lastTime;
    private float _currentFrequancy = 0;
    private int _currentPointInterestingIndex = 0;
    private Vector3 _centerWithoutOffset;




    public BoxCollider BoxCollider
    {
        get => _boxCollider = _boxCollider ??= GetComponent<BoxCollider>();
    }
    public Transform SelfTransform
    {
        get => _selfTransform = _selfTransform ??= transform;
    }




    private void OnEnable()
    {
        EnableSystemByState(EnemyOrientationState.Rest);

        _centerWithoutOffset = SelfTransform.position;
    }

    public void ResetState(EnemyOrientationState enemyState)
    {
        DisableAllSystems();

        EnableSystemByState(enemyState);
    }

    public void FollowByStepsMark()
    {
        if (StepsMarkHandler.Value == null)
            return;

        List<StepMark> visualMark = StepsMarkHandler.Value.StepMarks
                                        .Where(n => n._type == StepMarkType.Visual)
                                        .OrderBy(n => Time.time - n._bornTime)
                                        .ToList();

        if (visualMark.Count() != 0)
        {
            SelfTransform.position = visualMark.First()._position;
            return;
        }

        List<StepMark> soundMark = StepsMarkHandler.Value.StepMarks
                                        .Where(n => n._type == StepMarkType.Sound && Vector3.Distance(n._position, SelfTransform.position) <= n._radius)
                                        .OrderBy(n => Time.time - n._bornTime)
                                        .ToList();

        if (soundMark.Count() == 0)
            return;

        StepMark nearestMark = soundMark.First();
        SelfTransform.position = nearestMark._position;
    }

    public void FollowByInterestingPoint()
    {
        if (_interestingPointCollector == null)
            return;

        if (!TimeIsForWaitRest())
            return;

        SelfTransform.position = _interestingPointCollector.GetPointBy(_currentPointInterestingIndex);
        _currentPointInterestingIndex++;
    }

    public void FollowByAutoGenerateInterestingPoint()
    {
        if (!TimeIsForFind())
            return;

        if (TryRandomPoint(_findPointRadius, out Vector3 position))
        {
            SelfTransform.position = position;
        }
    }

    public void OnDisable()
    {
        DisableAllSystems();
    }

    private bool TryRandomPoint(Vector3 range, out Vector3 result)
    {
        for (int i = 0; i < 50; i++)
        {
            int centerOffsetIndex = Random.Range(0, _findCenterOffsets.Length);
            Vector3 centerOffset = _findCenterOffsets.Length == 0 ? Vector3.zero : _findCenterOffsets[centerOffsetIndex];

            Vector3 randomPoint = _centerWithoutOffset + centerOffset + Vector3.Scale(Random.insideUnitSphere, range);

            NavMeshHit hit;
            NavMeshPath navMeshPath = new NavMeshPath();

            if (NavMesh.SamplePosition(randomPoint, out hit, 1, NavMesh.AllAreas) && NavMesh.CalculatePath(SelfTransform.position, hit.position, NavMesh.AllAreas, navMeshPath))
            {
                result = hit.position;
                _centerWithoutOffset = result - centerOffset;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    private bool TimeIsForWaitRest()
    {
        if (Time.time - _lastTime < _currentFrequancy)
            return false;

        RefreshFrequancyByWait();
        _lastTime = Time.time;
        return true;

    }

    private bool TimeIsForFind()
    {
        if (Time.time - _lastTime < _currentFrequancy)
            return false;

        RefreshFrequancyByFind();
        _lastTime = Time.time;
        return true;

    }

    private void RefreshFrequancyByWait()
    {
        _currentFrequancy = Random.Range(_waitRange.x, _waitRange.y);
    }

    private void RefreshFrequancyByFind()
    {
        _currentFrequancy = Random.Range(_findRange.x, _findRange.y);
    }

    private void EnableSystemByState(EnemyOrientationState enemyState)
    {
        System.Action currentSystem = FollowByInterestingPoint;

        switch (enemyState)
        {
            case EnemyOrientationState.Rest:
                break;
            case EnemyOrientationState.Chasing:
                currentSystem = FollowByStepsMark;
                break;
            case EnemyOrientationState.Finding:
                currentSystem = FollowByAutoGenerateInterestingPoint;
                break;
            case EnemyOrientationState.Sleep:
                return;
        }

        PlayerLoopExtentions.ModifyCurrentPlayerLoop((ref PlayerLoopSystem system) =>
        {
            system.AddSystem<EnemySpaceOrientator>(currentSystem.Invoke);
        });
    }

    private static void DisableAllSystems()
    {
        PlayerLoopExtentions.ModifyCurrentPlayerLoop((ref PlayerLoopSystem system) =>
        {
            system.RemoveSystem<EnemySpaceOrientator>(false);
        });
    }
}
