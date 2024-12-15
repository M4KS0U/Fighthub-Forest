// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Unity.Netcode;

// public class WeaponSpawner : NetworkBehaviour
// {
//     [Header("Weapons")]
//     public GameObject[] weapons;
//     public GameObject networkWeaponPrefab;

//     [Header("Spawn Settings")]
//     public string playerTag = "Player";
//     public int numberOfWeaponsToSpawn = 5;

//     [Header("Map Settings")]
//     public Vector2 MapSize = new Vector2(50f, 50f);
//     public LayerMask GroundLayers;

//     private Transform player;

//     private List<NetworkObjectReference> spawnedWeapons = new List<NetworkObjectReference>();

//     void Start()
//     {
//         StartCoroutine(FindPlayerAndInitialize());
//     }

//     IEnumerator FindPlayerAndInitialize()
//     {
//         while (player == null)
//         {
//             GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
//             if (playerObject != null)
//             {
//                 player = playerObject.transform;
//             }
//             yield return null;
//         }

//         if (IsHost)
//         {
//             SpawnWeaponsOnMap();
//         }
//     }

//     void SpawnWeaponsOnMap()
//     {
//         for (int i = 0; i < numberOfWeaponsToSpawn; i++)
//         {
//             Vector3 randomPosition = GetRandomPositionOnMap();
//             int randomIndex = Random.Range(0, weapons.Length);

//             GameObject weaponInstance = Instantiate(networkWeaponPrefab, randomPosition, Quaternion.identity);

//             NetworkObject networkObject = weaponInstance.GetComponent<NetworkObject>();
//             networkObject.Spawn(true);

//             weaponInstance.GetComponent<WeaponData>().weaponIndex.Value = randomIndex;

//             spawnedWeapons.Add(networkObject);
//         }
//     }


//     private Vector3 GetRandomPositionOnMap()
//     {
//         float randomX = Random.Range(-MapSize.x / 2, MapSize.x / 2);
//         float randomZ = Random.Range(-MapSize.y / 2, MapSize.y / 2);
//         Vector3 randomPosition = new Vector3(randomX, 0f, randomZ);

//         if (Physics.Raycast(new Vector3(randomX, 10f, randomZ), Vector3.down, out RaycastHit hit, Mathf.Infinity, GroundLayers))
//         {
//             randomPosition.y = hit.point.y;
//         }

//         return randomPosition;
//     }

//     public override void OnNetworkSpawn()
//     {
//         if (!IsHost)
//         {
//             SyncWeaponsOnClient();
//         }
//     }

//     private void SyncWeaponsOnClient()
//     {
//         foreach (var weaponRef in spawnedWeapons)
//         {
//             if (weaponRef.TryGet(out NetworkObject weaponObject))
//             {
//                 GameObject weaponInstance = Instantiate(weapons[weaponObject.GetComponent<WeaponData>().weaponIndex.Value],
//                                                         weaponObject.transform.position,
//                                                         Quaternion.identity);
//                 weaponInstance.transform.parent = weaponObject.transform;
//             }
//         }
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WeaponSpawner : NetworkBehaviour
{
    [Header("Weapons")]
    public GameObject[] weapons;
    public GameObject[] networkWeaponPrefabs;

    [Header("Spawn Settings")]
    public string playerTag = "Player";
    public int numberOfWeaponsToSpawn = 5;

    [Header("Map Settings")]
    public Vector2 MapSize = new Vector2(50f, 50f);
    public LayerMask GroundLayers;

    private Transform player;

    private List<NetworkObjectReference> spawnedWeapons = new List<NetworkObjectReference>();

    void Start()
    {
        StartCoroutine(FindPlayerAndInitialize());
    }

    IEnumerator FindPlayerAndInitialize()
    {
        while (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            yield return null;
        }

        if (IsHost)
        {
            SpawnWeaponsOnMap();
        }
    }

    void SpawnWeaponsOnMap()
    {
        spawnedWeapons.Clear();

        for (int i = 0; i < numberOfWeaponsToSpawn; i++)
        {
            Vector3 randomPosition = GetRandomPositionOnMap();

            int randomNetworkPrefabIndex = Random.Range(0, networkWeaponPrefabs.Length);
            GameObject selectedNetworkPrefab = networkWeaponPrefabs[randomNetworkPrefabIndex];

            int randomWeaponIndex = Random.Range(0, weapons.Length);

            GameObject weaponInstance = Instantiate(selectedNetworkPrefab, randomPosition, Quaternion.identity);

            NetworkObject networkObject = weaponInstance.GetComponent<NetworkObject>();
            networkObject.Spawn(true);

            weaponInstance.GetComponent<WeaponData>().weaponIndex.Value = randomWeaponIndex;

            spawnedWeapons.Add(networkObject);
        }
    }

    private Vector3 GetRandomPositionOnMap()
    {
        float randomX = Random.Range(-MapSize.x / 2, MapSize.x / 2);
        float randomZ = Random.Range(-MapSize.y / 2, MapSize.y / 2);
        Vector3 randomPosition = new Vector3(randomX, 0f, randomZ);

        if (Physics.Raycast(new Vector3(randomX, 10f, randomZ), Vector3.down, out RaycastHit hit, Mathf.Infinity, GroundLayers))
        {
            randomPosition.y = hit.point.y;
        }

        return randomPosition;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsHost)
        {
            SyncWeaponsOnClient();
        }
    }

    private void SyncWeaponsOnClient()
    {
        foreach (var weaponRef in spawnedWeapons)
        {
            if (weaponRef.TryGet(out NetworkObject weaponObject))
            {
                GameObject weaponInstance = Instantiate(
                    weapons[weaponObject.GetComponent<WeaponData>().weaponIndex.Value],
                    weaponObject.transform.position,
                    Quaternion.identity
                );
                weaponInstance.transform.parent = weaponObject.transform;
            }
        }
    }
}
