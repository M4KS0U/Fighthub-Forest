// using UnityEngine;
// using Unity.Netcode;
// using System.Collections.Generic;

// public class NetworkWeapon : NetworkBehaviour
// {
//     public string objectType = "Weapon";
//     private bool playerInRange;
//     private NetworkVariable<bool> isPickedUp = new NetworkVariable<bool>(false);

//     void Update()
//     {
//         if (IsOwner && Input.GetKeyDown(KeyCode.E) && playerInRange)
//         {
//             RequestPickUpWeaponServerRpc(NetworkManager.Singleton.LocalClientId);
//         }
//     }

//     [ServerRpc(RequireOwnership = false)]
//     private void RequestPickUpWeaponServerRpc(ulong playerId)
//     {
//         if (isPickedUp.Value) return;
//         isPickedUp.Value = true;

//         var player = NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject;

//         Transform weaponHolder = FindInChildren(player.transform, "Sword_01");
//         if (weaponHolder != null)
//         {
//             // Send the target player ID and weapon holder information to all clients
//             AttachWeaponClientRpc(playerId, weaponHolder.gameObject.name);
//         }

//         Destroy(gameObject);
//     }

//     [ClientRpc]
//     private void AttachWeaponClientRpc(ulong playerId, string weaponHolderName)
//     {
//         Debug.Log(playerId + " " + NetworkManager.Singleton.LocalClientId);
//         if (NetworkManager.Singleton.LocalClientId == playerId)
//         {
//             var playerTransform = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform;

//             Transform weaponHolder = FindInChildren(playerTransform, weaponHolderName);

//             Debug.Log(weaponHolder);
//             if (weaponHolder != null)
//             {
//                 weaponHolder.gameObject.SetActive(true);
//             }
//         } else {
//             GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
//             Transform weaponHolder = FindInChildren(players[playerId].transform, "Sword_01");

//             if (weaponHolder != null)
//             {
//                 weaponHolder.gameObject.SetActive(true);
//             }
//         }
//     }

//     private static Transform FindInChildren(Transform parent, string name)
//     {
//         foreach (Transform child in parent)
//         {
//             if (child.name == name)
//             {
//                 return child;
//             }

//             Transform found = FindInChildren(child, name);
//             if (found != null)
//             {
//                 return found;
//             }
//         }

//         return null;
//     }

//     private void OnTriggerEnter(Collider collider)
//     {
//         if (!IsOwner || isPickedUp.Value) return;

//         if (collider.CompareTag("Player"))
//         {
//             playerInRange = true;
//         }
//     }

//     private void OnTriggerExit(Collider collider)
//     {
//         if (collider.CompareTag("Player"))
//         {
//             playerInRange = false;
//         }
//     }
// }

// using UnityEngine;
// using Unity.Netcode;
// using System.Collections.Generic;

// public class NetworkWeapon : NetworkBehaviour
// {
//     public string objectType = "Weapon"; // Définit le type d'objet, ex: "Sword" ou "Pistol"
//     private bool playerInRange;
//     private NetworkVariable<bool> isPickedUp = new NetworkVariable<bool>(false);

//     void Update()
//     {
//         if (IsOwner && Input.GetKeyDown(KeyCode.E) && playerInRange)
//         {
//             RequestPickUpWeaponServerRpc(NetworkManager.Singleton.LocalClientId);
//         }
//     }

//     [ServerRpc(RequireOwnership = false)]
//     private void RequestPickUpWeaponServerRpc(ulong playerId)
//     {
//         if (isPickedUp.Value) return;
//         isPickedUp.Value = true;

//         var player = NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject;

//         // Détermine le nom du support de l'arme en fonction du type d'objet
//         string weaponHolderName = objectType == "Sword" ? "Sword_01" : "Pistol_E";

//         Transform weaponHolder = FindInChildren(player.transform, weaponHolderName);
//         if (weaponHolder != null)
//         {
//             // Envoie les informations de l'arme à attacher à tous les clients
//             AttachWeaponClientRpc(playerId, weaponHolder.gameObject.name);
//         }

//         Destroy(gameObject); // Détruit l'objet ramassé
//     }

