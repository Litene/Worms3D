using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Nest : MonoBehaviour {
    [SerializeField] private PlayerColor _colorOfNest;
    [FormerlySerializedAs("spawnPoint")] public List<Transform> SpawnPoint = new List<Transform>();
    public Player Owner;
    private Nest _intance;
    public PlayerColor GetNestColor() {
        return _colorOfNest;
    }
    public Nest SetOwner(Player owner) {
       this.Owner = owner;
       _colorOfNest = owner.Color;
       return _intance;
    }

    private void Awake() {
        _intance = this;
        for (int i = 0; i < SpawnPoint.Count; i++) {
            SpawnPoint[i] = transform.GetChild(i);
        }
    }

    public Transform GetRandomSpawnPoint() {
        Transform point = null;
        point = SpawnPoint[Random.Range(0, SpawnPoint.Count)];
        SpawnPoint.Remove(point);
        return point;
    }

}


