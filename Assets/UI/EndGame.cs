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
        return players.Length;
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
            gameManager.restart();
        }
    }
}
