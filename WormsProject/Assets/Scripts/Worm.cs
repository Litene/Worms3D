using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class Worm : MonoBehaviour {
    
    [SerializeField, Range(0.1f, 10f)]private float _movementSpeed = 3f ;

    //[SerializeField, Range(0f, 10f)] private float test = 5f;
    //private Vector2 _smoothInputVelocity = Vector3.zero;
    //private Vector3 _moveDir = Vector3.zero;
    //private float _smoothTime = 0.3f;
    private Player _owner;
    public int index; // not used.
    public PlayerController _controller;
    //private Camera cam;
    // [SerializeField] private PlayerInput _playerInput;
    // [SerializeField] private InputAction _playerMovement;
   // private InputAction _playerJump;
    //private Vector2 _currentInputVector;
    private void Awake() {
        _controller = GetComponent<PlayerController>();
      //  _playerInput = GetComponent<PlayerInput>();
       // _playerMovement = _playerInput.actions["Move"];
        //_playerMovement = _playerInput.actions["Jump"];
       // cam = Camera.main;
    }

    private void Start() {
       // _movementSpeed = 4;
    }

    public Player GetOwner() {
        return _owner;
    }

    private void FixedUpdate() {
        // Vector2 input = _playerMovement.ReadValue<Vector2>();
        // _currentInputVector = Vector2.SmoothDamp(_currentInputVector, input, ref _smoothInputVelocity, _smoothTime);
        // _controller.Move(new Vector3(_currentInputVector.x, 0, _currentInputVector.y) * (_movementSpeed * Time.fixedDeltaTime));
    }

    private PlayerColor _color;
    public PlayerColor GetColor() {
        return _color;
    }
    public void ActivateWorm() {
        InputManager.Instance.SetCurrentController(_controller);
        //_controller._cameraManager.SetTarget(this.transform);
        // _controller.enabled = true;
        // cam.GetComponent<CamController>().SetTarget(this.gameObject.transform);
    }
    
    
}