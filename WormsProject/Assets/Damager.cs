using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour {
    private int _damage;

    public void SetDamage(int damage) {
        this._damage = damage;
    }

    // private void OnTriggerEnter(Collider other) {
    //     if (other.gameObject.GetComponent<Worm>() is IDamageable) {
    //         Debug.Log("yo");
    //         (other as IDamageable).Health.Damage(other as IDamageable,_damage);
    //         
    //     }
    // }

    private void OnCollisionEnter(Collision collision) {
        Worm hitObject = collision.gameObject.GetComponent<Worm>();
        if (hitObject != null) {
            Debug.Log("yo");
            hitObject.Health.Damage(hitObject, _damage);
        }
    }
}