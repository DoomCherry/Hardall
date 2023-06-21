using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour, IPlayerAnimatable
{
    [SerializeField] private float _maxWalkSpeed = 3;
    [SerializeField] private float _maxRunSpeedMult = 2;
    [SerializeField] private float _jumpPower = 200;
    [SerializeField] private float _jumpInterval = 0.1f;
    [SerializeField] private int _jumpCount = 2;

    private float _currentSpeed;
    private int _maxJumpCount;
    private Rigidbody _rigidbody;
    private Vector2 _moveDirection;
    private CapsuleCollider _capsuleCollider;
    private PlayerStairWalker _playerStairWalker;
    private Vector3 _lastPosition;




    public float Speed => _currentSpeed;
    public float MaxWalkSpeed => _maxWalkSpeed;
    public float FallingDirection => Rigidbody.velocity.y;
    public bool IsOnStreet => false;
    public bool IsJump { get; set; }
    public bool IsInStair => PlayerStairWalker.IsOnStair;
    public Vector2 MoveStairDirection
    {
        get
        {
            Debug.Log(PlayerStairWalker.StairDirection);
            return PlayerStairWalker.StairDirection;
        }
    }
    public Vector2 MoveDirection => _moveDirection;
    public bool IsGrounded
    {
        get
        {
            return CapsuleCollider.Overlap(enableDebugMode: true, offsetPosition: Vector3.up * -0.002f)
                                  .Select(n => n.GetComponent<Ground>())
                                  .Where(n => n != null).Count() > 0;
        }
    }
    private Rigidbody Rigidbody
    {
        get => _rigidbody = _rigidbody ??= GetComponent<Rigidbody>();
    }
    private CapsuleCollider CapsuleCollider
    {
        get
        {
            return _capsuleCollider = _capsuleCollider ??= GetComponent<CapsuleCollider>();
        }
    }
    private PlayerStairWalker PlayerStairWalker
    {
        get => _playerStairWalker = _playerStairWalker ??= GetComponent<PlayerStairWalker>();
    }




    private void Start()
    {
        _maxJumpCount = _jumpCount;
    }
    private void FixedUpdate()
    {
        if (IsGrounded && !IsJump)
            _jumpCount = _maxJumpCount;

        _moveDirection = Vector2.zero;
        float runMult = 1;

        if (Input.GetKey(KeyCode.A))
        {
            _moveDirection = Vector2.left;
        }

        if (Input.GetKey(KeyCode.D))
        {
            _moveDirection = Vector2.right;
        }

        if (Input.GetKey(KeyCode.LeftShift))
            runMult = _maxRunSpeedMult;

        _currentSpeed = Mathf.Abs(_moveDirection.x) * _maxWalkSpeed * runMult;
        Rigidbody.velocity = new Vector3(0, Rigidbody.velocity.y, _moveDirection.normalized.x * _maxWalkSpeed * runMult * Time.deltaTime);
        _lastPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TrySimpleInteract();
            TryPlayerParamInteract();
            TryPlayerStairWalkerParamInteract();
        }

        if (!IsJump && Input.GetKeyDown(KeyCode.Space) && _jumpCount > 0)
        {
            IsJump = true;
            Rigidbody.AddForce(Vector3.up * _jumpPower);
            _jumpCount--;
            DOTween.Sequence().AppendInterval(_jumpInterval).AppendCallback(() => IsJump = false);
        }
    }

    private void TrySimpleInteract()
    {
        IEnumerable<Collider> interactZone = CapsuleCollider.Overlap(queryTriggerInteraction: QueryTriggerInteraction.Collide).Where(n => n.TryGetComponent(out IInteractable interactable));

        foreach (var item in interactZone)
            item.GetComponent<IInteractable>().Interact();
    }

    private void TryPlayerParamInteract()
    {
        InteractWithParam(this);
    }

    private void TryPlayerStairWalkerParamInteract()
    {
        if (PlayerStairWalker == null)
            return;

        InteractWithParam(PlayerStairWalker);
    }

    private void InteractWithParam<T>(T param)
    {
        IEnumerable<Collider> interactZone = CapsuleCollider.Overlap(queryTriggerInteraction: QueryTriggerInteraction.Collide)
                                                            .Where(n => n.TryGetComponent(out IInteractableWithOneParameter<T> interactable));

        foreach (var item in interactZone)
            item.GetComponent<IInteractableWithOneParameter<T>>().Interact(param);
    }
}
