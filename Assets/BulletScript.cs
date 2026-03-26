using System;
using System.Collections;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Bullet _bullet;
    private float _lifeTime;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    public void Init(Bullet bulletData, float lifeTime)
    {
        _bullet = bulletData;
        _lifeTime = lifeTime;
    }
    public void Launch(Vector3 direction, float force)
    {
        _rigidbody.AddForce(direction * force, ForceMode.Impulse);
        StartCoroutine(DestroyBullet());
    }
    private void OnCollisionEnter(Collision collision)
    {

        Destroy(gameObject);
    }
    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(_lifeTime);
        Destroy(gameObject);
    }
}
