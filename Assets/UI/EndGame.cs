using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    private PauseHandling pauseHandling;
    private GameManager gameManager;
    void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        pauseHandling = gameObject.GetComponent<PauseHandling>();
    }

    int playerCount()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // remove dead players
        for (int i = 0; i < players.Length; i++)
        {
            if (!players[i].GetComponent<Enemy>().isAlive)
            {
                players[i] = null;
            }
        }

        int count = 0;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                count++;
            }
        }
        return count;
    }

    // Update is called once per frame
    void Update()
    {
        if (!pauseHandling.IsPaused() && playerCount() == 1 && pauseHandling.isGameStart.Value)
        {
            pauseHandling.EndGame();
        }
        // check input r
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    public void Restart()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<Enemy>().Revive();
        }

        // replace the cylinder
        GameObject cylinder = GameObject.FindGameObjectWithTag("BorderLimit");
        cylinder.transform.localScale = new Vector3(200, 200, 200);
        pauseHandling.StartGame();
    }
}
