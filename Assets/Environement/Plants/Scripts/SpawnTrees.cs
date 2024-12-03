using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnTree : NetworkBehaviour
{
    public GameObject[] treePrefabs;
    public int maxTrees = 100;
    public float minDistance = 2f;
    public float maxSlope = 30f; // Maximum inclination in degrees
    public float maxRaycastDistance = 100f;

    private NetworkList<Vector3> spawnPoints; // Networked list for tree positions
    public float minScale = 0.8f;
    public float maxScale = 1.2f;

    private void Awake()
    {
        // Initialize the NetworkList only once (before the network is started)
        spawnPoints = new NetworkList<Vector3>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Generate spawn points on the server once the network is fully initialized
            GenerateSpawnPoints();
            PlaceTrees(); // Server places trees locally for visualization purposes
        }

        // Clients instantiate trees based on the server's spawnPoints
        if (IsClient)
        {
            // Manually place trees if the list is already populated
            if (spawnPoints.Count > 0)
            {
                PlaceTrees();
            }

            // Listen for changes in spawnPoints so clients can update trees in real time
            spawnPoints.OnListChanged += (change) => PlaceTrees();
        }
    }

    private void GenerateSpawnPoints()
    {
        int tries = 0;
        while (spawnPoints.Count < maxTrees && tries < maxTrees * 10)
        {
            // Generate a random point in the area
            Vector3 randomPoint = new Vector3(
                Random.Range(transform.position.x - maxRaycastDistance, transform.position.y + maxRaycastDistance),
                transform.position.y + maxRaycastDistance / 2,
                Random.Range(transform.position.z - maxRaycastDistance, transform.position.y + maxRaycastDistance)
            );

            // Raycast down to find the terrain height and slope
            if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, maxRaycastDistance))
            {
                Vector3 hitNormal = hit.normal;
                float slopeAngle = Vector3.Angle(Vector3.up, hitNormal);

                if (IsPreferredLocation(hit, slopeAngle) && IsNotTooClose(hit.point))
                {
                    spawnPoints.Add(hit.point); // Add to NetworkList for synchronization
                }
            }
            tries++;
        }
    }

    private bool IsPreferredLocation(RaycastHit hit, float slopeAngle)
    {
        // Check the height of the hit point
        if (hit.point.y < transform.position.y - 1.8) return false;

        // Check triangle color (using vertex colors)
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider != null)
        {
            Mesh mesh = meshCollider.sharedMesh;
            int triangleIndex = hit.triangleIndex * 3;

            Color vertexColorA = mesh.colors[mesh.triangles[triangleIndex]];
            Color vertexColorB = mesh.colors[mesh.triangles[triangleIndex + 1]];
            Color vertexColorC = mesh.colors[mesh.triangles[triangleIndex + 2]];

            Color averageColor = (vertexColorA + vertexColorB + vertexColorC) / 3f;
            Vector3 averageColorVector = new Vector3(averageColor.r, averageColor.g, averageColor.b);
            averageColorVector.Normalize();

            // The greener it is, the better the spot
            if (averageColorVector.y < 0.75) return false;
            return true;
        }
        return false;
    }

    private bool IsNotTooClose(Vector3 position)
    {
        foreach (Vector3 point in spawnPoints)
        {
            if (Vector3.Distance(point, position) < minDistance)
                return false;
        }
        return true;
    }

    private void PlaceTrees()
    {
        // Clear existing trees (optional, if you want to avoid duplicates)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Instantiate trees at all spawn points in the NetworkList
        foreach (Vector3 spawnPoint in spawnPoints)
        {
            var treePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
            GameObject tree = Instantiate(treePrefab, spawnPoint, Quaternion.identity, transform);

            // Randomize scale
            float randomScale = Random.Range(minScale, maxScale);
            tree.transform.localScale *= randomScale;

            // Randomize rotation
            tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        }
    }
}
