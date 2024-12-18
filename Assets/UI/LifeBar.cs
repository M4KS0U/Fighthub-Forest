using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LifeBar : MonoBehaviour
{
    float maxSize = 1;
    float currentSize = 1;
    float maxLife = 100;
    public float currentLife = 100;

    RectTransform rt;

    GameObject player;
    void Start()
    {
        rt = GetComponent<RectTransform>();
        // get size in px from stretch mode

        maxSize = rt.sizeDelta.x;
        
    }

    void getPlayerOwner()
    {
        ulong playerId = NetworkManager.Singleton.LocalClientId;
        NetworkSpawnManager manager = NetworkManager.Singleton.SpawnManager;

        if (manager == null)
            return;
        NetworkObject obj = manager.GetPlayerNetworkObject(playerId);
        if (obj == null)
            return;
        player = obj.gameObject;
        // get player life
        currentLife = player.GetComponent<Enemy>().health;
    }

    // Update is called once per frame
    void Update()
    {
        // change size of life bar
        // based on current life
        // with stretch mode
        getPlayerOwner();
        currentSize = currentLife / maxLife;
        rt.sizeDelta = new Vector2(maxSize * currentSize, rt.sizeDelta.y);
    }
}
