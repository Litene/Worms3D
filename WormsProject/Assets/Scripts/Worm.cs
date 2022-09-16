using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Worm : MonoBehaviour {
    private Player _owner;
    public int index; // not used.
    private PlayerController _controller;
    private Camera cam;
    private void Awake() {
        _controller = GetComponent<PlayerController>();
        cam = Camera.main;
    }

    public Player GetOwner() {
        return _owner;
    }

    private PlayerColor _color;
    public PlayerColor GetColor() {
        return _color;
    }
    public void ActivateWorm() {
        _controller.enabled = true;
        cam.GetComponent<CamController>().SetTarget(this.gameObject.transform);
    }
    
    public void DeactivateWorm() {
        _controller.enabled = false;
    }
    
}