using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Worm {
    private Player _owner;
    public Player GetWonder() {
        return _owner;
    }

    private PlayerColor _color;
    public PlayerColor GetColor() {
        return _color;
    }
    public Worm(Player owner) {
        this._owner = owner;
        this._color = owner.color;
    }
}