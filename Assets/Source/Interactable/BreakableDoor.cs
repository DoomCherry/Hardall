using NaughtyAttributes;
using UnityEngine;

public class BreakableDoor : MonoBehaviour, IInteractableWithOneParameter<Vector3>
{
    [SerializeField] private HingeJoint _hingeJoint;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Vector3 _forceDirection;




    [Button]
    private void Interact()
    {
        Interact(_forceDirection);
    }

    public void Interact(Vector3 force)
    {
        _hingeJoint.breakForce -= force.magnitude;
        _rigidbody.AddTorque(force, ForceMode.Impulse);
    }
}
