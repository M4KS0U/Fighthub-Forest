using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWeapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public float bulletSpeed = 30;

    public float lifeTime = 3;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Fire();
        }
    }

    private void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab);

        Physics.IgnoreCollision(bullet.GetComponent<Collider>(),
            bulletSpawn.parent.GetComponent<Collider>());

        bullet.transform.position = bulletSpawn.position;

        Vector3 rotation = bullet.transform.rotation.eulerAngles;

        bullet.transform.rotation = Quaternion.Euler(rotation.x, transform.eulerAngles.y, rotation.z);
        
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward * bulletSpeed, ForceMode.Impulse);

        StartCoroutine(DestroyBulletAfterTime(bullet, lifeTime));
    }
    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(bullet);
    }
}   

// with animation but have to fix it

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class FireWeapon : MonoBehaviour
// {
//     public GameObject bulletPrefab;
//     public Transform bulletSpawn;
//     public float bulletSpeed = 30;
//     public float lifeTime = 3;
//     public Animator playerAnimator;  // L'Animator du joueur

//     void Start()
//     {
//         // Si nécessaire, assure-toi que l'Animator est bien assigné via l'Inspector
//     }

//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.M))
//         {
//             Fire();
//         }
//     }

//     private void Fire()
//     {
//         // Déclencher l'animation de tir
//         if (playerAnimator != null)
//         {
//             playerAnimator.SetTrigger("FirePistolTrigger");  // Utilise le Trigger que tu as configuré
//         }

//         // Tirer le projectile
//         GameObject bullet = Instantiate(bulletPrefab);

//         Physics.IgnoreCollision(bullet.GetComponent<Collider>(),
//             bulletSpawn.parent.GetComponent<Collider>());

//         bullet.transform.position = bulletSpawn.position;

//         Vector3 rotation = bullet.transform.rotation.eulerAngles;

//         bullet.transform.rotation = Quaternion.Euler(rotation.x, transform.eulerAngles.y, rotation.z);
        
//         bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward * bulletSpeed, ForceMode.Impulse);

//         StartCoroutine(DestroyBulletAfterTime(bullet, lifeTime));
//     }

//     private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
//     {
//         yield return new WaitForSeconds(delay);
//         Destroy(bullet);
//     }
// }
