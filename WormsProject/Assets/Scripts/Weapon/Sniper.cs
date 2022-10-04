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

    //private const float coolDown = 3;

    //public float shootTimer;
    public override void InitializeWeapon() {
        ShootOnRelease = true;
        ShootTimer = 3; 
        CoolDown = 3;
    }

    public override GameObject Shoot(Transform muscle, ref int currentAmmo, ObjectPool<GameObject> pool,
        Worm worm, bool shooting, bool ButtonUp, OrbitCamera cam) {
        if (currentAmmo <= 0) {
            return null;
        }

        ShootTimer += Time.deltaTime;

        ShootTimer = Mathf.Clamp(ShootTimer, 0, CoolDown);
        if (shooting) {
            ChargePower(shooting);
        }
        else {
            return null;
        }

        if (ShootOnRelease && ShootTimer >= CoolDown && ButtonUp) {
            currentAmmo--;
            var poolObject = pool.Get();
            Physics.IgnoreCollision(poolObject.GetComponent<Collider>(),
                worm.GetComponent<Collider>());
            poolObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            poolObject.transform.position = muscle.transform.position;
            Vector3 dir = cam.transform.rotation * Vector3.forward;
            dir.Normalize();
            //Vector3 dir = Quaternion.Euler(shootRotation).eulerAngles /** Vector3.forward*/;
            poolObject.GetComponent<Rigidbody>().AddForce(dir * power, ForceMode.Impulse);
            poolObject.GetComponent<Damager>().SetDamage(Damage);
            ShootTimer = 0;
            power = MinimumShootPower;
            
            return poolObject;
        }

        return null;
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