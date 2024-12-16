using UnityEngine;

public class TreeChopping : MonoBehaviour
{
    public GameObject TreeChopped;
    public float fallForce = 5f;
    public int treeHealth = 100;

    void OnMouseDown()
    {
        if (TreeChopped != null)
        {
            treeHealth -= 50;

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
        }
        else
        {
            Debug.LogWarning("Aucun objet n'est assigné pour l'instanciation !");
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
