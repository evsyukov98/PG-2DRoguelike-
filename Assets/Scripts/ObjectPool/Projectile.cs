using System.Collections;
using System.Collections.Generic;
using RogueLike2D;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolable
{
    private Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float force)
    {
        _rigidbody2D.AddForce(direction * force);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.TryGetComponent<Enemy>(out var enemyController))
        {
            enemyController.Damaged(1);
            OnDespawn();
        }
        else
        {
            OnDespawn();
        }
    }
    public void OnSpawn()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        gameObject.SetActive(true);
    }

    public void OnDespawn()
    {
        gameObject.SetActive(false);
    }
}
