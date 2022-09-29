using System;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;


public enum LookMode {
    ThirdPerson,
    FirstPerson
};

public class CameraManager : MonoBehaviour {
    private Vector3 _cameraFollowVel = Vector3.zero;
    public bool CameraIsPanning;

    private float _smoothTime = 0.2f;
    private float _cameraLerpTime = 10f;

    //private Transform _currentTarget;
    private Vector3 _thirdPositionOffset = new Vector3(0f, 2f, -6f);
    private Quaternion _thirdRotation;
    private Vector3 _firstPositionOffset = new Vector3(0f, 0.6f, 0.25f);
    [SerializeField] private Transform _firstPersonView;
    [SerializeField] private Transform _thirdPersonView;
    public Transform _currentView;
    public LookMode lookMode;

    public float LookAngle;
    public float pivotAngle;
    private float _mouseXInput;
    private float _mouseYInput;
    private float _cameraLookSpeed = 0.1f;
    private float _minimumPivot = -35f;
    private float _maximumPivot = 45f;
    private float _defaultPos;
    private Vector3 rotation;
    private Quaternion targetRotation;
    private float _cameraCollisionRadius = 1f;
    public LayerMask CollisionLayers;
    private float _cameraCollisionOffset = 0.2f;
    private float _minimumColisionOffset = 0.2f;
    private Vector3 _cameraVectorPos;
    private Transform _currentTarget;
    private Vector3 _thirdPersonPanValue = new Vector3(15f, 0f, 0f);
    private Vector3 _firstPersonPanValue = Vector3.zero; // jumps to much

    private Transform _cameraTransform;
    //public Quaternion _currentRotation;//private Transform _mainCamTransform;

    private void Awake() {
        _firstPersonView = transform.Find("FirstPersonPivot");
        _thirdPersonView = transform.Find("ThirdPersonPivot");
        _cameraTransform = Camera.main.transform;
        _currentView = _cameraTransform.parent;
        _defaultPos = _cameraTransform.localPosition.z;
    }

    private void Start() {
        _thirdPersonView.position = _thirdPositionOffset;
        _firstPersonView.position = _firstPositionOffset;
        _thirdRotation = _thirdPersonView.rotation;
        _cameraTransform.localEulerAngles = _thirdPersonPanValue;
        // _currentRotation = transform.rotation * _firstPersonView.rotation;
    }

    public Vector3 GetCurrentEulerRotation() {
        return new Vector3(_firstPersonView.localEulerAngles.x, transform.localEulerAngles.y, 0);
    }

    public void SwapCameraMode() {
        // glitchy solution, lerp towards all values here.. good for now. 
        pivotAngle = 0;
        if (_cameraTransform.parent == _firstPersonView) {
            _cameraTransform.parent = _thirdPersonView;
            _currentView = _thirdPersonView;
            _cameraTransform.localEulerAngles = _thirdPersonPanValue;
            _thirdPersonView.localEulerAngles = Vector3.zero;
            lookMode = LookMode.ThirdPerson;
        }
        else {
            _cameraTransform.parent = _firstPersonView;
            _currentView = _firstPersonView;
            _cameraTransform.localEulerAngles = _firstPersonPanValue;
            _firstPersonView.localEulerAngles = Vector3.zero;
            lookMode = LookMode.FirstPerson;
        }

        //transform.rotation = _thirdRotation;

        if (!CameraIsPanning) {
            StartCoroutine(CameraPan());
        }
    }

    public void ResetCamera() {
        _currentView = _thirdPersonView;
        _cameraTransform.parent = _thirdPersonView;
        lookMode = LookMode.ThirdPerson;

        _cameraTransform.localEulerAngles = _thirdPersonPanValue;
        pivotAngle = 0;
        if (!CameraIsPanning) {
            StartCoroutine(CameraPan());
        }
    }

    // ResetRotation() {
    //     
    // }

    public void FollowTarget(Transform currentTarget) {
        if (currentTarget == null || CameraIsPanning) return;

        if (_currentTarget != currentTarget) {
            _currentTarget = currentTarget;
        }

        Vector3 targetPos = Vector3.zero;

        if (_currentView == _firstPersonView) {
            targetPos = currentTarget.position;
        }
        else {
            targetPos = Vector3.SmoothDamp(transform.position, currentTarget.position, ref _cameraFollowVel,
                _smoothTime);
        }

        transform.position = targetPos;
    }

    public void SetRotationCamera(Vector2 input) {
        _mouseYInput = input.y;
        _mouseXInput = input.x;
    }

    public void RotateCamera() {
        if (CameraIsPanning) return;
        if (lookMode == LookMode.FirstPerson) {
            //_currentView.rotation = transform.rotation;
            pivotAngle = pivotAngle - (_mouseYInput * _cameraLookSpeed);
            pivotAngle = Mathf.Clamp(pivotAngle, _minimumPivot, _maximumPivot);
            rotation = Vector3.zero;
            rotation.x = pivotAngle;
            targetRotation = Quaternion.Euler(rotation);
            _currentView.localRotation = targetRotation;
        }
        else {
            transform.LookAt(_currentTarget);
            //_currentView.Rotate(new Vector3(-30, transform.rotation.y, transform.rotation.z));
            //_firstPersonView.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        LookAngle = LookAngle + (_mouseXInput * _cameraLookSpeed);
        rotation = Vector3.zero;
        rotation.y = LookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;
    }

    private IEnumerator CameraPan() {
        CameraIsPanning = true;
        while (Mathf.Abs((_cameraTransform.position - _currentView.position).magnitude) >= 0.1f) {
            _cameraTransform.position =
                Vector3.Lerp(_cameraTransform.position, _currentView.position, _cameraLerpTime * Time.deltaTime);
            //_currentView.LookAt(_currentTarget);
            yield return new WaitForEndOfFrame();
        }

        CameraIsPanning = false;
    }

    
    
    public void HandleCameraCollision() {
        if (lookMode == LookMode.FirstPerson) return;

        // RaycastHit hit;
        // float targetPos = _defaultPos;
        //
        // if (_currentTarget != null) {
        //     Vector3 direction = _cameraTransform.position - _currentTarget.position;
        //     direction.Normalize();
        //
        //     Debug.DrawRay(_currentTarget.position, -direction, Color.red, 10f);
        //     if (Physics.SphereCast(_currentTarget.position, _cameraCollisionOffset, direction, out hit,
        //             Vector3.Magnitude(_cameraTransform.position - _currentTarget.position))) {
        //         // var cordinate = Vector3.Project(hit.point - _currentTarget.position, direction) +
        //         //                 _currentTarget.position;
        //         //transform.position += transform.forward * _cameraCollisionOffset;
        //         transform.Translate(-direction);
        //         //transform.position = Vector3.Lerp(transform.position, transform.forward * _cameraCollisionOffset, 0.2f);
        //         return true;
        //         // float distance = Vector3.Distance(hit.point, _currentTarget.position);
        //         // targetPos = +(distance - _cameraCollisionOffset);
        //
        //     }
        // }
        //
        // return false;
        // if (Mathf.Abs(targetPos) < _minimumColisionOffset) {
        //     targetPos = targetPos - _minimumColisionOffset;
        // }
        //
        // _cameraVectorPos.z = Mathf.Lerp(_cameraTransform.localPosition.z, targetPos, 0.2f);
        // _cameraTransform.localPosition = _cameraVectorPos;
    }
}