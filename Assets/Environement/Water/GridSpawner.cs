using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    public GameObject prefab; // L'objet à instancier
    public int gridSize = 5; // Nombre d'objets par côté
    public float offset = 1.0f; // Espacement entre les objets

    public void SpawnGrid()
    {
        // Supprime tous les enfants de l'objet qui contient le script
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        // Calcule le point de départ pour le quadrillage
        Vector3 startPosition = transform.position - new Vector3((gridSize - 1) * offset / 2, 0, (gridSize - 1) * offset / 2);

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // Crée une nouvelle instance de l'objet
                GameObject instance = Instantiate(prefab);

                // Positionne l'objet
                instance.transform.position = startPosition + new Vector3(x * offset, 0, z * offset);

                // Définit l'objet comme enfant de l'objet qui contient le script
                instance.transform.SetParent(transform);
            }
        }
    }
}
