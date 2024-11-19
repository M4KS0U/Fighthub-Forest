// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;

// public class Object : MonoBehaviour
// {
//     public string objectInfo = "Object";
//     private bool playerInRange;

//     public SwitchWeapons switchWeapons;

//     void Start()
//     {
//     }

//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.E) && playerInRange)
//         {
//             Debug.Log($"Player has picked up: {objectInfo}");
//             PickupObject();
//         }
//     }

//     void OnMouseOver()
//     {
//         transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
//     }

//     void OnMouseExit()
//     {
//         transform.localScale = Vector3.one;
//     }

//     private void OnTriggerEnter(Collider collider)
//     {
//         if (collider.CompareTag("Player"))
//         {
//             playerInRange = true;
//             Debug.Log($"Player can pick up: {objectInfo}");
//         }
//     }

//     private void OnTriggerExit(Collider collider)
//     {
//         if (collider.CompareTag("Player"))
//         {
//             playerInRange = false;
//         }
//     }

//     private void PickupObject()
//     {
//         if (objectInfo == "Sword")
//         {
//             switchWeapons.SwitchWeapon("Sword");
//         }
//         else if (objectInfo == "Pistol")
//         {
//             switchWeapons.SwitchWeapon("Pistol");
//         }

//         Destroy(gameObject);
//     }
// }

using UnityEngine;

public class Object : MonoBehaviour
{
    public string objectType = "Weapon";
    public Transform playerWeaponHolder;

    private bool playerInRange;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange && playerWeaponHolder != null)
        {
            PickUpWeapon();
        }
    }

    private void PickUpWeapon()
    {
        playerWeaponHolder.gameObject.SetActive(true);
    }

    private static Transform FindInChildren(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child; // Si le GameObject est trouvé
            }

            // Recherche récursive dans les sous-enfants
            Transform found = FindInChildren(child, name);
            if (found != null)
            {
                return found; // Si le GameObject est trouvé dans les sous-enfants
            }
        }

        return null; // Si rien n'est trouvé
    }


    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInRange = true;
            GameObject player = collider.gameObject;
            // get rig child
            playerWeaponHolder = FindInChildren(player.transform, "Sword_01");
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
