using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class HealthSystem {
    private int _health;
    private int _healthMax;
    
    public HealthSystem(int healthMax) {
        this._healthMax = healthMax;
        _health = healthMax;
    }

    public int GetHealth() {
        return _health;
    }

    public void Damage(Worm objectToTakeDamage, int damageAmount = 1) {
        _health -= damageAmount;
        if (_health < 0)
            _health = 0;

        if (_health == 0) 
            Die(objectToTakeDamage);
    }
    
    private void Die(Worm objectToTakeDamage) {
        objectToTakeDamage.Die(objectToTakeDamage);
    }

    public void Heal(int healAmount) {
        _health += healAmount;
        if (_health > _healthMax)
            _health = _healthMax;
    }
    
}

public interface IDamageable 
{
    public HealthSystem Health { get; set; }
    public GameObject ReturnGameObject();
    public void Die(Worm worm);
}


