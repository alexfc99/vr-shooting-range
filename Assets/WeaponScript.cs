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
    public AudioSource audioSource;
    private AudioClip shoot; 
    
    [Header("Fire Settings")]
    public Transform firePoint; 
    private float fireRate;   
    private float firePower;
    private int ammo;
    
    [Header("VFX Settings")]
    public ParticleSystem muzzleFlash; 
    
    private float nextFireTime = 0f;
    private bool isFiring = false;
    private bool isReloading = false;

    void Start()
    {
        muzzleFlash.Stop();
       
        if (weapon != null)
        {
            shoot = weapon.FireSound;
            fireRate = weapon.FireRate;
            firePower = weapon.FirePower;
            ammo = weapon.MagCapacity;
        }
        
    }
    void Awake()
    {
        _interactable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        _interactable.activated.AddListener(StartFiring);
        _interactable.deactivated.AddListener(StopFiring);
    }

    void OnDisable()
    {
        _interactable.activated.RemoveListener(StartFiring);
        _interactable.deactivated.RemoveListener(StopFiring);
    }

    void Update()
    {
        if (!isFiring || isReloading) return;

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void StartFiring(ActivateEventArgs args)
    {
        
        isFiring = true;
    }

    /*void StartFiring2()
    {
        
        isFiring = true;
    }*/
    
    
    void StopFiring(DeactivateEventArgs args)
    {
        isFiring = false;
        //muzzleFlash.Stop();
    }

    void Shoot()
    {
        if (ammo > 0)
        {
            ammo--;
            audioSource.PlayOneShot(shoot);

            if (muzzleFlash != null)
            {
                muzzleFlash.Emit(1); // emite 1 partícula instantánea
            }

            GameObject bulletGO = Instantiate(weapon.Bullet.BulletPrefab, firePoint.position, firePoint.rotation);
            BulletScript bulletScript = bulletGO.GetComponent<BulletScript>();

            
            bulletScript.Init(weapon.Bullet, 2.5f);

           
            bulletScript.Launch(firePoint.forward, firePower);
            
        }
        else
        {
            if (!isReloading)
                StartCoroutine(Reload());
        }
        
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        Debug.Log("Reload called");

        yield return new WaitForSeconds(weapon.ReloadTime);

        ammo = weapon.MagCapacity;

        isReloading = false;
    }
}