
using System;
using UnityEngine;

public class InputManager : MonoBehaviour { // why is this a singleton?
    private PlayerControls _controls;
    private Vector2 _movementInput;
    [SerializeField] private PlayerController _currentController;
    private Vector2 _cameraInput;
    [SerializeField] private OrbitCamera _camManager;

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

    private void OnEnable() {
        _controls ??= new PlayerControls();
        _camManager = FindObjectOfType<OrbitCamera>();
        _controls.PlayerMovement.Movement.performed += val => _currentController.SetMoveVector(val.ReadValue<Vector2>());
        _controls.PlayerSequence.EnterAction.performed += val => _currentController.EnterAction();
        _controls.SpaceAction.Space.performed += val => _currentController.SpaceAction();
        _controls.PlayerMovement.Shoot.performed += val => _currentController.Owner.CurrentWorm.ShootCurrentWeapon(val.ReadValue<float>()); 
        _controls.PlayerMovement.OnMouseRelease.canceled += val => _currentController.Owner.CurrentWorm.OnRelease();
        _controls.PlayerMovement.Camera.performed += val => _camManager.ManualRotateCamera(val.ReadValue<Vector2>());
        _controls.PlayerMovement.Jump.performed += val => _currentController.CharacterJump(val.ReadValue<float>());
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
    
}