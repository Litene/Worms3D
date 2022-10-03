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

   public virtual GameObject Shoot(Transform muscle, ref int currentAmmo, ObjectPool<GameObject> _pool,
       Worm worm, bool shooting, bool buttonUp, OrbitCamera cam) {
      return new GameObject();
   }

   public virtual void InitializeWeapon() {
      
   }


  
}

