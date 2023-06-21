using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterestingPointCollector : MonoBehaviour
{
    [SerializeField] private List<Vector3> _localPoints = new List<Vector3>();
    [SerializeField] private float _debugPointSize = 1;
    [SerializeField] private Color _debugPointColor = Color.white;
    [SerializeField] private bool _isRecord = false;

    private Transform _selfTransform;




    public Transform SelfTransform
    {
        get => _selfTransform = _selfTransform ??= GetComponent<Transform>();
    }




    private void OnDrawGizmos()
    {
        InitializeTransform();
    }

    private void OnDrawGizmosSelected()
    {
        if (_isRecord)
            CollectPoint();

        for (int i = 0; i < _localPoints.Count; i++)
        {
            Color defaultColor = Gizmos.color;
            Gizmos.color = _debugPointColor;

            Gizmos.DrawWireCube(GetPointBy(i), Vector3.one * _debugPointSize);

            Gizmos.color = defaultColor;
        }
    }

    public Vector3 GetPointBy(int index)
    {
        index %= _localPoints.Count;
        return SelfTransform.TransformPoint(_localPoints[index]);
    }

    [Button]
    public void CollectPoint()
    {
        _localPoints.Clear();

        for (int i = 0; i < SelfTransform.childCount; i++)
        {
            _localPoints.Add(SelfTransform.GetChild(i).localPosition);
        }
    }

    [Button]
    public void ImmitatePoint()
    {
        for (int i = 0; i < _localPoints.Count; i++)
        {
            GameObject point = new GameObject($"Point {i}");
            Transform transformPoint = point.transform;

            transformPoint.SetParent(SelfTransform);
            transformPoint.localPosition = _localPoints[i];
        }
    }

    private void InitializeTransform()
    {
        if (SelfTransform == null)
            _selfTransform = transform;
    }
}
