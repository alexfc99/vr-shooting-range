using System;
using System.Collections;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    
    

    private Rigidbody _rigidbody;
    private int _timer = 5;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        StartCoroutine(DestroyBullet());
    }
    

    public void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(_timer);
        
        HideBullet();
    }

    void HideBullet()
    {
        Destroy(this.gameObject);
    }
}
