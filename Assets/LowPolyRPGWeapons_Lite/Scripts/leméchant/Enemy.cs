using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100;

    // public void TakeDamage(int damage)
    // {
    //     health -= damage;
    //     Debug.Log("Ennemi touché ! Santé restante : " + health);

    //     if (health <= 0)
    //     {
    //         Die();
    //     }
    // }
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Ennemi touché ! Santé restante : " + health);

        // Effet de couleur lors de l'impact
        GetComponent<Renderer>().material.color = Color.red;
        Invoke("ResetColor", 0.1f);

        if (health <= 0)
        {
            Die();
        }
    }

    void ResetColor()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }
    void Die()
    {
        Debug.Log("Le boug est dead!");
        Destroy(gameObject);
    }
}
