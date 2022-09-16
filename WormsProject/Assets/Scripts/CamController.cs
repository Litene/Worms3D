using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class CamController : MonoBehaviour {
    private Transform _currentTarget;
    private Vector3 _offset = new Vector3(7, 3, 0);
    private Vector3 _rotation = new Vector3(30, -90, 0);
    [Range(0.1f, 2)] [SerializeField] private float _smoothness;
    private Vector3 _cameraPos;

    private void Awake() {
        _smoothness = 0.2f;
    }

    private void FixedUpdate() {
        if (_currentTarget == null) return;
        
        _cameraPos = _currentTarget.localPosition + _offset;
        transform.localRotation = Quaternion.Euler(_rotation);
        transform.localPosition = Vector3.Lerp(transform.localPosition, _cameraPos, _smoothness * Time.fixedTime);
    }
    public void SetTarget(Transform target) {
        _currentTarget = target;
        //transform.parent = target;
        _offset = (_currentTarget.forward * -1);
    }
    
}
