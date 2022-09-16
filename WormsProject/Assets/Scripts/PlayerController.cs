using System;
using UnityEngine;
public class PlayerController : MonoBehaviour {
    private Rigidbody _rb;
    [SerializeField][Range(0.1f, 10f)]private float _movementSpeed;
    //private Vector3 _moveDir = Vector3.zero;
    public Player owner;
    private void Awake() {
        //_rb = GetComponent<Rigidbody>();
    }
    private void Move(Vector3 moveDir) {
        //_moveDir = moveDir;
    }

    private void FixedUpdate() {
       // _rb.MovePosition(_moveDir * _movementSpeed);
    }


    //public void NextWorm
}
