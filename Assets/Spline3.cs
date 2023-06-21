using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineSpline))]
public class Spline3 : MonoBehaviour
{
    [SerializeField, ReadOnly] private List<Vector3> _points = new List<Vector3>();
    [SerializeField] private int _quality = 3;

    private LineSpline _lineSpline;



    public LineSpline LineSpline
    {
        get => _lineSpline = _lineSpline ??= GetComponent<LineSpline>();
    }




    private void OnDrawGizmos()
    {
        RefreshAllPoint();

        for (int i = 1; i < _points.Count; i++)
        {
            Color bufferColor = Gizmos.color;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(_points[i - 1], _points[i]);

            Gizmos.color = bufferColor;
        }
    }

    private void RefreshAllPoint()
    {
        _points.Clear();

        for (int i = 2; i < LineSpline.Points.Count; i += 2)
        {
            for (int j = 0; j <= _quality; j++)
            {
                float step = (float)j / _quality;

                Vector3 positionAB = Vector3.Lerp(LineSpline.Points[i - 2], LineSpline.Points[i - 1], step);
                Vector3 positionBC = Vector3.Lerp(LineSpline.Points[i - 1], LineSpline.Points[i], step);

                Vector3 newPoint = Vector3.Lerp(positionAB, positionBC, step);

                if (_points.Contains(newPoint) == false)
                    _points.Add(newPoint);
            }
        }
    }
}
