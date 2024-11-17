using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Object : MonoBehaviour
{
    public string infoObject = "Object";
    private bool playerInRange;

    private Vector3 originalScale;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            Debug.Log($"Le joueur a ramassé : {infoObject}");
            RamasserObjet();
        }
    }

    void OnMouseOver()
    {
        transform.localScale = originalScale * 1.1f;
    }

    void OnMouseExit()
    {
        transform.localScale = originalScale;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log($"Le joueur peut ramasser : {infoObject}");
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void RamasserObjet()
    {
        Destroy(gameObject);
    }
}