// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class WeaponSpawner : MonoBehaviour
// {
//     [Header("Weapons")]
//     public GameObject[] weapons;

//     [Header("Spawn Settings")]
//     public Vector2 spawnAreaMin;
//     public Vector2 spawnAreaMax;
//     public int numberOfWeaponsToSpawn = 5;

//     void Start()
//     {
//         SpawnWeapons();
//     }

//     void SpawnWeapons()
//     {
//         for (int i = 0; i < numberOfWeaponsToSpawn; i++)
//         {
//             Vector3 randomPosition = GetRandomPosition();
//             GameObject randomWeapon = weapons[Random.Range(0, weapons.Length)];
//             Instantiate(randomWeapon, randomPosition, Quaternion.identity);
//         }
//     }

//     Vector3 GetRandomPosition()
//     {
//         float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
//         float randomZ = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
//         return new Vector3(randomX, 0, randomZ);
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [Header("Weapons")]
    public GameObject[] weapons; // List of weapon prefabs

    [Header("Spawn Settings")]
    public string playerTag = "Player"; // Tag used to find the player
    public float spawnRadius = 5f; // Radius around the player for spawning weapons
    public int numberOfWeaponsToSpawn = 5; // Number of weapons to spawn

    private Transform player; // Reference to the player's Transform

    void Start()
    {
        StartCoroutine(FindPlayerAndSpawnWeapons());
    }

    IEnumerator FindPlayerAndSpawnWeapons()
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

        SpawnWeaponsAroundPlayer();
    }

    void SpawnWeaponsAroundPlayer()
    {
        for (int i = 0; i < numberOfWeaponsToSpawn; i++)
        {
            Vector3 randomPosition = GetRandomPositionAroundPlayer();
            GameObject randomWeapon = weapons[Random.Range(0, weapons.Length)];
            Instantiate(randomWeapon, randomPosition, Quaternion.identity);
        }
    }

    Vector3 GetRandomPositionAroundPlayer()
    {
        // Generate a random point within a circle (on XZ plane)
        Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
        return new Vector3(
            player.position.x + randomPoint.x,
            player.position.y,
            player.position.z + randomPoint.y
        );
    }
}
