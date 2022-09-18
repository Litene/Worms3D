using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.InputSystem;


public class CamController : MonoBehaviour {
    private Transform _currentTarget;
    private Vector3 _offset = Vector3.zero;

    private Vector3 Yoffset = new Vector3(0, 3, 0);
    //private Vector3 _rotation = new Vector3(30, -90, 0);
    [Range(0.1f, 2)] [SerializeField] private float _smoothness;
    private Vector3 _cameraPos;
    private PlayerInput _playerInput;
    private InputAction _playerCamMovement;

    private void Awake() {
        _playerInput = GetComponent<PlayerInput>();
        _playerCamMovement = _playerInput.actions["Look"];
        _smoothness = 1f;
    }

    private void FixedUpdate() {
        if (_currentTarget == null) return;
        
        //_cameraPos = _currentTarget.localPosition + _offset;
        //transform.LookAt(_currentTarget);
        //transform.localPosition = Vector3.Lerp(transform.position, _cameraPos, _smoothness * Time.fixedTime);
        transform.position = new Vector3(_playerCamMovement.ReadValue<Vector2>().x, _playerCamMovement.ReadValue<Vector2>().y, 0) + transform.position * Time.fixedDeltaTime;
    }
    public void SetTarget(Transform target) {
        _currentTarget = target;
        _offset = (target.forward * -3) + Yoffset;
        transform.position = _currentTarget.position + _offset;
        transform.LookAt(_currentTarget);
    }
    
}
