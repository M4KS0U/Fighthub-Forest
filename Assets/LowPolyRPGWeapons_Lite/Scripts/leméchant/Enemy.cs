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

    private MaterialPropertyBlock mpb;

    void Start()
    {
        // get the DamageEffect component in the scene
        damageEffect = GameObject.FindObjectOfType<DamageEffect>();
        animator = GetComponent<Animator>();
        mpb = new MaterialPropertyBlock();
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

    private void ColorDamage()
    {
        // Appliquer la couleur principale (_BaseColor)
        mpb.SetColor("_BaseColor", Color.red);

        mpb.SetTexture("_BaseMap", Texture2D.whiteTexture);

        // Récupérer le SkinnedMeshRenderer principal
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.SetPropertyBlock(mpb);
        }

        // Appliquer aux enfants
        Renderer[] rends = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (Renderer r in rends)
        {
            r.SetPropertyBlock(mpb);
        }
    }
    

    public void TakeDamage(float damage)
    {
        if (!isAlive)
            return;
        health -= damage;
        Debug.Log("Ennemi touché ! Santé restante : " + health);

        // find all the renderers in the object
        ColorDamage();
        Invoke("ResetColor", 0.2f);
        if (IsOwner)
            damageEffect.DamageEffectOn();
        if (DeadBody)
            animator.SetTrigger("Hit");
        if (health <= 0)
        {
            Die();
        }
    }

    void ResetColor()
    {
        mpb.Clear();

        // Récupérer le SkinnedMeshRenderer principal
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.SetPropertyBlock(mpb);
        }

        // Appliquer aux enfants
        Renderer[] rends = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (Renderer r in rends)
        {
            r.SetPropertyBlock(mpb);
        }
    }

    void Die()
    {
        isAlive = false;
        Netcode.NetworkPlayer player = GetComponent<Netcode.NetworkPlayer>();
        if (player)
            player.enabled = false;
        else
            GetComponent<AIControllerBear>().enabled = false;
        if (IsServer) {
            isAliveServ.Value = false;
        }
        if (!DeadBody)
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isDead", true);
            return;
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
        Netcode.NetworkPlayer player = GetComponent<Netcode.NetworkPlayer>();
        if (player)
            player.enabled = true;
        else
            GetComponent<AIControllerBear>().enabled = true;
        if (IsServer) {
            isAliveServ.Value = true;
        }
        if (!DeadBody)
        {
            animator.SetBool("isDead", false);
            return;
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
