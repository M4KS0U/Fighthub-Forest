using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Netcode;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100;
    private bool OutZone = false;
    private DamageEffect damageEffect;

    void Start()
    {
        // get the DamageEffect component in the scene
        damageEffect = GameObject.FindObjectOfType<DamageEffect>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "BorderLimit")
        {
            OutZone = false;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "BorderLimit")
        {
            OutZone = true;
        }
    }

    private void Update()
    {
        if (OutZone)
        {
            TakeDamage(Time.deltaTime * 10);
        }
    }
    

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Ennemi touché ! Santé restante : " + health);

        // Effet de couleur lors de l'impact
        Renderer render = GetComponent<Renderer>();
        if (render)
        {
            render.material.color = Color.red;
            Invoke("ResetColor", 0.1f);
        }
        //if (IsOwner)
            damageEffect.DamageEffectOn();

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
