using UnityEngine;
using Unity.Netcode;

public class NetworkWeapon : NetworkBehaviour
{
    public string objectType = "Weapon";
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

        Transform weaponHolder = FindInChildren(player.transform, "Sword_01");

        if (weaponHolder != null)
        {
            // Send the target player ID and weapon holder information to all clients
            AttachWeaponClientRpc(playerId, weaponHolder.gameObject.name);
        }

        Destroy(gameObject);
    }

    [ClientRpc]
    private void AttachWeaponClientRpc(ulong playerId, string weaponHolderName)
    {
        if (NetworkManager.Singleton.LocalClientId == playerId)
        {
            var playerTransform = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform;

            Transform weaponHolder = FindInChildren(playerTransform, weaponHolderName);

            if (weaponHolder != null)
            {
                weaponHolder.gameObject.SetActive(true);
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
