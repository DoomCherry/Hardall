using PathCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class AiTeleportator : MonoBehaviour
{
    [SerializeField] private LayerMask _teleport;
    [SerializeField] private float _timeByLength = 1;

    private PathCreator _currentPath;
    private CapsuleCollider _capsuleCollider;
    private float _currentPosition = 0;




    public event Action OnTeleportStart;
    public event Action OnTeleportEnd;




    public CapsuleCollider CapsuleCollider
    {
        get => _capsuleCollider = _capsuleCollider ??= GetComponent<CapsuleCollider>();
    }




    private void FixedUpdate()
    {
        if (_currentPath != null)
        {
            OnTeleportStart?.Invoke();
            MoveToPath();
            return;
        }

        if (IsCollideWithTeleport(out IEnumerable<PathCreator> paths))
            _currentPath = paths.First();
    }

    private void MoveToPath()
    {
        transform.position = _currentPath.path.GetPointAtDistance(_currentPosition, EndOfPathInstruction.Stop);
        _currentPosition += Time.deltaTime * _timeByLength;

        if (_currentPosition >= _currentPath.path.length)
        {
            _currentPosition = 0;
            _currentPath = null;
            OnTeleportEnd?.Invoke();
        }
    }

    private bool IsCollideWithTeleport(out IEnumerable<PathCreator> paths)
    {
        paths = CapsuleCollider.Overlap(_teleport, QueryTriggerInteraction.Collide)
            .Select(n => n.GetComponent<PathCreator>())
            .Where(n => n != null);

        return paths.Count() > 0;
    }
}
