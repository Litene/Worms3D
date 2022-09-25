using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(menuName = "Weapon/Sniper")]
public class Sniper : Weapon {
    private float shootPower;
    private const float MaximumShootPower = 30;
    private const float MinimumShootPower = 5;
    private const float _bulletUptime = 3;
    public override void Shoot(Transform muscle, ref int currentAmmo, ObjectPool<GameObject> pool, Vector3 shootRotation, Worm worm, bool buttonUp) {
        if (currentAmmo <= 0) {
            return;
        }
        // GET RECKED DOESN't WORK

        shootPower = shootPower * Time.deltaTime * 10f;
        shootPower = Mathf.Clamp(shootPower, MinimumShootPower, MaximumShootPower);

        if (!buttonUp) {
            return;
        }
        
        currentAmmo--;
        var poolObject = pool.Get();
        Physics.IgnoreCollision(poolObject.GetComponent<Collider>(),
            worm.GetComponent<Collider>());
        poolObject.transform.position = muscle.transform.position;
        Vector3 dir = Quaternion.Euler(shootRotation) * Vector3.forward;
        poolObject.GetComponent<Rigidbody>().AddForce(dir * shootPower, ForceMode.Impulse);
        //base.ClearPoolObject(_bulletUptime, poolObject, pool);
    }
    
    //can't start coroutines from non monobehavior.. needs to call worm.clearpool or something, maybe shoot should return the projectile? and 
}
