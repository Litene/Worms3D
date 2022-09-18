using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    
    public Player owner;
    private void Awake() {
        //_rb = GetComponent<Rigidbody>();
    }
    public void Move(Vector3 moveVector) { // fail, needs to happen in update, or fixedupdate
        transform.position = transform.position + moveVector;
        //_moveDir = moveDir;
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
