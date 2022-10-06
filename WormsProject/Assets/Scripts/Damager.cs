using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Damager : MonoBehaviour {
    private int _damage;
    private const float ExplosionMass = 2;
    private ObjectPool<GameObject> _pool;
    private GameObject _poolObject;
    public bool Explosive;
    [SerializeField] private ParticleSystem _effect;

    private Vector3 _impactPoint;
    public void SetDamage(int damage) {
        this._damage = damage;
    }

    public void SetReferences(GameObject bullet, ObjectPool<GameObject> pool) {
        _pool = pool;
        _poolObject = bullet;
    }
    private void OnCollisionEnter(Collision collision) {

        Worm hitObject = collision.gameObject.GetComponent<Worm>();
        if (hitObject != null) {
            hitObject.Health.Damage(hitObject, _damage);
            _pool.Release(_poolObject);
        }
        else if (_poolObject != null && _poolObject.GetComponent<Damager>().Explosive) {
            DamageWormsInProximity(transform.position);
            SpawnParticle(transform.position);
            _pool.Release(_poolObject);
        }
    }
    
    private void SpawnParticle(Vector3 pos) {
        var explosion = Instantiate(_effect, pos, Quaternion.identity);
        explosion.Play();
    }

    private void DamageWormsInProximity(Vector3 impactPoint) {
        var worms = FindObjectsOfType<Worm>();
        List<Tuple<Worm, int>> wormsToTakeDamage = new List<Tuple<Worm, int>>();
        foreach (var worm in worms) {
            var distanceFromImpact = Vector3.Distance(worm.transform.position, impactPoint);
            int damage = GetPressureAtPoint(distanceFromImpact);
            if (damage > 0) {
                wormsToTakeDamage.Add(new Tuple<Worm, int>(worm, damage));
            }
        }

        foreach (var wormAndDamage in wormsToTakeDamage) {
            wormAndDamage.Item1.Health.Damage(wormAndDamage.Item1, wormAndDamage.Item2);
        }
    }
    private int GetPressureAtPoint(float distanceFromImpact) { 
        var a = (0.95f / distanceFromImpact) * MathF.Pow(ExplosionMass, 0.33f);
        var b = (3.9f / MathF.Pow(distanceFromImpact, 2)) * MathF.Pow(ExplosionMass, 0.33f);
        var c = (13 / MathF.Pow(distanceFromImpact, 3)) * ExplosionMass;
        var atm = a + b + c;
        return (int)atm;
    }
    
}