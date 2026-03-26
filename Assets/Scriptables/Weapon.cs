using UnityEngine;

namespace Scriptables
{
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
        
        //getters setters
        public string Model { get => model; set => model = value; }
        public int Damage { get => damage; set => damage = value; }
        public float FireRate { get => fireRate; set => fireRate = value; }
        public float ReloadTime { get => reloadTime; set => reloadTime = value; }
        public int MaxAmmo { get => maxAmmo; set => maxAmmo = value; }
        public int MagCapacity { get => magCapacity; set => magCapacity = value; }
        public float FirePower { get => firePower; set => firePower = value; }
        public Bullet Bullet { get => bullet; set => bullet = value; }
        public AudioClip FireSound { get => fireSound; set => fireSound = value; }
    }
}
