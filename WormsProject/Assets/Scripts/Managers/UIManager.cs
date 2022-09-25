using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    
    private static UIManager _instance;
    public static UIManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<UIManager>();
                if (_instance == null) {
                    GenerateSingleton();
                }
            }
            return _instance;
        }
    }
    
    
    
    [SerializeField] private GameObject Hud;
    [SerializeField] private Image _aim;
    [SerializeField] private TextMeshProUGUI _currentPlayer; // fix
    [SerializeField] private Transform _middleScreenTextObject; // fix
    [SerializeField] private TextMeshProUGUI _currentWeapon; // fix

    private Image _timeToWalkImage;
    private Image _timeToShootImage;
    private Image _selectWormImage;
    private Image _selectWeaponImage;

    private List<Image> _middleScreenImages = new List<Image>();
    
    
    private void Awake() {
        Hud = GameObject.Find("Hud");
        _aim = Hud.transform.Find("Aim").GetComponent<Image>();
        _currentPlayer = Hud.transform.Find("CurrentPlayerHeader").transform.Find("CurrentPlayer").GetComponent<TextMeshProUGUI>();
        _middleScreenTextObject = Hud.transform.Find("MiddleText");
        _currentWeapon = Hud.transform.Find("CurrentWeapon").GetComponent<TextMeshProUGUI>();
        _selectWormImage = _middleScreenTextObject.Find("NextWormImage").GetComponent<Image>();
        _timeToShootImage = _middleScreenTextObject.Find("TimeToShoot").GetComponent<Image>();
        _timeToWalkImage = _middleScreenTextObject.Find("TimeToWalk").GetComponent<Image>();
        _selectWeaponImage = _middleScreenTextObject.Find("SelectWeapon").GetComponent<Image>();
    }

    private void Start() {
        _aim.enabled = false;
        _currentWeapon.enabled = false;
        _middleScreenImages.Add(_timeToShootImage);
        _middleScreenImages.Add(_timeToWalkImage);
        _middleScreenImages.Add(_selectWormImage);
        _middleScreenImages.Add(_selectWeaponImage);
        foreach (Transform child in _middleScreenTextObject) {
            child.GetComponent<Image>().enabled = false;
        }
        
    }

    public void ActivateMiddleTextImage(PlayerTurn turn) {
        foreach (Image image in _middleScreenImages) {
            image.enabled = false;
        }
        switch (turn) { // set in a coroutine to only have it on screen for a little while. 
            case PlayerTurn.ChooseWorm:
                _selectWormImage.enabled = true;
                break;
            case PlayerTurn.Shoot:
                _timeToShootImage.enabled = true;
                break;
            case PlayerTurn.EndTurn:
                break;
            case PlayerTurn.Walk:
                _timeToWalkImage.enabled = true;
                break;
            case PlayerTurn.SelectWeapon:
                _selectWeaponImage.enabled = true;
                break;
        }
        
    }

    public void ToggleAim(PlayerTurn turn) {
        if (turn == PlayerTurn.Shoot) _aim.enabled = true;
        else _aim.enabled = false;
    }
    
    private static void GenerateSingleton() {
        GameObject UIManagerObject = new GameObject("UIManager");
        UIManagerObject.transform.parent = GameObject.Find("Managers").transform;
        _instance = UIManagerObject.AddComponent<UIManager>();
    }
    
    
}