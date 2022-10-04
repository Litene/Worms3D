using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.Events;
using UnityEngine.Serialization;

// movement like worms, like it stops and continues
// closer camera
// fick jump
// walktimer

public enum PlayerTurn {
    ChooseWorm,
    Walk,
    SelectWeapon,
    Shoot,
    EndTurn
}


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    public Player Owner;
    private Worm _worm;
    private Vector3 _moveDir;
    private float _verticalInput;
    private float _horizontalInput;
    private Transform _cameraObject;
    private Rigidbody _rb;

    public UnityEvent OnImpact;

    private float _rotationSpeed = 15;
    [SerializeField] private Vector3 targetDirection;

    //public CameraManager _cameraManager;

    //spublic bool IsGrounded;
    public LayerMask groundLayer;
    [SerializeField] private Transform _groundChild;
    private CapsuleCollider _collider;
    public PlayerTurn turn;

    private float _inAirTimer;
    private float _jumpVelocity = 3;
    private float _fallingSpeed = 200;
    private float rayCastHeightOffset = 0.5f;
    private bool hitDetect;
    private RaycastHit hit;
    private float _groundCheckRange;
    [SerializeField] private bool startJumping;
    [SerializeField] private float jumpPower;
     public float WalkTimer;
    private float _groundCheckDistance = 0.05f;
    private float _groundCheckRadiusMultiplier = 0.9f;
    private bool _canWalk;


    [Header("new controller")] private Vector2 _playerInput;
    private Vector3 _velocity, _desiredVelocity;
    private float _maxAcceleration = 50f, _maxAirAcceleration = 18;
    private float _movementSpeed = 7f;
    [SerializeField] private bool _desiredJump;
    private const float MinFallTime = 1f;
    private const float MinYVelocity = -10f;

    private float _jumpHeight = 3;

    private float _fallTime = 0;
    private float _maxGroundAngle = 25f, _maxStairAngle = 50f;
    private float _minGroundDotProduct, _minStairDotProduct;
    private Vector3 _contactNormal, _steepContactNormal;
    private int _groundContactNormalCount, _steepContactNormalCount;
    private int _maxAirJumps = 0;
    private int _jumpPhase;
    private int _stepsSinceLastGrounded, _stepsSinceLastJump;
    private float _maxSnapSpeed = 100;
    private float probeDistance = 1f; // increase value 
    [SerializeField] private LayerMask _probeMask = -1, _stairMask = -1;
    public OrbitCamera _orbitCamera;
    [SerializeField] private Transform _playerInputSpace = default;
    private int _fallDamage;
    private const float E = 2.71828175f;
    private bool OnGround => _groundContactNormalCount > 0; //this could be the check, but this is true every frame
    private bool OnSteep => _steepContactNormalCount > 0;

    private void HandleMovement() {
        UpdateState();
        AdjustVelocity();
        if (_desiredJump) {
            _desiredJump = false;
            Jump();
        }

        _rb.velocity = _velocity;
        ClearState();
    }

    private float GetMinDot(int layer) {
        return (_stairMask & (1 << layer)) == 0 ? _minGroundDotProduct : _minStairDotProduct;
    }

    private void ClearState() {
        _groundContactNormalCount = _steepContactNormalCount = 0;
        _contactNormal = _steepContactNormal = Vector3.zero;
    }

    public void CharacterJump(float input) {
        _desiredJump |= input > 0.1f;
    }

    private void UpdateState() {
        _stepsSinceLastGrounded += 1;
        _stepsSinceLastGrounded = Mathf.Clamp(_stepsSinceLastGrounded, 0, int.MaxValue);
        _stepsSinceLastJump += 1;
        _stepsSinceLastJump = Mathf.Clamp(_stepsSinceLastJump, 0, Int32.MaxValue);
        _velocity = _rb.velocity;
        if (OnGround || SnapToGround()) {
            _stepsSinceLastGrounded = 0;
            _jumpPhase = 0;
            if (_groundContactNormalCount > 1) {
                _contactNormal.Normalize();
            }
        }
        else {
            _contactNormal = Vector3.up;
        }
    }

    bool SnapToGround() {
        if (_stepsSinceLastGrounded > 1 || _stepsSinceLastJump <= 2) return false;

        float speed = _velocity.magnitude;

        if (speed > _maxSnapSpeed) return false;

        if (!Physics.Raycast(_rb.position, Vector3.down, out RaycastHit hit, probeDistance)) return false;

        if (hit.normal.y < GetMinDot(hit.collider.gameObject.layer)) return false;

        _groundContactNormalCount = 1;
        _contactNormal = hit.normal;

        float dot = Vector3.Dot(_velocity, hit.normal);
        if (dot > 0f) {
            _velocity = (_velocity - hit.normal * dot).normalized * speed;
        }

        return true;
    }

    private Vector3 ProjetOnContactPlane(Vector3 vector) {
        return vector - _contactNormal * Vector3.Dot(vector, _contactNormal);
    }

    private void AdjustVelocity() {
        Vector3 xAxis = ProjetOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjetOnContactPlane(Vector3.forward).normalized;

        float currentX = Vector3.Dot(_velocity, xAxis);
        float currentZ = Vector3.Dot(_velocity, zAxis);

        float acceleration = OnGround ? _maxAcceleration : _maxAirAcceleration;
        float maxChangeSpeed = acceleration * Time.deltaTime;

        float newX = Mathf.MoveTowards(currentX, _desiredVelocity.x, maxChangeSpeed);
        float newZ = Mathf.MoveTowards(currentZ, _desiredVelocity.z, maxChangeSpeed);
        _velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    private void OnValidate() {
        _minGroundDotProduct = Mathf.Cos(_maxGroundAngle * Mathf.Deg2Rad);
        _minStairDotProduct = Mathf.Cos(_maxStairAngle * Mathf.Deg2Rad);
    }

    private void Jump() {
        if (OnGround || _jumpPhase < _maxAirJumps) {
            _stepsSinceLastJump = 0;
            _jumpPhase++;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * _jumpHeight);
            float alignedSpeed = Vector3.Dot(_velocity, _contactNormal);
            if (alignedSpeed > 0f) {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0);
            }

            _velocity += _contactNormal * jumpSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (!(_fallTime > 0)) return;
        
        if (_fallTime > MinFallTime) {
            _fallDamage = (int)(_fallTime * 10f);
            _worm.Health.Damage(_worm, _fallDamage);
        }
        _fallTime = 0;
    }

    private void OnCollisionStay(Collision collision) => EvaluateCollision(collision);
    private void OnCollisionExit(Collision collision) => EvaluateCollision(collision);

    private void EvaluateCollision(Collision collision) {
        float minDot = GetMinDot(collision.gameObject.layer);
        for (int i = 0; i < collision.contactCount; i++) {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minDot) {
                _groundContactNormalCount += 1;
                _contactNormal += normal;
            }
            else if (normal.y > -0.01f) {
                _steepContactNormalCount += 1;
                _steepContactNormal += normal;
            }

            ToggleSlippery(normal.y);
        }
    }

    private void ToggleSlippery(float normalY) {
        if (normalY > 0.7f) return;

        if (normalY < 0.3f)
            _rb.AddForce(Vector3.down * 10, ForceMode.Force);
    }

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _orbitCamera = FindObjectOfType<OrbitCamera>();
        _groundChild = transform.Find("GroundCheckObject");
        _collider = GetComponent<CapsuleCollider>();
        _worm = GetComponent<Worm>();
        if (Camera.main != null) _cameraObject = Camera.main.transform;

        _playerInputSpace = FindObjectOfType<OrbitCamera>().transform;
        OnValidate();
    }

    private void Start() {
        _groundCheckRange = _collider.bounds.extents.y + 0.01f;
        WalkTimer = 10;
    }

    public void InitializePlayerTurn() {
        DefaultToWalkingCheck();
        _canWalk = true;
    }

    private void DefaultToWalkingCheck() {
        // this is run too early, latestart could solve it but its an uggly solution.
        if (Owner._worms.Count <= 1) {
            if (turn == PlayerTurn.ChooseWorm) {
                turn = PlayerTurn.Walk;
                InputManager.Instance.EnableMovement();
                UIManager.Instance.ActivateMiddleTextImage(turn);
            }
        }
        else {
            InputManager.Instance.DisableMovement();
        }
    }

    public void SetMoveVector(Vector2 moveVector) {
        _playerInput.x = moveVector.x;
        _playerInput.y = moveVector.y;
        _playerInput = Vector2.ClampMagnitude(_playerInput, 1f);
    }

    public void EnterAction() {
        if (_orbitCamera.CameraIsPanning) return;

        switch (turn) {
            case PlayerTurn.ChooseWorm:
                InputManager.Instance.EnableMovement();
                turn = PlayerTurn.Walk;
                _canWalk = true;
                UIManager.Instance.ActivateMiddleTextImage(turn);
                ResetMovement();
                break;
            case PlayerTurn.Walk:
                InputManager.Instance.DisableMovement();
                _orbitCamera.ToggleCameraMode(true); // needs to remake to swap mode
                turn = PlayerTurn.SelectWeapon;
                UIManager.Instance.ActivateMiddleTextImage(turn);
                UIManager.Instance.ToggleAim(GameManager.Instance.CurrentPlayer);
                UIManager.Instance.SetWeaponSprite(_worm._currentWeapon);
                ResetMovement();
                break;
            case PlayerTurn.SelectWeapon:
                turn = PlayerTurn.Shoot;
                UIManager.Instance.ActivateMiddleTextImage(turn);
                break;
            case PlayerTurn.Shoot:
                GameManager.Instance.NextPlayer();
                turn = PlayerTurn.ChooseWorm;
                UIManager.Instance.ToggleAim(GameManager.Instance.CurrentPlayer);
                UIManager.Instance.ActivateMiddleTextImage(turn);
                //DefaultToWalkingCheck();
                break;
            default:
                break;
        }
    }

    public void SpaceAction() {
        switch (turn) {
            case PlayerTurn.ChooseWorm:
                GameManager.Instance.NextWorm();
                break;
            case PlayerTurn.SelectWeapon:
                Owner._currentWorm.ChangeCurrentWeapon(false);
                break;
            case PlayerTurn.Shoot:
                break;
            default:
                break;
        }
    }

    private void FixedUpdate() {
        HandleMovement();
    }

    private void StopTransform() {
        _rb.velocity = Vector3.zero;
        _playerInput = Vector2.zero;
        // _desiredVelocity = Vector3.zero;
    }

    private void Update() {
        FallTimer(_rb.velocity.y);
        UIManager.Instance.UpdateWalkingTimer(GameManager.Instance.CurrentPlayer);
        
        if (_playerInputSpace) {
            Vector3 forward = _playerInputSpace.forward;
            forward.y = 0;
            forward.Normalize();
            Vector3 right = _playerInputSpace.right;
            right.y = 0;
            right.Normalize();
            _desiredVelocity = (forward * _playerInput.y + right * _playerInput.x) * _movementSpeed;
        }
        else {
            _desiredVelocity = new Vector3(_playerInput.x, 0, _playerInput.y) * _movementSpeed;
        }

        if (turn == PlayerTurn.Walk && _desiredVelocity != Vector3.zero && _canWalk) {
            if (WalkTimer > 0) {
                WalkTimer -= Time.deltaTime;
            }
            else if (turn == PlayerTurn.Walk) {
                EnterAction();
                ResetMovement();
                _canWalk = false;
            }
        }
    }

    public void ResetMovement() {
        StopTransform();
        //InputManager.Instance.DisableMovement();
        WalkTimer = 10;
    }

    private void FallTimer(float currentYVelocity) {
        // last highest velocity, return tuple... 
        //float minFallTime = 5.5f;
        //float minYVelocity = -10f; //change to a good 

        if (currentYVelocity < 0) _fallTime += Time.deltaTime;


        //
        // //Debug.Log(currentYVelocity);
        // if (_fallTime > minFallTime) {
        //     if (currentYVelocity < minYVelocity) {
        //         //return (_fallTime * currentYVelocity);
        //     }
        // }
        //
        // return new Tuple<float, float>(0, 0);
    }
}