using System;
using System.Collections;
using Scriptables;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Random = UnityEngine.Random;

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

    [Header("Recoil Settings")]
    [SerializeField] private Transform weaponModel; // 🔥 IMPORTANTE (hijo)
    [SerializeField] private float recoilBackAmount = 0.02f;
    [SerializeField] private float recoilUpAmount = 5f;
    [SerializeField] private float recoilRecoverySpeed = 8f;

    [Header("Testing")]
    [SerializeField] private bool enableTesting = true;

    private Vector3 modelOriginalPos;
    private Quaternion modelOriginalRot;

    private Vector3 recoilOffset;
    private Vector3 recoilRotation;

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
        fireRate = Mathf.Max(weapon.FireRate, 0.05f); // evita locuras
        firePower = weapon.FirePower;
        ammo = weapon.MagCapacity;

        if (weaponModel != null)
        {
            modelOriginalPos = weaponModel.localPosition;
            modelOriginalRot = weaponModel.localRotation;
        }

        if (muzzleFlash != null)
            muzzleFlash.Stop();
    }

    private void OnEnable()
    {
        if (_interactable != null)
        {
            _interactable.activated.AddListener(StartFiring);
            _interactable.deactivated.AddListener(StopFiring);
        }
    }

    private void OnDisable()
    {
        if (_interactable != null)
        {
            _interactable.activated.RemoveListener(StartFiring);
            _interactable.deactivated.RemoveListener(StopFiring);
        }
    }

    private void Update()
    {
        if (weapon == null) return;

        if (!isReloading)
        {
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

       
        HandleTestInput();
    }

    // 🎮 TEST SIN VR
    private void HandleTestInput()
    {
        if (!enableTesting) return;

        if (Input.GetMouseButtonDown(0))
            StartFiring(null);

        if (Input.GetMouseButtonUp(0))
            StopFiring(null);

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isReloading)
                StartCoroutine(Reload());
        }
    }

    public void StartFiring(ActivateEventArgs args)
    {
        if (weapon == null || isReloading) return;

        switch (weapon.FireType)
        {
            case FireType.SemiAuto:
            case FireType.Shotgun:
                TryShoot();
                break;

            case FireType.FullAuto:
                isFiring = true;
                break;

            case FireType.Burst:
                TryStartBurst();
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
        // vacío (ya controlado por coroutine)
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

        // 🔊 AUDIO
        if (audioSource != null && shoot != null)
        {
            float pitch = Random.Range(weapon.PitchRange.x, weapon.PitchRange.y);
            float volume = Random.Range(weapon.VolumeRange.x, weapon.VolumeRange.y);

            if (weapon.FireType == FireType.FullAuto)
                pitch += Random.Range(-0.05f, 0.05f);

            audioSource.pitch = pitch;
            audioSource.volume = volume;

            audioSource.PlayOneShot(shoot);
        }

        // 🔥 VFX
        if (muzzleFlash != null)
            muzzleFlash.Emit(1);

        // 🔫 DISPARO
        if (weapon.FireType == FireType.Shotgun)
            FireShotgun();
        else
            FireSingleBullet(firePoint.forward, firePoint.rotation);

        // 🔁 RECOIL
        ApplyRecoil();

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

    // 🔫 RECOIL (solo visual en hijo)
    private void ApplyRecoil()
    {
        recoilOffset += new Vector3(0, 0, -recoilBackAmount);
        recoilRotation += new Vector3(-recoilUpAmount, Random.Range(-1f, 1f), 0);
    }

    private void LateUpdate()
    {
        HandleRecoil();
    }

    private void HandleRecoil()
    {
        if (weaponModel == null) return;

        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);
        recoilRotation = Vector3.Lerp(recoilRotation, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);

        weaponModel.localPosition = modelOriginalPos + recoilOffset;
        weaponModel.localRotation = modelOriginalRot * Quaternion.Euler(recoilRotation);
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