using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(menuName = "Weapon/Sniper")]
public class Sniper : Weapon {
    //private float shootPower;
    public float MaximumShootPower = 100; 
    public float MinimumShootPower = 30;
    private float _power;

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
            var damager = poolObject.GetComponent<Damager>();
            //Vector3 dir = Quaternion.Euler(shootRotation).eulerAngles /** Vector3.forward*/;
            damager.Explosive = true;
            poolObject.GetComponent<Rigidbody>().AddForce(dir * _power, ForceMode.Impulse);
            damager.SetDamage(Damage);
            damager.SetReferences(poolObject, pool);
            ShootTimer = 0;
            _power = MinimumShootPower;
            
            return poolObject;
        }

        return null;
    }
    
    

    private bool ChargePower(bool HoldingButton) {
        if (HoldingButton) {
            _power += Time.deltaTime * 10f;
            _power = Mathf.Clamp(_power, MinimumShootPower,
                MaximumShootPower); 
        }

        return true;
    }


}