using System;
using UnityEngine;
using UnityEngine.InputSystem;

// movement like worms, like it stops and continues
// closer camera
// fick jump
// walktimer

public enum PlayerTurn {
    ChooseWorm,
    Walk,
    Shoot,
    EndTurn
}


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    public Player owner;
    private Vector3 _moveDir;
    private float _verticalInput;
    private float _horizontalInput;
    private Transform _cameraObject;
    private Rigidbody _rb;
    private float _movementSpeed = 7;
    private float _rotationSpeed = 15;
    [SerializeField] private Vector3 targetDirection;
    public CameraManager _cameraManager;
    public bool IsGrounded;
    public LayerMask groundLayer;
    [SerializeField] private Transform _groundChild;
    private Collider _collider;
    public PlayerTurn turn;

    private float _inAirTimer;
    private float _jumpVelocity = 3;
    private float _fallingSpeed = 33;
    private float rayCastHeightOffset = 0.5f;
    private bool hitDetect;
    private RaycastHit hit;
    private float _groundCheckRange;
    [SerializeField] private bool startJumping;
    [SerializeField] private float jumpPower;
    [SerializeField] private float walkTimer;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _cameraManager = FindObjectOfType<CameraManager>();
        _groundChild = transform.Find("GroundCheckObject");
        _collider = GetComponent<Collider>();
        if (Camera.main != null) _cameraObject = Camera.main.transform;
    }

    private void Start() {
        _groundCheckRange = _collider.bounds.extents.y + 0.01f;
        walkTimer = 5;
    }

    public void InitializePlayerTurn() { // fill with logic for the player?
        DefaultToWalkingCheck();
    }
    
    private void DefaultToWalkingCheck() { // all worms need to change enum
        if (owner._worms.Count <= 1) {
            if (turn == PlayerTurn.ChooseWorm) {
                turn = PlayerTurn.Walk;
                InputManager.Instance.EnableMovement();
            }
        }
    }

    public void SetMoveVector(Vector2 moveVector) {
        _horizontalInput = moveVector.x;
        _verticalInput = moveVector.y;
    }

    public void EnterAction() {
        switch (turn) {
            case PlayerTurn.ChooseWorm:
                InputManager.Instance.EnableMovement();
                turn = PlayerTurn.Walk;
                break;
            case PlayerTurn.Walk:
                InputManager.Instance.DisableMovement();
                _cameraManager.SwapCameraMode();
                turn = PlayerTurn.Shoot;
                break;
            case PlayerTurn.Shoot:
                GameManager.Instance.NextPlayer();
                turn = PlayerTurn.ChooseWorm;
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
            default:
                break;
        }
    }

    private void HandleMovement() {
        _moveDir = _cameraObject.forward * _verticalInput;
        _moveDir = _moveDir + _cameraObject.right * _horizontalInput;
        _moveDir.Normalize();
        _moveDir.y = 0; // clumsy solution. 
        _moveDir = _moveDir * _movementSpeed;
        //Debug.Log(_moveDir);
        Vector3 movementVelocity = _moveDir;
        _rb.velocity = movementVelocity;
    }

    public void HandleAllMovement() {
        HandleMovement();
        HandleRotation();
    }

    private void FixedUpdate() {
        GroundCheck();
        if (!(owner == GameManager.Instance.CurrentPlayer) || _cameraManager.CameraIsPanning ||
            _cameraManager.lookMode == LookMode.FirstPerson) {
            StopTransform();
            return;
        }

        if (!IsGrounded) return;

        HandleAllMovement();
    }

    private void LateUpdate() {
        if (!(owner == GameManager.Instance.CurrentPlayer) || _cameraManager.CameraIsPanning) return;

        _cameraManager.RotateCamera();
        _cameraManager.FollowTarget(owner._currentWorm.transform);
        _cameraManager.HandleCameraCollision();
    }

    private void StopTransform() {
        _rb.velocity = Vector3.zero;
    }

    public void HandleRotation() {
        if (_cameraManager.lookMode == LookMode.ThirdPerson) {
            targetDirection = Vector3.zero;
            targetDirection = _cameraObject.forward * _verticalInput;
            targetDirection = targetDirection + _cameraObject.right * _horizontalInput;
            targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero) {
                targetDirection = transform.forward;
            }

            Quaternion targetRot = Quaternion.LookRotation(targetDirection);
            Quaternion playerRot = Quaternion.Slerp(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);

            transform.rotation = playerRot;
        }
        else {
            owner._currentWorm.transform.rotation = 
                Quaternion.Euler( new Vector3(owner._currentWorm.transform.rotation.x,
                _cameraManager.transform.rotation.y,
                owner._currentWorm.transform.rotation.z)); // this isn't working needs to rotate.
        }
    }

    private void Update() {
        if (_moveDir != Vector3.zero) {
            if (walkTimer > 0) {
                walkTimer -= Time.deltaTime;
            }
            else {
                turn = PlayerTurn.Shoot;
                InputManager.Instance.DisableMovement();
                StopTransform();
                walkTimer = 5;
            }
        }

        if (startJumping) {
            jumpPower += Time.deltaTime * 10f;
            jumpPower = Mathf.Clamp(jumpPower, 1, 10);
        }
    }
    
    private void GroundCheck() {
        // do this better.
        if (!IsGrounded) {
            _inAirTimer = +Time.deltaTime;
            _rb.AddForce(-Vector3.up * (_fallingSpeed * _inAirTimer), ForceMode.Acceleration);
        }

        hitDetect = Physics.Raycast(_collider.bounds.center, Vector3.down, out hit, _groundCheckRange, groundLayer);

        if (hitDetect) this.IsGrounded = true;
        else this.IsGrounded = false;
    }

    public void SetJumpValue() {
        if (!IsGrounded || _cameraManager.lookMode == LookMode.FirstPerson) {
            jumpPower = 0;
            return;
        }

        startJumping = !startJumping;
        if (!startJumping) {
            CharacterJump();
            jumpPower = 0;
        }
    }

    private void CharacterJump() {
        //polish jump
        _rb.AddForce((Vector3.up * jumpPower) + Vector3.forward, ForceMode.Impulse);
    }
}