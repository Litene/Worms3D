using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public enum PlayerColor {
    Red = 0,
    Yellow = 1,
    Purple = 2,
    Blue = 3
}

public class GameManager : MonoBehaviour {
    public int amountOfPlayers;
    [SerializeField] private int amountOfWorms;
    private Vector3 SpawnOffset;
    public Nest[] Nests { get; private set; }
    [SerializeField] private List<Player> _players;
    private Player _currentPlayer;

    public Player CurrentPlayer { // wait wat?
        get { return _currentPlayer; }
        set {
            value.NextWorm(true);
            _currentPlayer = value;
        }
    }
    [FormerlySerializedAs("_currentPlayerIntager")] [SerializeField] private int _currentPlayerIndex;
    private TMP_Text _playerText;
    private List<Vector3> _spawnPoints;
    private GameSettings _settings;
    private void Awake() {
        Nests = FindObjectsOfType<Nest>();
    }
    private void Start() {
        _settings = FindObjectOfType<GameSettings>();
        if (!_settings) return;

        amountOfPlayers = _settings.amountOfPlayers;
        amountOfWorms = _settings.amountOfWorms;
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
    public void NextPlayer(InputAction.CallbackContext context) {
        if (context.performed) {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
            CurrentPlayer = _players[_currentPlayerIndex];
            foreach (var worm in CurrentPlayer._worms) {
                worm.DeactivateWorm();
            }
        }
    }

    public void NextWorm(InputAction.CallbackContext context) {
        if (context.performed) {
            CurrentPlayer.NextWorm(false);
        }
    }
    private void Update() {
        //This is to be removed
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log(Utility.GetCorrectPrefab(PlayerColor.Red).name);
            //NextPlayer();
        }
    }
    private static void GenerateSingleton() {
        GameObject gameManagerObject = new GameObject("GameManager");
        DontDestroyOnLoad(gameManagerObject);
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

    private void GenerateWorms() {
        foreach (var nest in Nests) {
            for (int i = 0; i < amountOfWorms; i++) {
                if (nest.Owner.prefab == null) continue;
                var spawnPoint = nest.GetRandomSpawnPoint();
                var worm = Instantiate(nest.Owner.prefab, spawnPoint.position + SpawnOffset, spawnPoint.rotation, Utility.GetCorrectSpawnParent(nest.Owner.color));
                worm.GetComponent<PlayerController>().owner = nest.Owner; //cache somehow?
            }
        }
    }
    public void StartGame() {
        InitializePlayers();
        AssignPlayerToNest();
        GenerateWorms();
        foreach (var player in _players) {
            player.SetWorms(amountOfWorms);
        }

        CurrentPlayer = _players[_currentPlayerIndex];
    }

    private void InitializePlayers() {
        for (int i = 0; i < amountOfPlayers; i++) {
            _players.Add(new Player((PlayerColor)i, amountOfWorms));
        }
    }
    
}