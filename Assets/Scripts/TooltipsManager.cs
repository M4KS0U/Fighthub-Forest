    using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; set; }

    public bool onTarget;
    public TMP_Text tooltipText;
    public Vector3 offset = new Vector3(10, 10, 0);

    private void Start()
    {
        onTarget = false;
        tooltipText.gameObject.SetActive(false);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //if (InventorySystem.Instance.isOpen == false)
        //{
            if (Physics.Raycast(ray, out hit))
            {
                Object objectName = hit.collider.GetComponent<Object>();
                if (objectName != null)
                {
                    onTarget = true;

                    tooltipText.gameObject.SetActive(true);
                    tooltipText.text = objectName.GetInfo();

                    Vector3 tooltipPosition = Input.mousePosition + offset; // Prendre la position de l'objet (collider.gameObject.position / collider.position)
                    tooltipText.transform.position = tooltipPosition;
                }
                else
                {
                    onTarget = false;
                    tooltipText.gameObject.SetActive(false);
                }
            }
            else
            {
                tooltipText.gameObject.SetActive(false);
            }
        //}
    }
}