//     [ClientRpc]
//     private void AttachWeaponClientRpc(ulong playerId, string weaponHolderName)
//     {
//         Debug.Log(playerId + " " + NetworkManager.Singleton.LocalClientId);
//         if (NetworkManager.Singleton.LocalClientId == playerId)
//         {
//             var playerTransform = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform;

//             Transform weaponHolder = FindInChildren(playerTransform, weaponHolderName);

//             Debug.Log(weaponHolder);
//             if (weaponHolder != null)
//             {
//                 weaponHolder.gameObject.SetActive(true);
//             }
//         }
//         else
//         {
//             GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
//             Transform weaponHolder = FindInChildren(players[playerId].transform, weaponHolderName);

//             if (weaponHolder != null)
//             {
//                 weaponHolder.gameObject.SetActive(true);
//             }
//         }
//     }

//     private static Transform FindInChildren(Transform parent, string name)
//     {
//         foreach (Transform child in parent)
//         {
//             if (child.name == name)
//             {
//                 return child;
//             }

//             Transform found = FindInChildren(child, name);
//             if (found != null)
//             {
//                 return found;
//             }
//         }

//         return null;
//     }

//     private void OnTriggerEnter(Collider collider)
//     {
//         if (!IsOwner || isPickedUp.Value) return;

//         if (collider.CompareTag("Player"))
//         {
//             playerInRange = true;
//         }
//     }

//     private void OnTriggerExit(Collider collider)
//     {
//         if (collider.CompareTag("Player"))
//         {
//             playerInRange = false;
//         }
//     }
// }

using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class NetworkWeapon : NetworkBehaviour
{
    public string objectType = "Weapon"; // Définit le type d'objet, ex: "Sword" ou "Pistol"
    private bool playerInRange;
    private NetworkVariable<bool> isPickedUp = new NetworkVariable<bool>(false);

    void Update()
    {
        if (IsOwner && Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            RequestPickUpWeaponServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestPickUpWeaponServerRpc(ulong playerId)
    {
        if (isPickedUp.Value) return;
        isPickedUp.Value = true;

        var player = NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject;

        // Détermine le nom du support de l'arme en fonction du type d'objet
        string weaponHolderName = objectType == "Sword" ? "Sword_01" : "Pistol_E";

        Transform weaponHolder = FindInChildren(player.transform, weaponHolderName);
        if (weaponHolder != null)
        {
            // Envoie les informations de l'arme à attacher à tous les clients
            AttachWeaponClientRpc(playerId, weaponHolder.gameObject.name);
        }

        Destroy(gameObject); // Détruit l'objet ramassé
    }

    [ClientRpc]
    private void AttachWeaponClientRpc(ulong playerId, string weaponHolderName)
    {
        Debug.Log(playerId + " " + NetworkManager.Singleton.LocalClientId);
        if (NetworkManager.Singleton.LocalClientId == playerId)
        {
            var playerTransform = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform;

            // Désactive l'arme actuellement activée
            DeactivateCurrentWeapon(playerTransform);

            // Trouve et active la nouvelle arme
            Transform weaponHolder = FindInChildren(playerTransform, weaponHolderName);

            Debug.Log(weaponHolder);
            if (weaponHolder != null)
            {
                weaponHolder.gameObject.SetActive(true);
            }
        }
        else
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            Transform weaponHolder = FindInChildren(players[playerId].transform, weaponHolderName);

            if (weaponHolder != null)
            {
                weaponHolder.gameObject.SetActive(true);
            }
        }
    }

    private void DeactivateCurrentWeapon(Transform playerTransform)
    {
        // Désactive les armes existantes (sword et pistol) si elles sont activées
        Transform swordHolder = FindInChildren(playerTransform, "Sword_01");
        if (swordHolder != null && swordHolder.gameObject.activeSelf)
        {
            swordHolder.gameObject.SetActive(false);
        }

        Transform pistolHolder = FindInChildren(playerTransform, "Pistol_E");
        if (pistolHolder != null && pistolHolder.gameObject.activeSelf)
        {
            pistolHolder.gameObject.SetActive(false);
        }
    }

    private static Transform FindInChildren(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }

            Transform found = FindInChildren(child, name);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!IsOwner || isPickedUp.Value) return;

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
