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
    [FormerlySerializedAs("_currentWeapon")] public Weapon CurrentWeapon;
    private Transform _weaponMuscle;
    private int _currentWeaponIndex;
    public int Ammo;
    public List<Weapon> AllWeapons;
    [FormerlySerializedAs("pBullet")] public GameObject PBullet;
    private ObjectPool<GameObject> _pool;
    private Collider _collider;
    [FormerlySerializedAs("_owner")] [SerializeField] public Player Owner;
    [FormerlySerializedAs("index")] public int Index;
    [FormerlySerializedAs("_controller")] public PlayerController Controller;
    private bool _chargingWeapon;
    private bool _shooting;
    public HealthSystem Health { get; set; }
    public int MaxHealth = 100;
    private bool _airControl;
    [FormerlySerializedAs("currentMaxAmmo")] public int CurrentMaxAmmo;

    private void Awake() {
        Controller = GetComponent<PlayerController>();
        _weaponMuscle = transform.Find("WeaponMuscle");
        AllWeapons = Resources.LoadAll<Weapon>("WeaponSO").ToList();
        _collider = GetComponent<Collider>();
        _pool = new ObjectPool<GameObject>(
            () => { return Instantiate(PBullet); },
            shootObject => { shootObject.gameObject.SetActive(true); },
            shootObject => { shootObject.gameObject.SetActive(false); },
            shootObject => { Destroy(shootObject.gameObject); },
            false, 30, 100
        );
    }

    private void Start() {
        UIManager.Instance.ActivateMiddleTextImage(Controller.Turn);
        Health = new HealthSystem(MaxHealth);
    }

    public Player GetOwner() {
        return Owner;
    }

    public void ChangeCurrentWeapon(bool initialCall) {
        if (initialCall) {
            CurrentWeapon = AllWeapons[_currentWeaponIndex];
            ResetWeapon();
            return;
        }

        _currentWeaponIndex = (_currentWeaponIndex + 1) % AllWeapons.Count;
        CurrentWeapon = AllWeapons[_currentWeaponIndex];
        ResetWeapon();
        UIManager.Instance.SetWeaponSprite(CurrentWeapon);
    }

    public void ResetWeapon() {
        if (CurrentWeapon != null) {
            Ammo = CurrentWeapon.MaxAmmo;
            CurrentMaxAmmo = CurrentWeapon.MaxAmmo;
            CurrentWeapon.InitializeWeapon();
        }

        UIManager.Instance.SetWeaponSprite(null);

        _chargingWeapon = false;
    }

    public void ShootCurrentWeapon(float pressValue) {
        _shooting = pressValue > 0.1f;
        if (!(Controller.Turn == PlayerTurn.Shoot)) {
            return;
        }
    }

    public void OnRelease() {
        // 
        if (!(CurrentWeapon is Sniper) || Controller.Turn != PlayerTurn.Shoot) {
            return;
        }

        var poolObject = CurrentWeapon.Shoot(_weaponMuscle, ref Ammo, _pool,
            this, true, true, Controller.OrbitCamera);
        if (poolObject != null) {
            StartCoroutine(ClearPoolObject(2, poolObject));
        }
    }

    private IEnumerator ClearPoolObject(float bulletUpTime, GameObject poolObject) {
        yield return new WaitForSeconds(bulletUpTime);
        _pool.Release(poolObject);
    }

    private void Update() {
        if (CurrentWeapon != null) {
            UIManager.Instance.UpdateBullets(GameManager.Instance.CurrentPlayer);
            UIManager.Instance.UpdateCoolDownTimer(CurrentWeapon.ShootTimer, CurrentWeapon.CoolDown, GameManager.Instance.CurrentPlayer);
        }

        if (!(Controller.Turn == PlayerTurn.Shoot))
            return;


        var poolObject = CurrentWeapon.Shoot(_weaponMuscle, ref Ammo, _pool, this, _shooting, false,
            Controller.OrbitCamera);
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
        InputManager.Instance.SetCurrentController(Controller);
        Controller.OrbitCamera.SetTarget(gameObject.transform);
        ChangeCurrentWeapon(true);
    }

    public GameObject ReturnGameObject() {
        return gameObject;
    }

    public void Die(Worm worm) {
        worm.Owner.Worms.Remove(worm);
        if (worm.Owner.CurrentWorm == this) {
            Controller.EnterAction(); 
        }

        GameManager.Instance.RemoveDeadPlayers();
        Destroy(worm.gameObject);
    }
}