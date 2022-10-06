using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class Worm : MonoBehaviour, IDamageable {
    [SerializeField, Range(0.1f, 10f)] private float _movementSpeed = 3f;
    public Weapon _currentWeapon;
    private Transform _weaponMuscle;
    private int _currentWeaponIndex;
    public int Ammo;
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
    public HealthSystem Health { get; set; }
    public int MaxHealth = 100;
    private bool _airControl;
    [FormerlySerializedAs("currentMaxAmmo")] public int CurrentMaxAmmo;

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
        Health = new HealthSystem(MaxHealth);
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
        UIManager.Instance.SetWeaponSprite(_currentWeapon);
    }

    public void ResetWeapon() {
        // this needs to be called on correct worm, not all worms.
        if (_currentWeapon != null) {
            //_currentWeapon.PProjectile.GetComponent<Damager>().SetDamage(_currentWeapon.Damage);
            Ammo = _currentWeapon.MaxAmmo;
            CurrentMaxAmmo = _currentWeapon.MaxAmmo;
            _currentWeapon.InitializeWeapon();
        }

        UIManager.Instance.SetWeaponSprite(null);

        _chargingWeapon = false;
    }

    public void ShootCurrentWeapon(float pressValue) {
        _shooting = pressValue > 0.1f;
        if (!(_controller.turn == PlayerTurn.Shoot)) {
            return;
        }
    }

    public void OnRelease() {
        // 
        if (!(_currentWeapon is Sniper) || _controller.turn != PlayerTurn.Shoot) {
            return;
        }

        var poolObject = _currentWeapon.Shoot(_weaponMuscle, ref Ammo, _pool,
            this, true, true, _controller._orbitCamera);
        // UIManager.Instance.UpdateBullets(_currentWeapon.MaxAmmo, _ammo, _controller.turn);
        if (poolObject != null) {
            StartCoroutine(ClearPoolObject(2, poolObject));
        }
    }

    private IEnumerator ClearPoolObject(float bulletUpTime, GameObject poolObject) {
        yield return new WaitForSeconds(bulletUpTime);
        _pool.Release(poolObject);
    }

    private void Update() {
        if (_currentWeapon != null) {
            UIManager.Instance.UpdateBullets(CurrentMaxAmmo, Ammo, GameManager.Instance.CurrentPlayer);
            UIManager.Instance.UpdateCoolDownTimer(_currentWeapon.ShootTimer, _currentWeapon.CoolDown, GameManager.Instance.CurrentPlayer);
        }

        if (!(_controller.turn == PlayerTurn.Shoot))
            return;


        var poolObject = _currentWeapon.Shoot(_weaponMuscle, ref Ammo, _pool, this, _shooting, false,
            _controller._orbitCamera);
        if (poolObject != null) {
            StartCoroutine(ClearPoolObject(3, poolObject));
        }
    }

    private PlayerColor _color;
    private float _shootPower;

    public PlayerColor GetColor() {
        return _color;
    }

    public void ActivateWorm() {
        InputManager.Instance.SetCurrentController(_controller);
        _controller._orbitCamera.SetTarget(gameObject.transform);
        ChangeCurrentWeapon(true);
    }

    public GameObject ReturnGameObject() {
        return gameObject;
    }

    public void Die(Worm worm) {
        worm._owner._worms.Remove(worm);
        if (worm._owner._currentWorm == this) {
            _controller.EnterAction(); // this can cause issues? is in shootmode, should go back to 
            //GameManager.Instance.NextPlayer();
        }

        GameManager.Instance.RemoveDeadPlayers();
        Destroy(worm.gameObject);
    }
}