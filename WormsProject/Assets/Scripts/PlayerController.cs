using System;
using UnityEngine;
using UnityEngine.InputSystem;


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
    
    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _cameraManager = FindObjectOfType<CameraManager>();
        if (Camera.main != null) _cameraObject = Camera.main.transform;
    }
    
    
    public void SetMoveVector(Vector2 moveVector) { // this should only set value;
        //transform.position = transform.position + moveVector;
        //_moveDir = cameraObject.forward * moveVector.x;
        _horizontalInput = moveVector.x;
        _verticalInput = moveVector.y;
       
        //Debug.Log(moveVector);
        //_moveDir = moveDir;
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
        
        HandleAllMovement();
    }

    private void LateUpdate() { // this probably calls on all
        if (!(owner == GameManager.Instance.CurrentPlayer)) return;

        _cameraManager.FollowTarget(transform);
    }

    public void HandleRotation() {
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

    // private void FixedUpdate() {
    //     var step = (transform.position + new Vector3(_moveDir.x, 0, _moveDir.y)) * _movementSpeed;
    //     //Debug.Log(step);
    //      //transform.Translate(Vector3.Lerp(transform.position, step, Time.fixedDeltaTime));
    //     //     Vector3.SmoothDamp(transform.position, step, ref _velocity, _smoothTime);
    //     //transform.Translate(new Vector3(-_moveDir.y, 0, _moveDir.x) * (_movementSpeed * Time.fixedDeltaTime));
    // }


    //public void NextWorm
}
