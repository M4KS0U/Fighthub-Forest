using UnityEngine;
using Unity.Netcode;

public class NetworkWeapon : NetworkBehaviour
{
    public string objectType = "Weapon";
    private bool playerInRange;
    private NetworkVariable<bool> isPickedUp = new NetworkVariable<bool>(false); // Sync weapon state

    void Update()
    {
        if (IsOwner && Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            RequestPickUpWeaponServerRpc(NetworkManager.Singleton.LocalClientId); // Notify server to pick up the weapon
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestPickUpWeaponServerRpc(ulong playerId)
    {
        if (isPickedUp.Value) return; // Prevent multiple pickups
        isPickedUp.Value = true; // Mark the weapon as picked up

        // Notify all clients to attach the weapon visually to the appropriate player
        AttachWeaponClientRpc(playerId);

        // Optionally destroy the weapon on the ground (it’s now "held")
        Destroy(gameObject);
    }

    [ClientRpc]
    private void AttachWeaponClientRpc(ulong playerId)
    {
        // Only activate the sword for the player who picked it up
        if (NetworkManager.Singleton.LocalClientId == playerId)
        {
            // Find the player object associated with the playerId
            var client = NetworkManager.Singleton.ConnectedClients[playerId];

            if (client != null)
            {
                Transform playerTransform = client.PlayerObject.transform;

                // Find the weapon holder (Sword_01) on the player
                Transform weaponHolder = FindInChildren(playerTransform, "Sword_01");

                if (weaponHolder != null)
                {
                    // Activate the weapon holder and attach the weapon to it
                    weaponHolder.gameObject.SetActive(true); // This activates the Sword_01 holder
                }
            }
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
        if (!IsOwner || isPickedUp.Value) return; // Only proceed if this player owns the object and it's not picked up

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
