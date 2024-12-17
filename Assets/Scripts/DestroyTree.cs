using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTree : MonoBehaviour
{
    public GameObject trunk;
    void Start()
    {
        if (trunk != null)
            Destroy(trunk, 4);
        else 
            Debug.Log("Le tron est indéfini");
    }
}
