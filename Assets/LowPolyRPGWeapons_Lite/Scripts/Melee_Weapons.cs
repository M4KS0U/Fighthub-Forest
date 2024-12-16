using Unity.Netcode;
using UnityEngine;

public class MeleeWeapon : NetworkBehaviour
{
    private Collider enemyCollider;
    private Animator playerAnimator;

    [SerializeField] private GameObject player;

    private bool isAttacking = false; // Flag to track if the player is attacking

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

        // Notify all clients to play the attack animation
        PlayAttackAnimationClientRpc();
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    [ClientRpc]
    private void PlayAttackAnimationClientRpc()
    {
        if (playerAnimator != null)
        {
            isAttacking = true;
            playerAnimator.SetTrigger("Attack");
            Invoke(nameof(ResetAttack), 1.0f); // Adjust duration to match your animation
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner || transform.parent.name == "Hand_r") return;

        if (other.gameObject.GetComponent<Enemy>())
        {
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

    public bool IsAttacking()
    {
        return isAttacking;
    }
}
