using UnityEngine;
using Unity.Netcode;

public class HostUIVisibility : NetworkBehaviour
{
    public bool inverse = false;
    public override void OnNetworkSpawn()
    {
        Debug.Log("HostUIVisibility: " + IsHost + " obj: " + gameObject.name);
        if (IsHost == inverse) {
            gameObject.SetActive(false);
        }
    }
}
