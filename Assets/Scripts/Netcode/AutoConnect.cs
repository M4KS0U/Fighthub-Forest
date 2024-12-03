using UnityEngine;
using Unity.Netcode.Transports.UTP;

using Unity.Netcode;

public class AutoConnect : MonoBehaviour
{
    [Header("Network Settings")]
    public bool isServer = true;
    public string clientIPAddress = "127.0.0.1";
    public ushort port = 7777;

    private NetData netData;

    private void Start()
    {
        netData = NetData.instance;
        if (netData != null)
        {
            isServer = netData.isServer;
            clientIPAddress = netData.ip;
        }
        if (isServer)
        {
            StartServer();
        }
        else
        {
            StartClient();
        }
    }

    private void StartServer()
    {
        Debug.Log("Starting server...");
        NetworkManager.Singleton.StartHost();
    }

    private void StartClient()
    {
        Debug.Log($"Starting client and connecting to {clientIPAddress}:{port}...");

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        if (transport == null)
        {
            Debug.LogError("UnityTransport component not found on NetworkManager.");
            return;
        }

        // Configuration de l'adresse IP et du port
        transport.SetConnectionData(clientIPAddress, port);

        NetworkManager.Singleton.StartClient();
    }
}
