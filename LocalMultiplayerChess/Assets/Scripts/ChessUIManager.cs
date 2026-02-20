using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class ChessUIManager : NetworkBehaviour
{ 
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject gameOverRestart;
    [SerializeField] GameObject teamSelectionScreen;

    [SerializeField] TextMeshProUGUI resultText;

    [SerializeField] Button whiteTeamButton;
    [SerializeField] Button blackTeamButton;

    private void Awake()
    {
        OnGameLaunched();
    }

    private void OnGameLaunched()
    {
        DisableAllScreens();
    }

    public void OnGameFinished(ChessPlayer winner)
    {
        DisableAllScreens();
        gameOverScreen.SetActive(true);
        if(HasStateAuthority) gameOverRestart.SetActive(true);
        if (winner == ChessGame.instance.whitePlayer)
        {
            resultText.text = "White Won";
        }
        else
        {
            resultText.text = "Black Won";
        }
    }

    void DisableAllScreens()
    {
        gameOverScreen.SetActive(false);
        gameOverRestart.SetActive(false);
        teamSelectionScreen.SetActive(false);
    }

    public void HideUI()
    {
        DisableAllScreens();
    }

    public void OnConnected()
    {
        DisableAllScreens();
        //teamSelectionScreen.SetActive(true);
    }
}
