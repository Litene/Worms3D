using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(menuName = "Weapon/MachineGun")]
public class MachineGun : Weapon {
    private float TimeBetweenBullets = 0.1f;
    private float shootPower = 30;
    private float bulletUpTime;
    private float _shootTimer;
    private Vector3 bulletSpread;
    
    public override GameObject Shoot(Transform muscle, ref int currentAmmo, ObjectPool<GameObject> pool,  Worm worm, bool shooting, bool buttonUp, OrbitCamera cam) {
        if (currentAmmo <= 0) {
            return null;
        }
        
        if (!shooting) {
            _shootTimer = 0;
            return null;
        }

        _shootTimer += Time.deltaTime;
        
        
        if (_shootTimer > TimeBetweenBullets) {
            currentAmmo--;
            bulletSpread = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f));
            var poolObject = pool.Get();
            Physics.IgnoreCollision(poolObject.GetComponent<Collider>(),
                worm.GetComponent<Collider>());
            poolObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            poolObject.transform.position = muscle.transform.position;
            Vector3 dir = cam.transform.rotation * Vector3.forward;
            dir.Normalize();
            poolObject.GetComponent<Rigidbody>().AddForce((dir + bulletSpread) * shootPower, ForceMode.Impulse);
            poolObject.GetComponent<Damager>().SetDamage(Damage);
            _shootTimer = 0;
            return poolObject;
        }

        return null;
    }
    
 
}
