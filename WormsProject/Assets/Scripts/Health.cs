using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem {
    private int health;
    private int healthMax;

    public HealthSystem(int healthMax) {
        this.healthMax = healthMax;
        health = healthMax;
    }

    public int GetHealth() {
        return health;
    }

    public void Damage(IDamageable objectToTakeDamage, int damageAmount = 1) {
        health -= damageAmount;
        if (health < 0)
            health = 0;

        if (health == 0) // hmm this is weird.. check later with debugs. 
            Die(objectToTakeDamage);
    }
    
    private void Die(IDamageable objectToTakeDamage) {
        objectToTakeDamage.Die();
    }

    public void Heal(int healAmount) {
        health += healAmount;
        if (health > healthMax)
            health = healthMax;
    }
    
}

public interface IDamageable //interface so that objects can take damage
{
    public HealthSystem Health { get; set; }
    public GameObject ReturnGameObject();
    public void Die();
}


