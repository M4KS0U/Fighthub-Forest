using UnityEngine;
using System.Collections;

public class TreeChopping : MonoBehaviour
{
    public GameObject TreeChopped;
    public float fallForce = 5f;
    public int treeHealth = 100;

    public float choppingRange = 5f;

    private Vector3 originalScale;

    private Transform playerTransform;


    public float shrinkDuration = 0.06f;
    public float growDuration = 0.06f;
    public float shrinkScale = 0.99f;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Aucun joueur trouvé avec le tag 'Player' !");
        }
        originalScale = transform.localScale;
    }

    // private void Update()
    // {
    //     if (playerTransform != null && Input.GetMouseButtonDown(0)) // Si clic gauche
    //     {
    //         CheckChoppingRange();
    //     }
    // }

    public void AnimateTree()
    {
        StartCoroutine(ShrinkAndGrow());
    }

    private IEnumerator ShrinkAndGrow()
    {
        // Étape 1 : Rétrécir
        float elapsedTime = 0f;
        while (elapsedTime < shrinkDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, originalScale * shrinkScale, elapsedTime / shrinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale * shrinkScale;

        // Étape 2 : Revenir à la taille normale
        elapsedTime = 0f;
        while (elapsedTime < growDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale * shrinkScale, originalScale, elapsedTime / growDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }

    void OnMouseDown()
    {
        if (TreeChopped != null)
        {
            if (playerTransform != null && CheckChoppingRange()) {
                treeHealth -= 50;
                AnimateTree();
                // Reduire la taille de l'arbre
                // transform.localScale = originalScale / 1.1f;
                if (treeHealth <= 0)
                {
                    Vector3 clickPosition = GetClickPosition();
                    Vector3 direction = (transform.position - clickPosition).normalized;

                    GameObject choppedTree = Instantiate(TreeChopped, transform.position, Quaternion.identity);

                    Transform trunk = choppedTree.transform.Find("Trunk");
                    if (trunk != null)
                    {
                        Rigidbody rb = trunk.GetComponent<Rigidbody>();
                        if (rb == null)
                        {
                            rb = trunk.gameObject.AddComponent<Rigidbody>();
                        }
                        rb.AddForce(direction * fallForce, ForceMode.Impulse);
                    }

                    Destroy(gameObject);
                }
            } else {
                Debug.Log ("Trop loiiin");
            }
        }
        else
        {
            Debug.LogWarning("Aucun objet n'est assign� pour l'instanciation !");
        }
    }

    private bool CheckChoppingRange()
    {
        // Vérifie la distance entre le joueur et l'arbre
        float distance = Vector3.Distance(playerTransform.position, transform.position);

        if (distance <= choppingRange)
        {
            return true;
        }
        else
        {
            Debug.Log("Le joueur est trop loin pour couper cet arbre.");
            return false;
        }
    }

    private Vector3 GetClickPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return transform.position;
    }

}
