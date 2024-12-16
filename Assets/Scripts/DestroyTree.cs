using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTree : MonoBehaviour
{
    public GameObject trunk;
    // Start is called before the first frame update
    void Start()
    {
        if (trunk != null)
            Destroy(trunk, 2);
        else 
            Debug.Log("Le tron est indéfini");
    }
}
