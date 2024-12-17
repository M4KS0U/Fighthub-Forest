using UnityEngine;
using Unity.Netcode;

public class AliveUIVisibility : NetworkBehaviour
{
    public bool inverse = false;
    private GameObject player;

    void Update()
    {
        if (player == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p in players)
            {
                if (p.GetComponent<NetworkObject>().IsLocalPlayer)
                {
                    player = p;
                    break;
                }
            }
        }
        Debug.Log("AliveUIVisibility: " + player.GetComponent<Enemy>().isAlive + " obj: " + player.gameObject.name);
        if (player == null)
        {
            return;
        }
        if (player.GetComponent<Enemy>().isAlive == inverse)
        {
            gameObject.SetActive(false);
        } else
        {
            gameObject.SetActive(true);
        }
    }
}
