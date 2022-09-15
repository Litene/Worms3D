using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Player {
    public PlayerColor color { get; private set; }
    public Nest SpawnSlot { get; private set; } 
    private List<Worm> _worms = new List<Worm>();
    public List<Worm> GetWorms() {
        return _worms;
    }
    public Player(PlayerColor color, Nest spawnSlot, int wormAmount) {
        this.color = color;
        this.SpawnSlot = spawnSlot;
        SetWorms(wormAmount);
    }
    public void SetWorms(int amount) {
        for (int i = 0; i < amount; i++) {
            _worms.Add(new Worm(this));
        }
    }
}
