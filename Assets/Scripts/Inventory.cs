using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int numberObject = 0;
    public string collectible;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == collectible)
        {
            numberObject++;
            Debug.Log(numberObject);
            Destroy(other.gameObject);
        }
    }
}
