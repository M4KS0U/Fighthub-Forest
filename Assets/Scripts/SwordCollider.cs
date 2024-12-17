using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    private MeleeWeapon meleeWeapon;

    private float cd = 0.0f;

    private void Start()
    {
        // Find the MeleeWeapon script on the parent player
        meleeWeapon = GetComponentInParent<MeleeWeapon>();
    }

    private void Update()
    {
        if (cd > 0.0f)
        {
            cd -= Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (cd > 0.0f)
        {
            return;
        }
        // Ensure the collision is with a Player and not the parent holding the Sword
        if (other.transform.root != transform.root)
        {
            if (meleeWeapon != null && meleeWeapon.IsAttacking())
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(20f);
                    cd = 1.0f;
                }
            }
        }
    }
}
