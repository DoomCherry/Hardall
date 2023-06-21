using UnityEngine;
using UnityEngine.AI;

public interface IEnemyAnimatable
{
    float Speed { get; }
    float MaxWalkSpeed { get; }
    float FallingDirection { get; }
    bool IsJump { get; }
    bool IsGrounded { get; }
    bool IsForward { get; }
    Vector2 MoveDirection { get; }
}

[RequireComponent(typeof(NavMeshAgent), typeof(AiTeleportator))]
public class EnemyLocomotion : MonoBehaviour, IEnemyAnimatable
{
    [SerializeField] private Transform _destination;

    private NavMeshAgent _navMeshAgent;
    private AiTeleportator _aiTeleportator;
    private float _maxSpeed = 1;
    private bool _isJump = false;
    private bool _isForward;




    public NavMeshAgent NavMeshAgent
    {
        get => _navMeshAgent = _navMeshAgent ??= GetComponent<NavMeshAgent>();
    }
    public AiTeleportator AiTeleportator
    {
        get => _aiTeleportator = _aiTeleportator ??= GetComponent<AiTeleportator>();
    }
    public float Speed => NavMeshAgent.velocity.magnitude;
    public float MaxWalkSpeed => _maxSpeed;
    public float FallingDirection => 0;
    public bool IsJump => _isJump;
    public bool IsGrounded => NavMeshAgent.enabled && NavMeshAgent.isOnNavMesh;
    public Vector2 MoveDirection => new Vector2(NavMeshAgent.velocity.z, NavMeshAgent.velocity.y);
    public bool IsForward => _isForward;




    private void OnEnable()
    {
        AiTeleportator.OnTeleportStart += OnTeleportStarted;
        AiTeleportator.OnTeleportEnd += OnTeleportEnded;
    }

    private void OnDisable()
    {
        AiTeleportator.OnTeleportStart -= OnTeleportStarted;
        AiTeleportator.OnTeleportEnd -= OnTeleportEnded;
    }

    private void FixedUpdate()
    {
        if (NavMeshAgent.isOnNavMesh)
            NavMeshAgent.SetDestination(new Vector3(_destination.position.x, _destination.position.y, _destination.position.z));

        _isJump = false;

        float zSign = _destination.position.z - transform.position.z;
        _isForward = zSign == 0 ? _isForward : zSign < 0;
    }

    private void OnTeleportStarted()
    {
        NavMeshAgent.enabled = false;
        _isJump = true;
    }

    private void OnTeleportEnded()
    {
        _destination.position = transform.position;
        NavMeshAgent.enabled = true;
    }
}
