using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Nest : MonoBehaviour {
    [SerializeField] private PlayerColor colorOfNest;
    public List<Transform> spawnPoint = new List<Transform>();
    public Player Owner;
    private Nest _intance;
    public PlayerColor GetNestColor() {
        return colorOfNest;
    }
    public Nest SetOwner(Player owner) {
       this.Owner = owner;
       colorOfNest = owner.color;
       return _intance;
    }

    private void Awake() {
        _intance = this;
        for (int i = 0; i < spawnPoint.Count; i++) {
            spawnPoint[i] = transform.GetChild(i);
        }
    }

    public Transform GetRandomSpawnPoint() {
        Transform point = null;
        point = spawnPoint[Random.Range(0, spawnPoint.Count)];
        spawnPoint.Remove(point);
        return point;
    }

    // public Nest SetNestColor(PlayerColor color) {
    //     this.colorOfNest = color;
    //     return _intance;
    // }
}


