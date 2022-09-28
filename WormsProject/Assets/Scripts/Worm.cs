using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.Pool;

public class Worm : MonoBehaviour, IDamageable {
    [SerializeField, Range(0.1f, 10f)] private float _movementSpeed = 3f;
    public Weapon _currentWeapon;
    private Transform _weaponMuscle;
    private int _currentWeaponIndex;
    [SerializeField] private int _ammo;
    public List<Weapon> AllWeapons;
    public GameObject pBullet;
    private ObjectPool<GameObject> _pool;
    private float _bulletUpTime = 5;
    private Collider _collider;
    [SerializeField] public Player _owner;
    public int index;
    public PlayerController _controller;
    private bool _chargingWeapon;
    private bool _shooting;

    private void Awake() {
        _controller = GetComponent<PlayerController>();
        _weaponMuscle = transform.Find("WeaponMuscle");
        AllWeapons = Resources.LoadAll<Weapon>("WeaponSO").ToList();
        _collider = GetComponent<Collider>();
        _pool = new ObjectPool<GameObject>(
            () => { return Instantiate(pBullet); },
            shootObject => { shootObject.gameObject.SetActive(true); },
            shootObject => { shootObject.gameObject.SetActive(false); },
            shootObject => { Destroy(shootObject.gameObject); },
            false, 30, 100
        );
    }

    private void Start() {
        UIManager.Instance.ActivateMiddleTextImage(_controller.turn);
        Health = new HealthSystem(100);
        //ChangeCurrentWeapon(true);
    }

    public Player GetOwner() {
        return _owner;
    }

    public void ChangeCurrentWeapon(bool initialCall) {
        if (initialCall) {
            _currentWeapon = AllWeapons[_currentWeaponIndex];
            ResetWeapon();
            return;
        }

        _currentWeaponIndex = (_currentWeaponIndex + 1) % AllWeapons.Count;
        _currentWeapon = AllWeapons[_currentWeaponIndex];
        ResetWeapon();
    }

    public void ResetWeapon() {
        
        // this needs to be called on correct worm, not all worms.
        if (_currentWeapon != null) {
            //_currentWeapon.PProjectile.GetComponent<Damager>().SetDamage(_currentWeapon.Damage);
            _ammo = _currentWeapon.MaxAmmo;
            _currentWeapon.InitializeWeapon();
        }

        _chargingWeapon = false;
    }

    public void ShootCurrentWeapon(float pressValue) {
        // now this is suscribed on button up, this needs to happen on button down for machine gun. 

        _shooting = pressValue > 0.1f;
        if (!(_controller.turn == PlayerTurn.Shoot)) {
            return;
        }

        

        // var poolObject = _pool.Get();
        // Physics.IgnoreCollision(poolObject.GetComponent<Collider>(),
        //     _collider); // cache this
        // _currentWeapon.Shoot(_weaponMuscle, ref _ammo, poolObject,
        //     _controller._cameraManager.GetCurrentEulerRotation());
        // StartCoroutine(ClearPoolObject(_bulletUpTime, poolObject));
        
            // _currentWeapon.Shoot(_weaponMuscle, ref _ammo, _pool,
            //     _controller._cameraManager.GetCurrentEulerRotation() /*this is trash*/, this, _shootPower);
        
    }

    public void OnRelease() {
        if (!(_currentWeapon is Sniper)) {
            return;
        }
        _currentWeapon.Shoot(_weaponMuscle, ref _ammo, _pool,
            _controller._cameraManager.GetCurrentEulerRotation() /*this is trash*/, this, true, true);
    }

   

    private IEnumerator ClearPoolObject(float bulletUpTime, GameObject poolObject) {
        // rename if you can't implement through queue.
        yield return new WaitForSeconds(bulletUpTime);
        _pool.Release(poolObject);
    }

    private void Timers() {
        // remove?
        //shootTimer += Time.deltaTime;
    }

    private void Update() {
        if (!(_controller.turn == PlayerTurn.Shoot))
            return;

        _currentWeapon.Shoot(_weaponMuscle, ref _ammo, _pool,
            _controller._cameraManager.GetCurrentEulerRotation() /*this is trash*/, this, _shooting, false);
       
        // if (_shooting) {
        //     // if ((_currentWeapon is Sniper)) { // move logic to weapon?
        //     //     _shootPower += Time.deltaTime * 10f;
        //     //     _shootPower = Mathf.Clamp(_shootPower, (_currentWeapon as Sniper).MinimumShootPower,
        //     //         (_currentWeapon as Sniper).MaximumShootPower); // this is a messy call
        //     // }
        //     if(_canShoot) {
        //     }
        // }
        // else if (!_shooting && !_canShoot && (_currentWeapon is Sniper)) {
        //     _currentWeapon.Shoot(_weaponMuscle, ref _ammo, _pool,
        //         _controller._cameraManager.GetCurrentEulerRotation() /*this is trash*/, this, _shooting, _canShoot);
        //     _canShoot = true;
        // }

        // if (_chargingWeapon) {
        //     if ((_currentWeapon is Sniper)) {
        //         _shootPower += Time.deltaTime * 10f;
        //         _shootPower = Mathf.Clamp(_shootPower, (_currentWeapon as Sniper).MinimumShootPower,
        //             (_currentWeapon as Sniper).MaximumShootPower); // this is a messy call
        //     }
        // }
        //Timers(); // this doesn't work, need fucntionality directly in weapon. 
        // Debug.Log(shootTimer);
        // if (_shooting && !hasShot) {
        //     if (_currentWeapon.CoolDown < shootTimer) {
        //         Physics.IgnoreCollision(poolObject.GetComponent<Collider>(),
        //             _collider); // cache this
        //         _currentWeapon.Shoot(_weaponMuscle, ref _ammo, _pool,
        //             _controller._cameraManager.GetCurrentEulerRotation(), this, _shooting);
        //         StartCoroutine(ClearPoolObject(_bulletUpTime, poolObject));
        //         shootTimer = 0;
        //         hasShot = true;
        //     }
        // }
    }


    private PlayerColor _color;
    private float _shootPower;

    public PlayerColor GetColor() {
        return _color;
    }

    public void ActivateWorm() {
        InputManager.Instance.SetCurrentController(_controller);
        ChangeCurrentWeapon(true);
    }


    public HealthSystem Health { get; set; }

    public GameObject ReturnGameObject() {
        return gameObject;
    }

    public void Die(Worm worm) {
        Debug.Log(worm._owner);
        worm._owner._worms.Remove(worm);
        if (_owner._currentWorm == this) {
            GameManager.Instance.NextPlayer();
        }

        GameManager.Instance.RemoveDeadPlayers();
        Destroy(worm.gameObject);
    }
}