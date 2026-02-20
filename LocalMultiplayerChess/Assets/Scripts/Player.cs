using Fusion;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Player : NetworkBehaviour
{
    bool gameStarted = false;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            GameManager.instance.startingCam.SetActive(false);
            GameManager.instance.camWhite.SetActive(true);
        }
        else
        {
            GameManager.instance.startingCam.SetActive(false);
            GameManager.instance.camBlack.SetActive(true);
        }
    }

    private void Update()
    {
        if (Runner.ActivePlayers.ToList().Count >= 2 && HasStateAuthority && !gameStarted) 
        {
            gameStarted = true;
            StartCoroutine(StartGame(3f));
        }

        if (HasStateAuthority && gameStarted && Input.GetKeyDown(KeyCode.R)) ChessGame.instance.RPC_RestartGame();     
    }

    IEnumerator StartGame(float secs)
    {
        yield return new WaitForSeconds(secs);
        ChessGame.instance.RPC_StartNewGame();
        ChessGame.instance.RPC_RestartGame();
    }
}
