using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; set; }

    public bool onTarget;
    public TMP_Text tooltipText;
    private Vector3 offset = new Vector3(0, 0.5f, 0);

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

        if (Physics.Raycast(ray, out hit))
        {
            Object objectName = hit.collider.GetComponent<Object>();
            if (objectName != null)
            {
                onTarget = true;
                tooltipText.gameObject.SetActive(true);
                tooltipText.text = objectName.GetInfo();

                offset.y = hit.collider.transform.position.y + 2.5f;

                Vector3 objectWorldPosition = hit.collider.transform.position;
                Vector3 tooltipWorldPosition = objectWorldPosition + offset;
                Vector3 tooltipScreenPosition = Camera.main.WorldToScreenPoint(tooltipWorldPosition);

                tooltipText.transform.position = tooltipScreenPosition;
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
    }
}
