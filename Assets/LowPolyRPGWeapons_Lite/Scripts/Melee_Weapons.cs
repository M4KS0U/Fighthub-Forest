using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    private bool isEnemyInRange = false;
    private Collider enemyCollider;
    private Animator playerAnimator;

    [SerializeField] private GameObject player;

    private void Start()
    {
        if (player != null)
        {
            playerAnimator = player.GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
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

        if (enemyCollider != null)
        {
            Debug.Log(enemyCollider.name);
            Enemy enemy = enemyCollider.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(25);
            }
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