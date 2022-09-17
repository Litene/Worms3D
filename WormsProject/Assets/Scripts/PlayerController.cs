using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    private Rigidbody _rb;
    [SerializeField][Range(0.1f, 10f)]private float _movementSpeed;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _moveDir = Vector3.zero;
    private float _smoothTime = 0.3f;
    public Player owner;
    private void Awake() {
        //_rb = GetComponent<Rigidbody>();
        _movementSpeed = 2f;
    }
    public void Move(InputAction.CallbackContext context) {
        _moveDir = context.ReadValue<Vector2>();
        //_moveDir = moveDir;
    }

    private void FixedUpdate() {
        var step = (transform.position + new Vector3(_moveDir.x, 0, _moveDir.y)) * _movementSpeed;
        Debug.Log(step);
         //transform.Translate(Vector3.Lerp(transform.position, step, Time.fixedDeltaTime));
        //     Vector3.SmoothDamp(transform.position, step, ref _velocity, _smoothTime);
        //transform.Translate(new Vector3(-_moveDir.y, 0, _moveDir.x) * (_movementSpeed * Time.fixedDeltaTime));
    }


    //public void NextWorm
}
