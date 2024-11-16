using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Object : MonoBehaviour
{
    public string infoText = "Object";
    public bool playerInRange;

    Color m_MouseOverColor = Color.blue;
    Color m_OriginalColor;
    MeshRenderer m_Renderer;

    void Start()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        m_OriginalColor = m_Renderer.material.color;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && playerInRange && TooltipManager.Instance.onTarget)
        {
            if (!InventorySystem.Instance.CheckIsFull())
            {
                InventorySystem.Instance.AddToInventory(infoText);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Inventory is full !!");
            }
        }
    }

    void OnMouseOver()
    {
        if (InventorySystem.Instance.isOpen == false)
            m_Renderer.material.color = m_MouseOverColor;
    }

    void OnMouseExit()
    {
        m_Renderer.material.color = m_OriginalColor;
    }

    public string GetInfo()
    {
        return infoText;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}


/* TODO

Faire en sorte que le nom de l'objet soit au dessus de l'objet et ne suive pas la souris

Récupération des objets lorsque l'on clique sur E
    Faire la range de l'objet
    Détecter lorsque le player est dans la range 
    Faire disparaitre l'objet lorsque l'on clique sur la touche
 */