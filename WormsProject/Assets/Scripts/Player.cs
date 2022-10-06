using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

// if I add AOE damage I need to check if it is the currnetworm and if it is, call next. 
[System.Serializable]
public class Player {
    public PlayerColor Color;

    //public Nest SpawnSlot; 
    public List<Worm> Worms = new List<Worm>();

    //public List<PlayerController> controllers;
    public int CurrentWormIndex;
    public Worm CurrentWorm;
    public GameObject Prefab { get; set; }

    public List<Worm> GetWorms() {
        return Worms;
    }

    public int GetTotalHealth() {
        int totalHealth = 0;

        foreach (var worm in Worms) {
            totalHealth += worm.Health.GetHealth();
        }

        return totalHealth;
    }

    public int GetMaxHealth() {
        int totalHealth = 0;
        foreach (var worm in Worms) {
            totalHealth += worm.MaxHealth;
        }

        return totalHealth;
    }

    public Player(PlayerColor color, int wormAmount) {
        this.Color = color;
        Prefab = Utility.GetCorrectPrefab(color);
    }

    public void NextWorm(bool playerSwap) {
        if (GameManager.Instance.GameOver) {
            return;
        }
        
        if (playerSwap && Worms.Count > CurrentWormIndex) {
            CurrentWorm = Worms[CurrentWormIndex];
            Worms[CurrentWormIndex].ActivateWorm();
            return;
        }
        

        CurrentWormIndex = (CurrentWormIndex + 1) % Worms.Count;
        CurrentWorm = Worms[CurrentWormIndex];
        Worms[CurrentWormIndex].ActivateWorm();
    }

    public void SetWorms(int amount) {
        for (int i = 0; i < amount; i++) {
            Worms.Add(Utility.GetCorrectsWorms(Color)[i]);
        }

        for (int i = 0; i < Worms.Count; i++) {
            Worms[i].Index = i;
        }
    }
}