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
            SetCameraToFollowPlayer();
        }
    }

    private void SetCameraToFollowPlayer()
    {
        // Find the player's object (assuming it has a "Player" tag or other identifier)
        GameObject player = GameObject.FindWithTag("Player");
        
        if (player != null)
        {
            // Set the virtual camera to follow and look at the player
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
            Debug.Log("Camera now following and looking at the player.");
        }
        else
        {
            Debug.LogWarning("Player not found. Ensure player prefab has the 'Player' tag or appropriate identifier.");
        }
    }
}
