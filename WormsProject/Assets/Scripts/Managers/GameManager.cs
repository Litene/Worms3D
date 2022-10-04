using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum PlayerColor {
    Red = 0,
    Yellow = 1,
    Purple = 2,
    Blue = 3
}

public class GameManager : MonoBehaviour {
    public int AmountOfPlayers;
    [SerializeField] private int _amountOfWorms;
    private Vector3 SpawnOffset;
    public Nest[] Nests { get; private set; }
    [SerializeField] private List<Player> _players;
    

    public List<Player> GetPlayers() {
        return _players;
    }
    private Player _currentPlayer;
    //private CameraManager _camManager;
    private OrbitCamera _orbitCamera;
    public bool GameOver = false;

    public Player CurrentPlayer {
        // wait wat?
        get { return _currentPlayer; }
        set {
            UIManager.Instance.SetCurrentPlayer(value.color);
            _currentPlayer = value;
        }
    }

    [SerializeField] private int _currentPlayerIndex;
    private TMP_Text _playerText;
    private List<Vector3> _spawnPoints;
    private GameSettings _settings;

    private void Awake() {
        Nests = FindObjectsOfType<Nest>();
        _orbitCamera = FindObjectOfType<OrbitCamera>();
    }

    private void Start() {
        _settings = FindObjectOfType<GameSettings>();
        if (!_settings) return;

        AmountOfPlayers = _settings.amountOfPlayers;
        _amountOfWorms = _settings.amountOfWorms;
        SpawnOffset = new Vector3(0, 0.75f, 0);
        StartGame();
    }

    private static GameManager _instance;

    public static GameManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null) {
                    GenerateSingleton();
                }
            }

            return _instance;
        }
    }

    public void NextPlayer() {
        // listen to key new system
        _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
        CurrentPlayer = _players[_currentPlayerIndex];
        StartCoroutine(InitializeWorms());
        
        _orbitCamera.ToggleCameraMode(false);
    }

    private IEnumerator InitializeWorms() {
        yield return new WaitForEndOfFrame();
        foreach (var worm in CurrentPlayer._worms) {
            worm._controller.InitializePlayerTurn();
        }

        CurrentPlayer.NextWorm(true);
    }

    public void NextWorm() {
        // listen to key
        CurrentPlayer.NextWorm(false);
    }

    private static void GenerateSingleton() {
        GameObject gameManagerObject = new GameObject("GameManager");
        gameManagerObject.transform.parent = GameObject.Find("Managers").transform;
        _instance = gameManagerObject.AddComponent<GameManager>();
    }

    public void AssignPlayerToNest() {
        List<Nest> RandomizedList = Nests.OrderBy(player => Random.Range(0, Nests.Length)).ToList();
        for (int i = 0; i < _players.Count; i++) {
            RandomizedList[i].SetOwner(_players[i]);
        }

        for (int i = 0; i < Nests.Length; i++) {
            if (Nests[i].Owner == null) {
                Nests[i].enabled = false;
                Nests[i] = null;
            }
        }
    }

    public void RemoveDeadPlayers() {
        List<Player> playerToRemove = new List<Player>();
        foreach (var player in _players) {
            if (player._worms.Count <= 0) {
                playerToRemove.Add(player);
            }
        }
        foreach (var player in playerToRemove) {
            _players.Remove(player);
        }
        WinCondition();
    }

    public void WinCondition() {
        if (_players.Count == 1) {
            GameOver = true;
            UIManager.Instance.ActivateEndscreen(_players[0].color);
        }
        else if (_players.Count == 0) {
            GameOver = true;
            // even all died
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            UIManager.Instance.ActivateEndscreen(PlayerColor.Red);
        }
    }

    private void GenerateWorms() {
        foreach (var nest in Nests) {
            for (int i = 0; i < _amountOfWorms; i++) {
                if (nest.Owner.prefab == null) continue;
                var spawnPoint = nest.GetRandomSpawnPoint();
                var worm = Instantiate(nest.Owner.prefab, spawnPoint.position + SpawnOffset,
                    nest.Owner.prefab.transform.rotation, Utility.GetCorrectSpawnParent(nest.Owner.color));
                worm.GetComponent<PlayerController>().Owner = nest.Owner; //cache somehow?
                worm.GetComponent<Worm>()._owner = nest.Owner;
            }
        }
    }

    public void StartGame() {
        InitializePlayers();
        AssignPlayerToNest();
        GenerateWorms();
        foreach (var player in _players) {
            player.SetWorms(_amountOfWorms);
        }

        CurrentPlayer = _players[_currentPlayerIndex];
        StartCoroutine(InitializeWorms());
        UIManager.Instance.SetupHealthBars(_players.Count);
    }

    private void InitializePlayers() {
        for (int i = 0; i < AmountOfPlayers; i++) {
            _players.Add(new Player((PlayerColor)i, _amountOfWorms));
        }
    }

    public void QuitGame() {
        Application.Quit();
    }
}