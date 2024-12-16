// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class BulletBehaviour : MonoBehaviour
// {
//     public int bulletDamage = 25;

//     private void OnTriggerEnter(Collider other)
//     {
//         print("Touche par balle " + other.name);

//         if (other.CompareTag("Enemy"))
//         {
//             Enemy enemy = other.GetComponent<Enemy>();
//             if (enemy != null)
//             {
//                 enemy.TakeDamage(bulletDamage);
//             }
//         }

//         Destroy(gameObject);
//     }
// }

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class BulletBehaviour : MonoBehaviour
// {
//     public int bulletDamage = 25;

//     private void OnTriggerEnter(Collider other)
//     {
//         // Affiche dans la console l'objet touché
//         print("Touche par balle : " + other.name);

//         // Si l'objet touché a le tag "Enemy", inflige des dégâts
//         if (other.CompareTag("Enemy"))
//         {
//             Enemy enemy = other.GetComponent<Enemy>();
//             if (enemy != null)
//             {
//                 enemy.TakeDamage(bulletDamage);
//             }
//         }
        
//         // La balle est détruite après avoir touché un objet (qu'il soit un ennemi ou non)
//         Destroy(gameObject);
//     }
// }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public int bulletDamage = 25;

    private void OnTriggerEnter(Collider other)
    {
        // Affiche dans la console l'objet touché
        // print("Touche par balle : " + other.name);
        print("OTUOTU");
        // Vérifie si l'objet a un tag et s'il correspond à "Enemy"
        if (other.CompareTag("Enemy"))
        {
            // Récupère le composant Enemy sur l'objet
            Enemy enemy = other.GetComponent<Enemy>();

            // Si l'ennemi existe, applique des dégâts
            if (enemy != null)
            {
                enemy.TakeDamage(bulletDamage);
            }
        }
        
        // La balle est détruite après avoir touché un objet (qu'il soit un ennemi ou non)
        Destroy(gameObject);
    }
}
