using System.Collections;
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

    [Header("Players")] 
    [SerializeField] private Sprite _purplePlayer;
    [SerializeField] private Sprite _yellowPlayer;
    [SerializeField] private Sprite _bluePlayer;
    [SerializeField] private Sprite _redPlayer;
    
    
    [SerializeField] private GameObject Hud;
    [SerializeField] private Image _aim;
    [SerializeField] private TextMeshProUGUI _currentPlayer; // fix
    [SerializeField] private Transform _middleScreenTextObject; // fix
    [SerializeField] private TextMeshProUGUI _currentWeapon; // fix
    [SerializeField] private Sprite _machineGun;
    [SerializeField] private Sprite _SniperRifle;
    private const float LerpValue = 0.01f;
    private Color Visible = new Color(255, 255, 255, 255);
    private Color Invisible = new Color(255, 255, 255, 0);
    
    [Header("EndScreen")]
    [SerializeField]private Image _endScreenBackground;
    [SerializeField]private Image _targetWinner;
    [SerializeField]private Image _hasWonText;
    [SerializeField]private Image _exitButton;
    [SerializeField]private Image[] endScreen;


    private Image _timeToWalkImage;
    private Image _timeToShootImage;
    private Image _selectWormImage;
    private Image _selectWeaponImage;
    private Image _targetWeaponSpriteImage;

    private List<Image> _middleScreenImages = new List<Image>();
    private Image _currentPlayerCornerImage;

    public void SetWeaponSprite(Weapon currentWeapon) { // you could refactor to just have the weapon have the sprite, this makes it so you can just call 
        if (currentWeapon is Sniper) {
            _targetWeaponSpriteImage.color = Visible;
            _targetWeaponSpriteImage.sprite = _SniperRifle;
        }
        else if (currentWeapon is MachineGun) {
            _targetWeaponSpriteImage.color = Visible;
            _targetWeaponSpriteImage.sprite = _machineGun;
        }
        else {
            _targetWeaponSpriteImage.color = Invisible;
        }
    }
    
    
    private void Awake() {
        Hud = GameObject.Find("Hud");
        _aim = Hud.transform.Find("Aim").GetComponent<Image>();
        //_currentPlayer = Hud.transform.Find("CurrentPlayerHeader").transform.Find("CurrentPlayer").GetComponent<TextMeshProUGUI>();
        _middleScreenTextObject = Hud.transform.Find("MiddleText");
        _currentWeapon = Hud.transform.Find("CurrentWeapon").GetComponent<TextMeshProUGUI>();
        _selectWormImage = _middleScreenTextObject.Find("NextWormImage").GetComponent<Image>();
        _timeToShootImage = _middleScreenTextObject.Find("TimeToShoot").GetComponent<Image>();
        _timeToWalkImage = _middleScreenTextObject.Find("TimeToWalk").GetComponent<Image>();
        _selectWeaponImage = _middleScreenTextObject.Find("SelectWeapon").GetComponent<Image>();
        _targetWeaponSpriteImage = GameObject.Find("TargetWeaponSpriteImage").GetComponent<Image>();
        _targetWinner = GameObject.Find("TargetWinnerPlayerImage").GetComponent<Image>();
        _endScreenBackground = GameObject.Find("EndScreen").GetComponent<Image>();
        _exitButton = GameObject.Find("EndScreenExitButton").GetComponent<Image>();
        _hasWonText = GameObject.Find("HasWonImage").GetComponent<Image>();
        _currentPlayerCornerImage = GameObject.Find("CurrentPlayerCornerTargetImage").GetComponent<Image>();
    

    }

    private void Start() {
        _aim.enabled = false;
        _currentWeapon.enabled = false;
        _middleScreenImages.Add(_timeToShootImage);
        _middleScreenImages.Add(_timeToWalkImage);
        _middleScreenImages.Add(_selectWormImage);
        _middleScreenImages.Add(_selectWeaponImage);
        endScreen = new Image[4] { _targetWinner, _endScreenBackground, _exitButton, _hasWonText };
        SetWeaponSprite(null);
        foreach (Transform child in _middleScreenTextObject) {
            child.GetComponent<Image>().enabled = false;
        }

        _endScreenBackground.gameObject.SetActive(false);
        
    }

    public void SetCurrentPlayer(PlayerColor player) {
        _currentPlayerCornerImage.sprite = ReturnPlayerImage(player);
    }

    public Sprite ReturnPlayerImage(PlayerColor player) { // call from GameManager.
        switch (player) {
            case PlayerColor.Blue:
                _targetWinner.sprite = _bluePlayer;
                return _bluePlayer;
            case PlayerColor.Purple:
                _targetWinner.sprite = _purplePlayer;
                return _purplePlayer;
            case PlayerColor.Red:
                _targetWinner.sprite = _redPlayer;
                return _redPlayer;
            default:
                _targetWinner.sprite = _yellowPlayer;
                return _yellowPlayer;

        }
    }

    public void ActivateEndscreen(PlayerColor player) {
        ReturnPlayerImage(player);
        StartCoroutine(FadeInEndScreen());
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


    private IEnumerator FadeInEndScreen() {
        _endScreenBackground.gameObject.SetActive(true);
        while (_endScreenBackground.color != Visible) {
            foreach (Image image in endScreen) {
                image.color = Color.Lerp(image.color, Visible, LerpValue * Time.deltaTime); // should it be time.deltatime
            }
            yield return new WaitForEndOfFrame();
        }
    }
    
}