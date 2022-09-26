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
    SelectWeapon,
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
    private bool _canWalk;

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

    public void InitializePlayerTurn() {
        // fill with logic for the player?
        DefaultToWalkingCheck();

        _canWalk = true;
    }

    private void DefaultToWalkingCheck() {
        // this is run too early, latestart could solve it but its an uggly solution.
        if (owner._worms.Count <= 1) {
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
        _horizontalInput = moveVector.x;
        _verticalInput = moveVector.y;
    }

    public void EnterAction() {
        switch (turn) {
            case PlayerTurn.ChooseWorm:
                InputManager.Instance.EnableMovement();
                turn = PlayerTurn.Walk;
                _canWalk = true;
                UIManager.Instance.ActivateMiddleTextImage(turn);
                break;
            case PlayerTurn.Walk:
                InputManager.Instance.DisableMovement();
                _cameraManager.SwapCameraMode();
                turn = PlayerTurn.SelectWeapon;
                UIManager.Instance.ActivateMiddleTextImage(turn);
                break;
            case PlayerTurn.SelectWeapon:
                UIManager.Instance.ToggleAim(turn);
                turn = PlayerTurn.Shoot;
                UIManager.Instance.ActivateMiddleTextImage(turn);
                break;
            case PlayerTurn.Shoot:
                GameManager.Instance.NextPlayer();
                turn = PlayerTurn.ChooseWorm;
                UIManager.Instance.ToggleAim(turn);
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
                owner._currentWorm.ChangeCurrentWeapon(false);
                break;
            case PlayerTurn.Shoot:
                //owner._currentWorm.ShootCurrentWeapon(); // needs to be passthrough... Toggle zoom? 
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
        Vector3 movementVelocity = _moveDir;
        _rb.velocity = movementVelocity;
    }

    public void HandleAllMovement() {
        HandleMovement();
        HandleRotation();
    }


    private void FixedUpdate() {
        GroundCheck();
        if (!(owner == GameManager.Instance.CurrentPlayer) || _cameraManager.CameraIsPanning) {
            StopTransform();
            return;
        }

        if (_cameraManager.lookMode == LookMode.FirstPerson || !_canWalk) {
            HandleRotation();
            return;
        }

        if (!IsGrounded) return;

        HandleAllMovement();
    }

    private void LateUpdate() {
        if (!(owner == GameManager.Instance.CurrentPlayer) || _cameraManager.CameraIsPanning) return;

        if (owner != null && owner._currentWorm != null) {
            _cameraManager.RotateCamera();
            _cameraManager.FollowTarget(owner._currentWorm.transform);
            _cameraManager.HandleCameraCollision();
        }
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
            // WATTAFAKKA
            transform.localEulerAngles = new Vector3(transform.localRotation.x,
                _cameraManager.GetCurrentEulerRotation().y,
                transform.localRotation.z); // this isn't working needs to rotate.
        }
    }

    private void Update() {
        if (turn == PlayerTurn.Walk && _moveDir != Vector3.zero && _canWalk) {
            if (walkTimer > 0) {
                walkTimer -= Time.deltaTime;
            }
            else if (turn == PlayerTurn.Walk) {
                // this cant be done enterACtion, this is called once in a while, every fifth second needs a gaurd cloud for starting timer. 
                InputManager.Instance.DisableMovement();
                EnterAction();
                _canWalk = false;
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