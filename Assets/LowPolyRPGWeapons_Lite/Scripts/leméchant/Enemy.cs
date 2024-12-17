using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Netcode;
using UnityEngine;
using Netcode;

public class Enemy : NetworkBehaviour
{
    public float health = 100;
    private bool OutZone = false;
    private DamageEffect damageEffect;

    public GameObject DeadBody;

    public bool isAlive = true;

    private NetworkVariable<bool> isAliveServ = new NetworkVariable<bool>(true);

    private Animator animator;

    void Start()
    {
        // get the DamageEffect component in the scene
        damageEffect = GameObject.FindObjectOfType<DamageEffect>();
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
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
        if (!IsServer) {
            if (isAliveServ.Value != isAlive && isAliveServ.Value)
                Revive();
            if (isAliveServ.Value != isAlive && !isAliveServ.Value)
                Die();
        }
        if (!isAlive)
            return;
        if (OutZone)
        {
            TakeDamage(Time.deltaTime * 15);
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
        if (IsOwner)
            damageEffect.DamageEffectOn();
        animator.SetTrigger("Hit");
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
        isAlive = false;
        GetComponent<Netcode.NetworkPlayer>().enabled = false;
        if (IsServer) {
            isAliveServ.Value = false;
        }
        //disable childs
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        Instantiate(DeadBody, transform.position, transform.rotation);
        GameObject hips = transform.Find("Skeleton").gameObject;
        GameObject hipsDead = DeadBody.transform.Find("Skeleton").gameObject;

        void copyChildPositions(Transform source, Transform target)
        {
            // copy all the position from the current rig and apply it to the dead body
            for (int i = 0; i < source.childCount; i++)
            {
                Transform sourceChild = source.GetChild(i);
                Transform targetChild = target.Find(sourceChild.name);
                if (targetChild)
                {
                    targetChild.position = sourceChild.position;
                    targetChild.rotation = sourceChild.rotation;
                    copyChildPositions(sourceChild, targetChild);
                }
            }
        }
    }

    public void Revive()
    {
        isAlive = true;
        health = 100;
        GetComponent<Netcode.NetworkPlayer>().enabled = true;
        if (IsServer) {
            isAliveServ.Value = true;
        }
        //Remove if there is a clone in name
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                if (child.name.Contains("(Clone)"))
                {
                    Destroy(child.gameObject);
                }
            }
        }
        
        //disable childs
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
