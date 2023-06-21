using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class LockableDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private HingeJoint _hingeJoint;
    [SerializeField] private Vector2 _closeLimits;
    [SerializeField] private ParticleSystem _closeEffect;
    [SerializeField, Min(0)] private float _angleCloseEffect;
    private Vector2 _defaultLimits;




    public UnityEvent OnOpen;
    public UnityEvent OnClose;




    private void Start()
    {
        _defaultLimits = new Vector2(_hingeJoint.limits.min, _hingeJoint.limits.max);
    }

    [Button]
    public void Interact()
    {
        _hingeJoint.useSpring = !_hingeJoint.useSpring;

        if (_hingeJoint.useSpring)
            CloseDoor();
        else
            OpenDoor();
    }

    private void OpenDoor()
    {
        SetJointLimits(_defaultLimits.x, _defaultLimits.y);
        OnOpen?.Invoke();
    }

    private void CloseDoor()
    {
        SetJointLimits(_closeLimits.x, _closeLimits.y);

        if (_closeEffect != null && Mathf.Abs(_hingeJoint.angle) > _angleCloseEffect)
        {
            _closeEffect.Stop();
            _closeEffect.Play();
        }

        OnClose?.Invoke();
    }

    private void SetJointLimits(float min, float max)
    {
        JointLimits jointLimits = _hingeJoint.limits;
        jointLimits.min = min;
        jointLimits.max = max;
        _hingeJoint.limits = jointLimits;
    }
}
