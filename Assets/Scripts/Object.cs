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
        if (Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            PickUpWeapon();
        }
    }

    private void PickUpWeapon()
    {
        transform.SetParent(playerWeaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        gameObject.SetActive(true);

        Debug.Log($"{objectType} picked up and equipped.");
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
