using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode; // Include Netcode for GameObjects namespace
using UnityEngine;

public class MeleeWeapon : NetworkBehaviour
{
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
        if (IsOwner && Input.GetKeyDown(KeyCode.M)) // Only allow the local player to initiate attacks
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
            Enemy enemy = enemyCollider.GetComponent<Enemy>();

            if (enemy != null)
            {
                // Request the server to apply damage
                ApplyDamageServerRpc(enemyCollider.gameObject.GetComponent<NetworkObject>().NetworkObjectId, 25);
            }
        }

        // Notify all clients to play the attack animation
        PlayAttackAnimationClientRpc();
    }

    [ServerRpc]
    private void ApplyDamageServerRpc(ulong enemyNetworkObjectId, int damage)
    {
        // Validate the enemy object exists on the server
        NetworkObject enemyObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[enemyNetworkObjectId];
        if (enemyObject != null)
        {
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    [ClientRpc]
    private void PlayAttackAnimationClientRpc()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Attack");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return; // Only the local player tracks their own triggers

        if (other.gameObject.GetComponent<Enemy>())
        {
            Debug.Log("Enemy in range");
            enemyCollider = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner) return; // Only the local player tracks their own triggers

        if (other.gameObject.GetComponent<Enemy>())
        {
            enemyCollider = null;
        }
    }
}
