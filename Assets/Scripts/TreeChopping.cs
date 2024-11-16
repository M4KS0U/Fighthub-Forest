using UnityEngine;

public class TreeChopping : MonoBehaviour
{
    public GameObject TreeChopped;

    void OnMouseDown()
    {
        if (TreeChopped != null)
        {
            Vector3 spawnPosition = transform.position;
            Destroy(gameObject);
            Instantiate(TreeChopped, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Aucun objet n'est assigné pour l'instanciation !");
        }
    }
}
