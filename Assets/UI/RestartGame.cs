using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public void restart()
    {
        // Fonction qui redémarre la scène pour tous les joueurs
        if (!IsServer) {
            Debug.LogWarning("Only the server can restart the game");
            return;
        }
        RestartSceneServerRpc();
    }
    // Fonction qui redémarre la scène pour tous les joueurs
    [ServerRpc(RequireOwnership = false)]
    public void RestartSceneServerRpc()
    {
        // Redémarre la scène côté serveur
        RestartScene();

        // Appel à une fonction côté client pour redémarrer la scène
        RestartSceneClientRpc();
    }

    // Fonction qui redémarre la scène côté client pour chaque joueur
    [ClientRpc]
    public void RestartSceneClientRpc()
    {
        // Assurez-vous que la fonction de redémarrage est appelée sur tous les clients
        if (IsOwner) // Vous pouvez aussi mettre une condition spécifique pour éviter de redémarrer plusieurs fois
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // Fonction commune qui redémarre la scène
    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
