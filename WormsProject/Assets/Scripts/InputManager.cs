using System;
using UnityEngine;

public class InputManager : MonoBehaviour {
    private PlayerControls _controls;
    private Vector2 _movementInput;
    private PlayerController _currentController;


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

    

    private void OnEnable() { // disable/pause _controls at the right time _controls 
        _controls ??= new PlayerControls();
        _controls.PlayerMovement.Movement.performed += val => _currentController.SetMoveVector(val.ReadValue<Vector2>());
        _controls.NextTurn.NextPlayer.performed += val => GameManager.Instance.NextPlayer();
        _controls.NextWorm.NextWorm.performed += val => GameManager.Instance.NextWorm();
        _controls.Enable();
    }

    private void OnDisable() {
        _controls.Disable();
    }
    
    //send to correct player . correct worm,
}