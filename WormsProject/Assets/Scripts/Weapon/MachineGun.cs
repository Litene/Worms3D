using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(menuName = "Weapon/MachineGun")]
public class MachineGun : Weapon
{
    public override void Shoot(Transform muscle, ref int currentAmmo, ObjectPool<GameObject> pool, Vector3 shootRotation, Worm worm, bool ButtonUp) {
        
    }
}
