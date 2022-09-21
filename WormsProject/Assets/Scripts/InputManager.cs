using System;
using UnityEngine;

public class InputManager : MonoBehaviour {
    private PlayerControls _controls;
    private Vector2 _movementInput;
    [SerializeField] private PlayerController _currentController;
    private Vector2 _cameraInput;
    [SerializeField] private CameraManager _camManager;
   


    private static InputManager _instance;
    public static InputManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<InputManager>();
                if (_instance == null) {
                    GenerateSingleton();
                }
            }
            return _instance;
        }
    }
    

    private static void GenerateSingleton() {
        GameObject inputManagerObject = new GameObject("InputManager");
        DontDestroyOnLoad(inputManagerObject);
        _instance = inputManagerObject.AddComponent<InputManager>();
    }
    public void SetCurrentController(PlayerController controller) {
        _currentController = controller;
    }
     // subscribe a action to it, and change depending
    

    private void OnEnable() { // disable/pause _controls at the right time _controls 
        _controls ??= new PlayerControls();
        _camManager = FindObjectOfType<CameraManager>();
        _controls.PlayerMovement.Movement.performed += val => _currentController.SetMoveVector(val.ReadValue<Vector2>());
        _controls.PlayerSequence.EnterAction.performed += val => _currentController.EnterAction();
        _controls.SpaceAction.Space.performed += val => _currentController.SpaceAction();
        //_controls.NextTurn.NextPlayer.performed += val => GameManager.Instance.NextPlayer();
        //_controls.NextWorm.NextWorm.performed += val => GameManager.Instance.NextWorm();
        //_controls.SwapCameraMode.SwapCameraMode.performed += val => _currentController._cameraManager.SwapCameraMode();
        _controls.PlayerMovement.Camera.performed += val => _camManager.SetRotationCamera(val.ReadValue<Vector2>());
        _controls.PlayerMovement.Jump.performed += val => _currentController.SetJumpValue();
        _controls.Enable();

    }

    public void EnableMovement() {
        _controls.PlayerMovement.Movement.Enable();
        _controls.PlayerMovement.Jump.Enable();
    }

    public void DisableMovement() {
        _controls.PlayerMovement.Movement.Disable();
        _controls.PlayerMovement.Jump.Disable();
    }

    private void OnDisable() {
        _controls.Disable();
    }
    
    //send to correct player . correct worm,
}