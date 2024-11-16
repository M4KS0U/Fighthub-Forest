using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchObject : MonoBehaviour
{
    private Camera myCamera;
    private Ray ray;
    private RaycastHit hit;

    void Start()
    {
        myCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleRayCast(Input.mousePosition);
    }

    void HandleRayCast(Vector3 myInput)
    {
        ray = myCamera.ScreenPointToRay(myInput);

        if (Physics.Raycast(ray, out hit)) 
        {
            Debug.Log("hit " + hit.transform.name);
        }
    }
}
