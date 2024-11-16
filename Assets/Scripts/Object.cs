using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Object : MonoBehaviour
{
    public string infoObejct = "Object";
    private bool playerInRange;

    private Vector3 originalScale;
    Color m_MouseOverColor = Color.blue;
    Color m_OriginalColor;
    MeshRenderer m_Renderer;

    void Start()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange && TooltipManager.Instance.onTarget)
        {
            if (!InventorySystem.Instance.CheckIsFull())
            {
                InventorySystem.Instance.AddToInventory(infoObejct);
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
        transform.localScale = originalScale * 1.1f;
    }

    void OnMouseExit()
    {
        transform.localScale = originalScale;
    }

    public string GetInfo()
    {
        return infoObejct;
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
