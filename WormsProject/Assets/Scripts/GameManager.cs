using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using UnityEngine.UI;


public enum PlayerColor {Red = 0, Yellow = 1, Purple = 2, Blue = 3}
public class GameManager : MonoBehaviour {
    public int amountOfPlayers;
    [SerializeField] private int amountOfWorms;
    private Nest[] _nests;
    //private Slider _slider;
    [SerializeField] private List<Player> _players;
    // public List<Player> GetPlayers() {
    //     return _players;
    // }
    [SerializeField] private Player _currentPlayer;
    [SerializeField] private int _currentPlayerIntager;
    private TMP_Text _playerText;
    private List<Vector3> _spawnPoints;
    private GameSettings settings;
    private void Awake() {
        _nests = FindObjectsOfType<Nest>();
        //_slider = FindObjectOfType<Slider>();
    }

    private void Start() {
        settings = FindObjectOfType<GameSettings>();
        if (!settings) return;
        
        amountOfPlayers = settings.amountOfPlayers ;
        amountOfWorms = settings.amountOfWorms;
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

    public void NextPlayer() { // Add context for playaerinput later
        _currentPlayerIntager = (_currentPlayerIntager + 1) % _players.Count;
       _currentPlayer = _players[_currentPlayerIntager];
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log(_players[0].GetWorms()[0].GetColor());
            NextPlayer();
            
        }
    }

    private static void GenerateSingleton() {
        GameObject gameManagerObject = new GameObject("GameManager");
        DontDestroyOnLoad(gameManagerObject);
        _instance = gameManagerObject.AddComponent<GameManager>();
    }
    

    private void GenerateWorms() {
        _spawnPoints = GetSpawnPoints(amountOfPlayers, amountOfWorms);
        for (int i = 0; i < amountOfPlayers; i++) {
            for (int j = 0; j < amountOfWorms; j++) {
                
            }
        }
    }

    private List<Vector3> GetSpawnPoints(int amountOfPlayer, int amountOfWorms) { // maybe not supposed to be random. this is total chaos, not what we want. 
        
        throw new NotImplementedException();
    }

    public void StartGame() {
        //var nests = InitializeNests();
        InitializePlayers();
        
    }

    private void InitializePlayers() { // messy.. 
        //var randomAssignment = new List<int>();
        // for (var i = 0; i < amountOfPlayers; i++) {
        //     var rand = Random.Range(0, _nests.Length);
        //     if (!randomAssignment.Contains(rand)) {
        //         randomAssignment.Add(rand);  
        //     }
        //     else {
        //         i--;
        //     }
        // }
        
        for (int i = 0; i < amountOfPlayers; i++) {
            _players.Add(new Player((PlayerColor)i, _nests[i], amountOfWorms));
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos() {
        // if (spawnPoints != null) {
        //     foreach (var VARIABLE in spawnPoints) {
        //         Gizmos.DrawWireSphere(VARIABLE, 1.0f);
        //     }
        // }
    }
#endif
}