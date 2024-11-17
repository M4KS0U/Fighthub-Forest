// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class MeleeWeapon : MonoBehaviour
// {
//     private bool isEnemyInRange = false;
//     private Collider enemyCollider;

//     private void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Space) && isEnemyInRange)
//         {
//             Attack();
//         }
//     }

//     private void Attack()
//     {
//         // Debug.Log(enemyCollider.name);
//         enemyCollider.GetComponent<Enemy>().TakeDamage(25);
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Enemy"))
//         {
//             isEnemyInRange = true;
//             enemyCollider = other;
//         }
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Enemy"))
//         {
//             isEnemyInRange = false;
//             enemyCollider = null;
//         }
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    private bool isEnemyInRange = false;
    private Collider enemyCollider;
    private Animator playerAnimator; // Animator du joueur

    [SerializeField] private GameObject player; // Référence au joueur (assignez-le via l'inspecteur)

    private void Start()
    {
        // Si le joueur est assigné, récupérez son Animator
        if (player != null)
        {
            playerAnimator = player.GetComponent<Animator>();
        }

        if (playerAnimator == null)
        {
            Debug.LogError("Aucun Animator trouvé sur le joueur. Assurez-vous que le joueur a un Animator.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) // Appuyer sur M pour attaquer
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Attack");
        }
        else
        {
            Debug.LogWarning("Animator du joueur non assigné ou introuvable.");
        }

        // Vérifiez si enemyCollider n'est pas nul avant d'essayer de l'utiliser
        if (enemyCollider != null)
        {
            Debug.Log(enemyCollider.name);
            Enemy enemy = enemyCollider.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(25);
            }
            else
            {
                Debug.LogWarning("L'objet n'a pas de composant Enemy.");
            }
        }
        else
        {
            Debug.LogWarning("Aucun ennemi dans la portée.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            isEnemyInRange = true;
            enemyCollider = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            isEnemyInRange = false;
            enemyCollider = null;
        }
    }
}
