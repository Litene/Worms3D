using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public class CollisionHandler {
    public LayerMask CollisionLayer;
    public bool colliding = false;
    public Vector3[] AdjustedCameraClipPoints;
    public Vector3[] DesiredCameraClipPoints;

    public Camera _camera;

    public void Initialize(Camera cam) {
        _camera = cam;
        AdjustedCameraClipPoints = new Vector3[5];
        DesiredCameraClipPoints = new Vector3[5];
        
    }

    public bool CollisionDetectionAtClipPoints(Vector3[] clipPoints, Vector3 fromPos) {
        
        return true;
    }

    public void UpdateCameraClipPoints(Vector3 camPos, Quaternion atRotation, ref Vector3[] intoArray) {
        
    }

    public float GetAdjustedDistanceWithRayFrom(Vector3 from) {
        return 0;
    }

    public void CheckColliding(Vector3 targetPos) {
        
    }
    
    

}
