using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class PlayerFollowCamera : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    
    private void Start()
    {
        // Check if we have the virtual camera assigned in the inspector
        if (virtualCamera == null)
        {
            Debug.LogError("CinemachineVirtualCamera not assigned in the inspector.");
            return;
        }
        
        // Register to the event when a player is spawned
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            // We only want to set the camera to follow the local player
            Invoke(nameof(SetCameraToFollowLocalPlayer), 0.1f);
        }
    }

    private void SetCameraToFollowLocalPlayer()
    {
        // Find all objects with the "Player" tag and look for the local player
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            NetworkBehaviour networkBehaviour = player.GetComponent<NetworkBehaviour>();
            
            if (networkBehaviour != null && networkBehaviour.IsOwner)
            {
                // Set the virtual camera to follow and look at the local player
                virtualCamera.Follow = player.transform;
                virtualCamera.LookAt = player.transform;
                Debug.Log("Camera now following the local player.");
                return;
            }
        }
        
        Debug.LogWarning("Local player not found. Ensure player prefab has the 'Player' tag.");
    }
}
