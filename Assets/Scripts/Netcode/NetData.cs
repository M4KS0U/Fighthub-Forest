using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetData : MonoBehaviour
{
    public static NetData instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public bool isServer = false;
    public int port = 7777;
    public string ip = "0.0.0.0";

    public TMPro.TextMeshProUGUI ipText;

    public void SetServer()
    {
        isServer = true;
    }

    public void SetClient()
    {
        isServer = false;
    }

    public void GetIpFromInput()
    {
        ip = ipText.text;
    }

    public void SetIp(string newIp)
    {
        ip = newIp;
    }
}
