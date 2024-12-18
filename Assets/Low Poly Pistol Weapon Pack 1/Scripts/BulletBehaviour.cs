using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public int bulletDamage = 25;

    private void OnTriggerEnter(Collider other)
    {
        print("Touche par balle : " + other.name);
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(bulletDamage);
            }
        }
        
        // Destroy(gameObject);
    }
}
