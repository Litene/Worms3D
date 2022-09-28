using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagerObject : MonoBehaviour {
    private RectMask2D _rectMask;
    private const int MaxSoftness = 30000;
    [SerializeField] private List<GameObject> _objectsToDisable;
    [SerializeField] private Sprite _numberOne;
    [SerializeField] private Sprite _numberTwo;
    [SerializeField] private Sprite _numberThree;
    [SerializeField] private Sprite _numberFour;
    private GameSettings _settings;
    private Sprite[] _playerNumbers;
    private Sprite[] _unitNumbers;
    private Image _playerImage;
    private Image _unitImage;
    
    private int _currentPlayerIndex;
    private int _currentUnitIndex;

    public void SetPlayerImage(int direction) {
        if (direction > 0) {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _playerNumbers.Length;
        }
        else {
            _currentPlayerIndex--;
            if (_currentPlayerIndex < 0) {
                _currentPlayerIndex = 2;
            }
        }
        
        _playerImage.sprite = _playerNumbers[_currentPlayerIndex];
    }

    public void SetUnitImage(int direction) {
        if (direction > 0) {
            _currentUnitIndex = (_currentUnitIndex + 1) % _playerNumbers.Length;
        }
        else {
            _currentUnitIndex--;
            if (_currentUnitIndex < 0) {
                _currentUnitIndex = 3;
            }
        }
        
        _unitImage.sprite = _unitNumbers[_currentUnitIndex];
    }

    private void Awake() {
        _rectMask = FindObjectOfType<RectMask2D>();
        _unitImage = GameObject.Find("UnitTargetImage").GetComponent<Image>();
        _playerImage = GameObject.Find("PlayerTargetImage").GetComponent<Image>();
        _settings = FindObjectOfType<GameSettings>();
    }

    private void Start() {
        _playerNumbers = new Sprite[3] { _numberTwo, _numberThree, _numberFour };
        _unitNumbers = new Sprite[4] { _numberOne, _numberTwo, _numberThree, _numberFour };
        _unitImage.sprite = _unitNumbers[_currentUnitIndex];
        _playerImage.sprite = _playerNumbers[_currentPlayerIndex];
    }

    public void StartTheGame() {
        StartCoroutine(FadeOut());
        //SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
        // change this to Next UI, Canvasgroup with fade. 
    }

    public void BeginBattle() {
        _settings.SetAmountOfWorms(_currentPlayerIndex + 2, _currentUnitIndex + 1);
        SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
    }


    public void ExitTheGame() {
        Application.Quit();
    }

    private IEnumerator FadeOut() {
        foreach (var deactivateObject in _objectsToDisable) {
            if (deactivateObject.GetComponent<Image>()) {
                deactivateObject.GetComponent<Image>().raycastTarget = false;
                foreach (Transform child in deactivateObject.transform) {
                    if (child.GetComponent<Image>()) {
                        child.GetComponent<Image>().raycastTarget = false;
                    }
                }
            }
        }
        while (_rectMask.softness.x < MaxSoftness && _rectMask.softness.y < MaxSoftness) {
            _rectMask.softness += new Vector2Int(10, 10); // frame dependant
            yield return new WaitForEndOfFrame();
        }
    }
}