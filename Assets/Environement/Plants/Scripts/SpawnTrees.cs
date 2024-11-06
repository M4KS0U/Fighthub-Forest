using System.Collections.Generic;
using UnityEngine;

public class SpawnTree : MonoBehaviour
{
    public GameObject[] treePrefabs;
    public int maxTrees = 100;
    public float minDistance = 2f;
    public float maxSlope = 30f; // Maximum inclination in degrees
    public float maxRaycastDistance = 100f;

    private List<Vector3> spawnPoints = new List<Vector3>();

    void Start()
    {
        GenerateSpawnPoints();
        PlaceTrees();
    }

    void GenerateSpawnPoints()
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
                    spawnPoints.Add(hit.point);
                } else {
                    Debug.DrawRay(randomPoint, Vector3.down * maxRaycastDistance, Color.red, 1f);
                }
            }
            tries++;
        }
    }

    bool IsPreferredLocation(RaycastHit hit, float slopeAngle)
    {
        // Check height oh the hit point
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
            
            // more its green, more it's a good spot
            if (averageColorVector.y < 0.75) return false;
            return true;
        }
        return false;
    }

    bool IsNotTooClose(Vector3 position)
    {
        foreach (Vector3 point in spawnPoints)
        {
            if (Vector3.Distance(point, position) < minDistance)
                return false;
        }
        return true;
    }

    void PlaceTrees()
    {
        foreach (Vector3 spawnPoint in spawnPoints)
        {
            var treePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
            GameObject tree = Instantiate(treePrefab, spawnPoint, Quaternion.identity);

            // Randomize scale
            float randomScale = Random.Range(0.8f, 1.2f);
            tree.transform.localScale *= randomScale;

            // Randomize rotation
            tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        }
    }
}
