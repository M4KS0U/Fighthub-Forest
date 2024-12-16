// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class FireWeapon : MonoBehaviour
// {
//     public GameObject bulletPrefab;
//     public Transform bulletSpawn;

//     public float bulletSpeed = 30;

//     public float lifeTime = 3;
//     void Start()
//     {
        
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWeapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public Animator playerAnimator;

    public float bulletSpeed = 30;
    public float lifeTime = 3;

    private int shootLayerIndex;

    private void Start()
    {
        // Récupère l'index du layer "Shoot" dans l'Animator
        if (playerAnimator != null)
        {
            shootLayerIndex = playerAnimator.GetLayerIndex("Shoot");
        }

        // Active le layer "Shoot" au démarrage si ce script est activé
        EnableShootLayer(true);
    }

    private void OnEnable()
    {
        // Active le layer "Shoot" lorsque ce script est activé
        EnableShootLayer(true);
    }

    private void OnDisable()
    {
        // Désactive le layer "Shoot" lorsque ce script est désactivé
        EnableShootLayer(false);
    }

    private void Update()
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

    private void EnableShootLayer(bool enable)
    {
        if (playerAnimator != null && shootLayerIndex != -1)
        {
            float weight = enable ? 1f : 0f;
            playerAnimator.SetLayerWeight(shootLayerIndex, weight);
        }
    }
}
