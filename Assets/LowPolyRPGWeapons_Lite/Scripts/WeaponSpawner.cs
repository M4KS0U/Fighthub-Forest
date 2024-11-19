using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WeaponSpawner : NetworkBehaviour
{
    [Header("Weapons")]
    public GameObject[] weapons; // List of weapon prefabs
    public GameObject networkWeaponPrefab;

    [Header("Spawn Settings")]
    public string playerTag = "Player"; // Tag used to find the player
    public int numberOfWeaponsToSpawn = 5; // Number of weapons to spawn

    [Header("Map Settings")]
    public Vector2 MapSize = new Vector2(50f, 50f); // Dimensions of the map
    public LayerMask GroundLayers; // Layers considered as ground for spawning

    private Transform player; // Reference to the player's Transform

    private List<NetworkObjectReference> spawnedWeapons = new List<NetworkObjectReference>();

    void Start()
    {
        StartCoroutine(FindPlayerAndInitialize());
    }

    IEnumerator FindPlayerAndInitialize()
    {
        // Wait until the player is instantiated in the scene
        while (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            yield return null; // Wait for the next frame
        }

        if (IsHost)
        {
            SpawnWeaponsOnMap();
        }
    }

    void SpawnWeaponsOnMap()
    {
        for (int i = 0; i < numberOfWeaponsToSpawn; i++)
        {
            Vector3 randomPosition = GetRandomPositionOnMap();
            int randomIndex = Random.Range(0, weapons.Length);

            // Instantiate the weapon prefab
            GameObject weaponInstance = Instantiate(networkWeaponPrefab, randomPosition, Quaternion.identity);

            // Spawn the network object
            NetworkObject networkObject = weaponInstance.GetComponent<NetworkObject>();
            networkObject.Spawn(true); // Spawn it for all clients

            // Now set the weapon index
            weaponInstance.GetComponent<WeaponData>().weaponIndex.Value = randomIndex;

            // Keep track of the spawned weapons
            spawnedWeapons.Add(networkObject);
        }
    }


    private Vector3 GetRandomPositionOnMap()
    {
        // Generate a random position within the defined map bounds
        float randomX = Random.Range(-MapSize.x / 2, MapSize.x / 2);
        float randomZ = Random.Range(-MapSize.y / 2, MapSize.y / 2);
        Vector3 randomPosition = new Vector3(randomX, 0f, randomZ);

        // Ensure the random position is grounded
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
                GameObject weaponInstance = Instantiate(weapons[weaponObject.GetComponent<WeaponData>().weaponIndex.Value],
                                                        weaponObject.transform.position,
                                                        Quaternion.identity);
                weaponInstance.transform.parent = weaponObject.transform;
            }
        }
    }
}
