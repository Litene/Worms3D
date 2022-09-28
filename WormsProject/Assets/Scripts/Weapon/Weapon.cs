using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class Weapon : ScriptableObject {
   public int MaxAmmo;
   public float ShootSpeed;
   public GameObject PProjectile;
   public int Damage;
   public float CoolDown;
   public bool ShootOnRelease;
   public virtual void Shoot(Transform muscle, ref int currentAmmo, ObjectPool<GameObject> _pool, Vector3 shootRotation, Worm worm, bool shooting, bool buttonUp) {}

   public virtual void InitializeWeapon() {
      
   }


  
}

