using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ReduceZone : NetworkBehaviour
{
    public float MinScale = 0.5f;
    public float speed = 0.5f;

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
        {
            return;
        }
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Max(MinScale, scale.x - speed * Time.deltaTime);
        scale.z = Mathf.Max(MinScale, scale.z - speed * Time.deltaTime);
        transform.localScale = scale;
    }
}
