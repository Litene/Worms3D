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
    [SerializeField] private int _amountOfWorms = 5;
    [SerializeField] private GameObject environment;
    private Nest[] _nests;
    private Slider _slider;
    private List<Player> _players;
    private int _currentPlayer;
    private TMP_Text _playerText;
    private List<Vector3> _spawnPoints;
    private void Awake() {
        _nests = FindObjectsOfType<Nest>();
        _slider = FindObjectOfType<Slider>();
    }

    
    private void Start() {
        amountOfPlayers = (int)_slider.value;
        
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
        _currentPlayer = (_currentPlayer + 1) % _players.Count;
    }

    private static void GenerateSingleton() {
        GameObject gameManagerObject = new GameObject("GameManager");
        DontDestroyOnLoad(gameManagerObject);
        _instance = gameManagerObject.AddComponent<GameManager>();
    }
    

    private void GenerateWorms() {
        _spawnPoints = GetSpawnPoints(amountOfPlayers, _amountOfWorms);
        for (int i = 0; i < amountOfPlayers; i++) {
            for (int j = 0; j < _amountOfWorms; j++) {
                
            }
        }
    }

    private List<Vector3> GetSpawnPoints(int amountOfPlayer, int amountOfWorms) { // maybe not supposed to be random. this is total chaos, not what we want. 
        
        throw new NotImplementedException();
    }

    public void StartGame() {
        InitializePlayers();
        InitializeNests();
    }

    private void InitializeNests() { // i'm retarded.... 
        List<int> randomAssignment = new List<int>();
        for (int i = 0; i < amountOfPlayers; i++) {
            var rand = Random.Range(0, _nests.Length);
            if (!randomAssignment.Contains(rand)) {
                 randomAssignment.Add(rand);  
            }
            else {
                i--;
            }
        }
        
    }

    private void InitializePlayers() {
        for (int i = 0; i < amountOfPlayers; i++) {
            _players.Add(new Player((PlayerColor)i));
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