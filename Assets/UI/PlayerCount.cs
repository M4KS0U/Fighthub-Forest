using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ClientCount : NetworkBehaviour
{
    private TextMeshProUGUI text;
    // The network variable that will keep track of the number of players in the game.
    private NetworkVariable<int> playerCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the local client is the server.
        if (NetworkManager.Singleton.IsServer)
        {
            // Update the player count on the server.
            playerCount.Value = NetworkManager.Singleton.ConnectedClientsList.Count;
        }

        // Display the player count on the UI
        int nb = playerCount.Value;
        if (nb == 1)
        {
            text.text = "There is currently 1 player on the lobby";
        } 
        else 
        {
            text.text = "There are currently " + nb + " players on the lobby";
        }
    }
}
