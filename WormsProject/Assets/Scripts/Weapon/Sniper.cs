using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(menuName = "Weapon/Sniper")]
public class Sniper : Weapon {
    //private float shootPower;
    public float MaximumShootPower = 100; // fix these
    public float MinimumShootPower = 30; // fix these
    private const float _bulletUptime = 3;
    private float power;
    private const float coolDown = 3;
    private float shootTimer;
    public override void InitializeWeapon() {
        ShootOnRelease = true;
        shootTimer = 5;
    }
    public override void Shoot(Transform muscle, ref int currentAmmo, ObjectPool<GameObject> pool,
        Vector3 shootRotation, Worm worm, bool shooting, bool ButtonUp) {
        if (currentAmmo <= 0) {
            return;
        }

        shootTimer += Time.deltaTime;
        
        if (shooting) {
            ChargePower(shooting);
        }
        else {
            return;
        }

        if (ShootOnRelease && shootTimer > coolDown && ButtonUp) {
            currentAmmo--;
            var poolObject = pool.Get();
            Physics.IgnoreCollision(poolObject.GetComponent<Collider>(),
                worm.GetComponent<Collider>());
            poolObject.transform.position = muscle.transform.position;
            Vector3 dir = Quaternion.Euler(shootRotation) * Vector3.forward;
            poolObject.GetComponent<Rigidbody>().AddForce(dir * power, ForceMode.Impulse);
            poolObject.GetComponent<Damager>().SetDamage(Damage);
            shootTimer = 0;
            power = MinimumShootPower;
        }
    }

    private bool ChargePower(bool HoldingButton) {
        if (HoldingButton) {
            power += Time.deltaTime * 10f;
            power = Mathf.Clamp(power, MinimumShootPower,
                MaximumShootPower); // this is a messy call
        }

        return true;
    }


    //can't start coroutines from non monobehavior.. needs to call worm.clearpool or something, maybe shoot should return the projectile? and 
}