using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum StepMarkType
{
    Sound,
    Visual
}

public struct StepMark
{
    public float _maxLiveTime;
    public float _bornTime;
    public float _radius;
    public StepMarkType _type;
    public Vector3 _position;
    public GameObject _source;
    public string _reason;
}

public class StepsMarkHandler : MonoBehaviour
{
    [SerializeField] private bool _enableDebug = false;

    private static StepsMarkHandler _value;
    private List<StepMark> _stepMarks = new List<StepMark>(256);
    private Camera _mainCamera;




    public static StepsMarkHandler Value
    {
        get => _value;
    }
    public IReadOnlyList<StepMark> StepMarks => _stepMarks;
    public Camera MainCamera => _mainCamera = _mainCamera ??= GetComponent<Camera>();




    private void Awake()
    {
        if (_value != null)
        {
            Debug.Log($"{nameof(StepsMarkHandler)} most be a single in hierarhy right now!");
            return;
        }

        _value = this;
    }

    private void Update()
    {
        for (int i = 0; i < _stepMarks.Count; i++)
        {
            if (Time.time - _stepMarks[i]._bornTime > _stepMarks[i]._maxLiveTime)
            {
                RemoveStepMark(i);
                i--;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < _stepMarks.Count; i++)
        {
            if (_enableDebug)
            {
                Color defaultColor = Gizmos.color;
                Gizmos.color = _stepMarks[i]._type == StepMarkType.Sound ? Color.cyan : Color.red;

                Gizmos.DrawWireSphere(_stepMarks[i]._position, _stepMarks[i]._radius);

                Gizmos.color = defaultColor;
            }
        }
    }

    public int AddStepMark(GameObject source, Vector3 position, float maxLiveTime, float radius, StepMarkType type, string reason = "", bool activeteDebug = false)
    {
        int index = _stepMarks.Count;
        _stepMarks.Add(new StepMark() {_source = source,  _bornTime = Time.time, _maxLiveTime = maxLiveTime, _radius = radius, _type = type, _reason = reason, _position = position });

        if (activeteDebug || _enableDebug)
            Debug.Log($"{type} {_stepMarks.Last()} " + _stepMarks.Last()._bornTime + $" Has been added because: {reason}");

        return index;
    }

    public void RemoveStepMark(int index, bool activeteDebug = false)
    {
        if (activeteDebug)
            Debug.Log($"{_stepMarks[index]._type} {_stepMarks[index]} " + _stepMarks[index]._bornTime + $" Has been removed: {_stepMarks[index]._reason}");

        _stepMarks.RemoveAt(index);
    }

    public void ClearAllStepMarks()
    {
        _stepMarks.Clear();
    }
}
