using System;
using UnityEngine;
using System.Collections;



public class CameraManager : MonoBehaviour {
    private Vector3 _cameraFollowVel = Vector3.zero;
    private float _smoothTime = 0.2f;
    private Transform _currentTarget;

    private Transform CurrentTarget { // both are calling currentTarget; this needs to pass worm and not player. 
        get { return _currentTarget; }
        set {
            if (_currentTarget != value) {
                StartCoroutine(CameraPan(value));
            }
            _currentTarget = value;
        }
    }
    
    public void FollowTarget(Transform _currentTarget) {
        if (_currentTarget == null) return;
        
        Vector3 targetPos = Vector3.SmoothDamp(transform.position, _currentTarget.position, ref _cameraFollowVel, _smoothTime);

        transform.position = targetPos;
    }

    private IEnumerator CameraPan(Transform target) {
        transform.position = target.position;
        yield return null;
    }


}

