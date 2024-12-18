using Unity.Netcode;
using UnityEngine;

public class PauseHandling : NetworkBehaviour
{
    private bool isPaused = false;
    private NetworkVariable<bool> isPausedNetworked = new NetworkVariable<bool>(true);

    public NetworkVariable<bool> isGameStart = new NetworkVariable<bool>(false);

    public GameObject EndGameUi;
    public GameObject lobbyUi;


    void Start()
    {
    }

    public void StartGame()
    {
        isPausedNetworked.Value = !isPausedNetworked.Value;
        isGameStart.Value = true;
    }

    public void EndGame()
    {
        isPausedNetworked.Value = !isPausedNetworked.Value;
        SetPause(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && IsHost)
        {
            isPausedNetworked.Value = !isPausedNetworked.Value;
            TogglePause();
        }
        if (isPausedNetworked.Value != isPaused)
        {
            SetPause(isPausedNetworked.Value);
        }
    }

    public void SetPause(bool value)
    {
        isPaused = value;

        if (isPaused)
        {
            Time.timeScale = 0;
            if (isGameStart.Value == false)
            {
                lobbyUi.SetActive(true);
            } else
                EndGameUi.SetActive(true);
        }
        else
        {
            isGameStart.Value = true;
            Time.timeScale = 1;
            lobbyUi.SetActive(false);
            EndGameUi.SetActive(false);
        }
    }

    public void TogglePause()
    {
        SetPause(!isPaused);
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
