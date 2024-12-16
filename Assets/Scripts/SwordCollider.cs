using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    private MeleeWeapon meleeWeapon;

    private void Start()
    {
        // Find the MeleeWeapon script on the parent player
        meleeWeapon = GetComponentInParent<MeleeWeapon>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Ensure the collision is with a Player and not the parent holding the Sword
        if (other.CompareTag("Player") && other.transform.root != transform.root)
        {
            Debug.Log(meleeWeapon + " " + meleeWeapon.IsAttacking());
            if (meleeWeapon != null && meleeWeapon.IsAttacking())
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(10f);
                }
            }
        }
    }
}
