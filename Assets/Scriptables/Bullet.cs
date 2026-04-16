using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "Scriptable Objects/Bullet")]
public class Bullet : ScriptableObject
{
    [SerializeField] private int damage;
    
    [SerializeField] private GameObject prefab;
    
    public int Damage { get => damage; set => damage = value; }
    
    public GameObject BulletPrefab { get => prefab; set => prefab = value; }
    
}
