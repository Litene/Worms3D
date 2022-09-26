using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomizeColor : MonoBehaviour {
    public Material[] materials;
    // Start is called before the first frame update
    private void Awake() {
        foreach (Transform child in transform) {
            if (child.gameObject.GetComponent<MeshRenderer>()) {
                child.gameObject.GetComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Length)];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
