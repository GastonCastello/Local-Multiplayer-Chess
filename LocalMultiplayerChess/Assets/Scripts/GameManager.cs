using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManager : MonoBehaviour
{
    [SerializeField] ChessUIManager chessUI;

    public GameObject startingCam;
    public GameObject camWhite;
    public GameObject camBlack;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

}
