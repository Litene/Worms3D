using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour {
    [SerializeField] private Transform _focus;
    [SerializeField, Range(0f, 20f)] private float _distance = 5f;
    [SerializeField, Min(0f)] private float _focusRadius = 1f;
    [SerializeField, Range(0f, 1f)] private float _focusCentering = 0.5f;
    [SerializeField, Range(0, 360)] private float _rotationSpeed = 10f;
    [SerializeField, Range(-89, 89)] private float _minVerticalAngle = -30, _maxVericalAngle = 60;
    [SerializeField, Min(0f)] private float _alignDelay = 5f;
    [SerializeField, Range(0f, 90f)] private float alignSmoothRange = 45f;


    private const int MaxCameraDistance = 5, MinCameraDistance = 0;
    private float _cameraPanSmoothing = 0.3f;
    private float _currentVelocity = 0;
    private Vector3 _focusPoint, _previousFocusPoint;
    private Vector2 _orbitAngles = new Vector2(45f, 0);
    private float _lastManualRotation;
    private bool _manualRotation;
    private Transform _muscle;
    private Camera _cam;
    private Transform _player;
    [field: SerializeField] public bool CameraIsPanning { get; private set; }

    Vector3 CameraHalfExtends {
        get {
            Vector3 halfExtends;
            halfExtends.y = _cam.nearClipPlane * Mathf.Tan(0.5f * Mathf.Rad2Deg * _cam.fieldOfView);
            halfExtends.x = halfExtends.y * _cam.aspect;
            halfExtends.z = 0f;
            return halfExtends;
        }
    }

    private void OnValidate() {
        if (_maxVericalAngle < _minVerticalAngle) {
            _maxVericalAngle = _minVerticalAngle;
        }
    }

    private void ConstrainAngles() {
        _orbitAngles.x = Mathf.Clamp(_orbitAngles.x, _minVerticalAngle, _maxVericalAngle);

        if (_orbitAngles.y < 0f) {
            _orbitAngles.y += 360f;
        }
        else if (_orbitAngles.y >= 360f) {
            _orbitAngles.y -= 360f;
        }
    }

    private void Awake() {
        _cam = GetComponent<Camera>();
        transform.localRotation = Quaternion.Euler(_orbitAngles);
    }

    public void SetTarget(Transform target) {
        if (Equals(target, _focus)) {
            // not 100% sure this works
            return;
        }

        _focus = target;
        _muscle = target.Find("WeaponMuscle");
        _player = target;
        _focusPoint = _focus.position;
    }


    public Quaternion GetCurrentEulerRotation() {
        // this needs to be changed. 

        return Quaternion.LookRotation(transform.forward, transform.up);
    }

    private void LateUpdate() {
        if (_focus == null) return;

        UpdateFocusPoint();
        Quaternion lookRotation;
        if (_manualRotation || AutomaticRotation()) {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(_orbitAngles);
        }
        else {
            lookRotation = transform.localRotation;
        }

        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = _focusPoint - lookDirection * _distance;

        Vector3 rectOffset = lookDirection * _cam.nearClipPlane;
        Vector3 rectPosition = lookPosition + rectOffset;
        Vector3 castFrom = _focus.position;
        Vector3 castLine = rectPosition - castFrom;
        float castDistance = castLine.magnitude;
        Vector3 castDirection = castLine / castDistance;

        if (Physics.BoxCast(castFrom, CameraHalfExtends, castDirection, out RaycastHit hit, lookRotation,
                castDistance)) {
            rectPosition = castFrom + castDirection * hit.distance;
            lookPosition = rectPosition - rectOffset;
        }

        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    public void ManualRotateCamera(Vector2 InputSystem) {
        Vector2 input = new Vector2(-InputSystem.y, InputSystem.x);
        const float e = 0.001f;
        if (input.x < -e || input.x > e || input.y < -e || input.y > e) {
            _orbitAngles += _rotationSpeed * Time.unscaledDeltaTime * input;
            _lastManualRotation += Time.unscaledDeltaTime;
            _manualRotation = true;
            return;
        }

        _manualRotation = false;
    }

    private bool AutomaticRotation() {
        if (Time.unscaledTime - _lastManualRotation < _alignDelay) {
            return false;
        }

        Vector2 movement = new Vector2(_focusPoint.x - _previousFocusPoint.x, _focusPoint.z - _previousFocusPoint.z);
        float movementDeltaSquare = movement.sqrMagnitude;
        if (movementDeltaSquare < 0.0001f) {
            return false;
        }

        float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSquare));
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(_orbitAngles.y, headingAngle));
        float rotationChange = _rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSquare);
        if (deltaAbs < alignSmoothRange) {
            rotationChange *= deltaAbs / alignSmoothRange;
        }
        else if (180f - deltaAbs < alignSmoothRange) {
            rotationChange *= (180f - deltaAbs) / alignSmoothRange;
        }

        _orbitAngles.y = Mathf.MoveTowardsAngle(_orbitAngles.y, headingAngle, rotationChange);
        return true;
    }

    private static float GetAngle(Vector2 direction) {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }

    private void UpdateFocusPoint() {
        // happens here?
        _previousFocusPoint = _focusPoint;
        Vector3 targetPoint = _focus.position;
        if (_focusRadius > 0f) {
            float distance = Vector3.Distance(targetPoint, _focusPoint);
            float t = Mathf.Pow(1f - _focusCentering, Time.deltaTime);
            if (distance > _focusRadius) {
                t = Mathf.Min(t, _focusRadius / distance);
            }

            _focusPoint = Vector3.Lerp(targetPoint, _focusPoint, Time.unscaledDeltaTime);
        }
        else {
            _focusPoint = targetPoint;
        }
    }

    public void ToggleCameraMode(bool firstPerson) {
        if (CameraIsPanning) {
            return;
        }

        if (firstPerson) {
            _focus = _muscle;
            _focusPoint = _muscle.position;
        }
        else {
            _focus = _player;
            _focusPoint = _player.position;
        }

        StartCoroutine(LerpCamera());
    }

    private IEnumerator LerpCamera() {
        CameraIsPanning = true;
        float target = Math.Abs(_distance - MinCameraDistance) < 0.1f ? MaxCameraDistance : MinCameraDistance;

        while (Mathf.Abs(_distance - target) > 0.2f) {
            float newDistance = Mathf.SmoothDamp(_distance, target, ref _currentVelocity, _cameraPanSmoothing);
            _distance = newDistance;
            yield return new WaitForEndOfFrame();
        }

        _distance = (int)target;

        CameraIsPanning = false;
    }
}