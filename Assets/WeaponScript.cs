using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WeaponScript : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public AudioSource audioSource;
    public AudioClip shoot;
    public float fireRate = 0.1f;   // velocidad de disparo
    public float firePower = 20f;
    public ParticleSystem muzzleFlash;
    
    private float nextFireTime = 0f;
    private bool isFiring = false;

    private XRGrabInteractable _interactable;

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
        if (!isFiring) return;

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

    void StopFiring(DeactivateEventArgs args)
    {
        isFiring = false;
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        audioSource.PlayOneShot(shoot);
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash.Play();
        }
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = firePoint.forward * firePower;
    }
}