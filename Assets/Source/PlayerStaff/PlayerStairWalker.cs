using CoroutineExtension;
using PathCreation;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerStairWalker : MonoBehaviour
{
    [SerializeField] private float _timeByLength = 1;

    private PathCreator _currentPath;
    private Player _player;
    private float _currentPosition = 0;
    private StairWalkDirection _stairWalkDirection;




    public Player Player
    {
        get => _player = _player ??= GetComponent<Player>();
    }
    public bool IsOnStair => _currentPath;
    public Vector2 StairDirection
    {
        get
        {
            if (_currentPath == null || _currentPath.path.NumPoints == 0)
                return -Vector2.right;

            return new Vector2(_currentPosition - _currentPath.path.length * 0.5f, _stairWalkDirection == StairWalkDirection.Forward ? 1 : -1);
        }
    }




    public void ResetPath(PathCreator path, StairWalkDirection stairWalkDirection)
    {
        if (Player == null)
            return;

        if (_currentPath != null)
            return;

        Player.enabled = false;
        _stairWalkDirection = stairWalkDirection;
        _currentPath = path;
        this.RepeatUntil(IsEndOfStair, GoToStair, OnStairEnded, 0);
    }

    private bool IsEndOfStair()
    {
        return _currentPosition < _currentPath.path.length;
    }

    private void GoToStair()
    {
        float currentPosition = _stairWalkDirection == StairWalkDirection.Forward ? _currentPosition : _currentPath.path.length - _currentPosition;
        Player.transform.position = _currentPath.path.GetPointAtDistance(currentPosition, EndOfPathInstruction.Stop);
        _currentPosition += Time.deltaTime * _timeByLength;
    }

    private void OnStairEnded()
    {
        _currentPosition = 0;
        Player.enabled = true;
        _currentPath = null;
    }
}
