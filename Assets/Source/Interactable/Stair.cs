using PathCreation;
using UnityEngine;

public enum StairWalkDirection
{
    Forward,
    Reverse
}

public class Stair : MonoBehaviour, IInteractableWithOneParameter<PlayerStairWalker>
{
    [SerializeField] private PathCreator _currentPath;
    [SerializeField] private StairWalkDirection _stairWalkDirection;
    [SerializeField] private float _timeByLength = 1;




    public void Interact(PlayerStairWalker player)
    {
        player.ResetPath(_currentPath, _stairWalkDirection);
    }
}
