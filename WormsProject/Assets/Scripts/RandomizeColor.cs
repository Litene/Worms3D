using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class RandomizeColor : MonoBehaviour {
    [FormerlySerializedAs("materials")] public Material[] Materials;
    private void Awake() {
        foreach (Transform child in transform) {
            if (child.gameObject.GetComponent<MeshRenderer>()) {
                child.gameObject.GetComponent<MeshRenderer>().material = Materials[Random.Range(0, Materials.Length)];
            }
        }
    }

 
}
