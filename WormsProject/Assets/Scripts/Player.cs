using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

// if I add AOE damage I need to check if it is the currnetworm and if it is, call next. 
[System.Serializable]
public class Player {
    public PlayerColor color;
    //public Nest SpawnSlot; 
    public List<Worm> _worms = new List<Worm>();
    
    //public List<PlayerController> controllers;
    public int _currentWormIndex;
    public Worm _currentWorm;
    public GameObject prefab { get; set; }
    public List<Worm> GetWorms() {
        return _worms;
    }

    
    public Player(PlayerColor color, int wormAmount) { // this is weird
        this.color = color;
        //this.SpawnSlot = spawnSlot;
        prefab = Utility.GetCorrectPrefab(color);
    }
    public void NextWorm(bool playerSwap) {
        if (GameManager.Instance.GameOver) {
            return;
        }
        // _worms[_currentWormIndex].DeactivateWorm();
        if (playerSwap) {
            _currentWorm = _worms[_currentWormIndex];
            _worms[_currentWormIndex].ActivateWorm();
            return;
        }
        _currentWormIndex = (_currentWormIndex + 1) % _worms.Count;
        _currentWorm = _worms[_currentWormIndex];
        _worms[_currentWormIndex].ActivateWorm();
        
    }
    public void SetWorms(int amount) {
        for (int i = 0; i < amount; i++) {
            _worms.Add(Utility.GetCorrectsWorms(color)[i]);
        }
        for (int i = 0; i < _worms.Count; i++) {
            _worms[i].index = i;
        }
    }
}
