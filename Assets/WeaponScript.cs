using System.Collections;
using Scriptables;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WeaponScript : MonoBehaviour
{
    private XRGrabInteractable _interactable;

    [SerializeField] private Weapon weapon;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;

    [Header("Fire Settings")]
    [SerializeField] private Transform firePoint;

    [Header("VFX Settings")]
    [SerializeField] private ParticleSystem muzzleFlash;

    private AudioClip shoot;
    private float fireRate;
    private float firePower;
    private int ammo;

    private float nextFireTime;
    private bool isFiring;
    private bool isReloading;
    private bool burstInProgress;

    private void Awake()
    {
        _interactable = GetComponent<XRGrabInteractable>();
    }

    private void Start()
    {
        if (weapon == null) return;

        shoot = weapon.FireSound;
        fireRate = weapon.FireRate;
        firePower = weapon.FirePower;
        ammo = weapon.MagCapacity;

        if (muzzleFlash != null)
            muzzleFlash.Stop();
    }

    private void OnEnable()
    {
        _interactable.activated.AddListener(StartFiring);
        _interactable.deactivated.AddListener(StopFiring);
    }

    private void OnDisable()
    {
        _interactable.activated.RemoveListener(StartFiring);
        _interactable.deactivated.RemoveListener(StopFiring);
    }

    private void Update()
    {
        if (isReloading || weapon == null) return;

        switch (weapon.FireType)
        {
            case FireType.FullAuto:
                HandleFullAuto();
                break;

            case FireType.Burst:
                HandleBurst();
                break;
        }
    }

    private void StartFiring(ActivateEventArgs args)
    {
        if (weapon == null || isReloading) return;

        switch (weapon.FireType)
        {
            case FireType.SemiAuto:
                TryShoot();
                break;

            case FireType.FullAuto:
                isFiring = true;
                break;

            case FireType.Burst:
                TryStartBurst();
                break;

            case FireType.Shotgun:
                TryShoot();
                break;
        }
    }

    private void StopFiring(DeactivateEventArgs args)
    {
        isFiring = false;
    }

    private void HandleFullAuto()
    {
        if (!isFiring) return;

        if (Time.time >= nextFireTime)
        {
            TryShoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void HandleBurst()
    {
        if (!isFiring) return;
    }

    private void TryStartBurst()
    {
        if (burstInProgress) return;
        StartCoroutine(BurstRoutine());
    }

    private IEnumerator BurstRoutine()
    {
        burstInProgress = true;
        isFiring = true;

        for (int i = 0; i < weapon.BurstCount; i++)
        {
            if (isReloading || ammo <= 0) break;

            TryShoot();
            yield return new WaitForSeconds(fireRate);
        }

        isFiring = false;
        burstInProgress = false;
    }

    private void TryShoot()
    {
        if (ammo <= 0)
        {
            if (!isReloading)
                StartCoroutine(Reload());
            return;
        }

        ammo--;

        if (audioSource != null && shoot != null)
            audioSource.PlayOneShot(shoot);

        if (muzzleFlash != null)
            muzzleFlash.Emit(1);

        switch (weapon.FireType)
        {
            case FireType.Shotgun:
                FireShotgun();
                break;

            default:
                FireSingleBullet(firePoint.forward, firePoint.rotation);
                break;
        }

        if (ammo <= 0 && !isReloading)
            StartCoroutine(Reload());
    }

    private void FireSingleBullet(Vector3 direction, Quaternion rotation)
    {
        GameObject bulletGO = Instantiate(weapon.Bullet.BulletPrefab, firePoint.position, rotation);
        BulletScript bulletScript = bulletGO.GetComponent<BulletScript>();

        bulletScript.Init(weapon.Bullet, 2.5f);
        bulletScript.Launch(direction.normalized, firePower);
    }

    private void FireShotgun()
    {
        int pellets = Mathf.Max(1, weapon.PelletsPerShot);

        for (int i = 0; i < pellets; i++)
        {
            Vector3 spreadDir = GetSpreadDirection(firePoint.forward, weapon.Spread);
            Quaternion rot = Quaternion.LookRotation(spreadDir);

            FireSingleBullet(spreadDir, rot);
        }
    }

    private Vector3 GetSpreadDirection(Vector3 forward, float spread)
    {
        Vector3 randomSpread = new Vector3(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            Random.Range(-spread, spread)
        );

        return (forward + randomSpread).normalized;
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        isFiring = false;

        yield return new WaitForSeconds(weapon.ReloadTime);

        ammo = weapon.MagCapacity;
        isReloading = false;
    }
}