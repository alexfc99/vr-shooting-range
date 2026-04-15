using UnityEngine;

namespace Scriptables
{
    public enum FireType
    {
        SemiAuto,
        FullAuto,
        Burst,
        Shotgun
    }

    [CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
    public class Weapon : ScriptableObject
    {
        [SerializeField] private string model;
        [SerializeField] private int damage;
        [SerializeField] private float fireRate;
        [SerializeField] private float firePower;
        [SerializeField] private float reloadTime;
        [SerializeField] private int maxAmmo;
        [SerializeField] private int magCapacity;
        [SerializeField] private Bullet bullet;
        [SerializeField] private AudioClip fireSound;

        [Header("Fire Mode")]
        [SerializeField] private FireType fireType;
        [SerializeField] private int burstCount = 3;
        [SerializeField] private int pelletsPerShot = 8;
        [SerializeField, Range(0f, 1f)] private float spread = 0.08f;

        // 🔊 NUEVO
        [Header("Audio Settings")]
        [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);
        [SerializeField] private Vector2 volumeRange = new Vector2(0.9f, 1f);

        public string Model { get => model; set => model = value; }
        public int Damage { get => damage; set => damage = value; }
        public float FireRate { get => fireRate; set => fireRate = value; }
        public float ReloadTime { get => reloadTime; set => reloadTime = value; }
        public int MaxAmmo { get => maxAmmo; set => maxAmmo = value; }
        public int MagCapacity { get => magCapacity; set => magCapacity = value; }
        public float FirePower { get => firePower; set => firePower = value; }
        public Bullet Bullet { get => bullet; set => bullet = value; }
        public AudioClip FireSound { get => fireSound; set => fireSound = value; }

        public FireType FireType { get => fireType; set => fireType = value; }
        public int BurstCount { get => burstCount; set => burstCount = value; }
        public int PelletsPerShot { get => pelletsPerShot; set => pelletsPerShot = value; }
        public float Spread { get => spread; set => spread = value; }

        // 🔊 NUEVO getters
        public Vector2 PitchRange { get => pitchRange; set => pitchRange = value; }
        public Vector2 VolumeRange { get => volumeRange; set => volumeRange = value; }
    }
}