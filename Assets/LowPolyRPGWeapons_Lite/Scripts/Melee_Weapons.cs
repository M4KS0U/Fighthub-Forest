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
        // left click to attack
        if (IsOwner && Input.GetMouseButtonDown(0) && !isAttacking)
        {
            AttackServerRpc();
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

    [ServerRpc]
    private void AttackServerRpc()
    {
        // Server triggers the ClientRpc for all clients
        PlayAttackAnimationClientRpc();
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    [ClientRpc]
    private void PlayAttackAnimationClientRpc()
    {
        Debug.Log("playerAnimator " + playerAnimator);
        if (playerAnimator != null)
        {
            isAttacking = true;
            playerAnimator.SetTrigger("Attack");
            Invoke(nameof(ResetAttack), 0.5f); // Adjust duration to match your animation
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner || (transform.parent && transform.parent.name == "Hand_r")) return;

        if (other.gameObject.GetComponent<Enemy>())
        {
            enemyCollider = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner) return;

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
