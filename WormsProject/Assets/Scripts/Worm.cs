using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Player _owner;
    public int index;
    public PlayerController _controller;
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
        ChangeCurrentWeapon(true);
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
        if (_currentWeapon != null) {
            _ammo = _currentWeapon.MaxAmmo;
        }

        _shooting = false;
    }

    public void ShootCurrentWeapon() {
        if (_controller.turn == PlayerTurn.Shoot) {
            _shooting = !_shooting;
            
            
            // var poolObject = _pool.Get();
            // Physics.IgnoreCollision(poolObject.GetComponent<Collider>(),
            //     _collider); // cache this
            // _currentWeapon.Shoot(_weaponMuscle, ref _ammo, poolObject,
            //     _controller._cameraManager.GetCurrentEulerRotation());
            // StartCoroutine(ClearPoolObject(_bulletUpTime, poolObject));
        }
    }


    private IEnumerator ClearPoolObject(float bulletUpTime, GameObject poolObject) {
        // rename if you can't implement through queue.
        yield return new WaitForSeconds(bulletUpTime);
        _pool.Release(poolObject);
    }

    private void Timers() { // remove?
        //shootTimer += Time.deltaTime;
    }

    private void Update() {
        if (!(_controller.turn == PlayerTurn.Shoot)) 
            return;
        
        Debug.Log(_shooting);
        
        _currentWeapon.Shoot(_weaponMuscle,ref _ammo, _pool, _controller._cameraManager.GetCurrentEulerRotation() /*this is trash*/, this, _shooting);
        
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

    public PlayerColor GetColor() {
        return _color;
    }

    public void ActivateWorm() {
        InputManager.Instance.SetCurrentController(_controller);
        ResetWeapon();
    }


    public HealthSystem Health { get; set; }

    public GameObject ReturnGameObject() {
        return gameObject;
    }

    public void Die() {
        _owner._worms.Remove(this);
        if (_owner._currentWorm == this) {
            GameManager.Instance.NextPlayer();
        }

        Destroy(gameObject); // maybe a method in player that removes and destroys it. 
    }
}