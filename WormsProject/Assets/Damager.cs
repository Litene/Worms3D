using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour {
    private int _damage;
    public void SetDamage(int damage) {
        this._damage = damage;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<Worm>() is IDamageable) {
            (other as IDamageable).Health.Damage(other as IDamageable,_damage);
        }
    }
}
