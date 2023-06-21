using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSpline : MonoBehaviour
{
    [SerializeField, ReadOnly] private List<Vector3> _points = new List<Vector3>();
    [SerializeField] private bool _isRecordMode = false;




    public IReadOnlyList<Vector3> Points
    {
        get => _points;
    }




    private void OnDrawGizmos()
    {
        if (_isRecordMode)
            RefreshAllPoint();

        for (int i = 1; i < _points.Count; i++)
        {
            Color bufferColor = Gizmos.color;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_points[i - 1], _points[i]);

            Gizmos.color = bufferColor;
        }
    }

    [Button]
    public void RefreshAllPoint()
    {
        _points.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            _points.Add(transform.GetChild(i).position);
        }
    }

    [Button]
    public void ImmitateCurrentPoint()
    {
        for (int i = 0; i < _points.Count; i++)
        {
            GameObject pointObject = new GameObject($"Point {i}");
            pointObject.transform.position = _points[i];
            pointObject.transform.SetParent(transform);
        }
    }
}
