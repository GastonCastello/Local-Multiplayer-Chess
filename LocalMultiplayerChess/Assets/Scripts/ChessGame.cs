using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fusion;

public class ChessGame : NetworkBehaviour
{
    public enum GameState { Start, Play, Finished}

    [SerializeField] BoardLayout startingBoardLayout;
    PieceCreator pieceCreator;
    [SerializeField] Board board;
    [SerializeField] ChessUIManager UIManager;

    [HideInInspector] public ChessPlayer whitePlayer;
    [HideInInspector] public ChessPlayer blackPlayer;
    [HideInInspector] public ChessPlayer activePlayer;

    public static ChessGame instance;

    public GameState state;

    private void Awake()
    {
        instance = this;
        pieceCreator = GetComponent<PieceCreator>();
        CreatePlayers();
    }

    void CreatePlayers()
    {
        whitePlayer = new ChessPlayer(TeamColor.White, board);
        blackPlayer = new ChessPlayer(TeamColor.Black, board);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_StartNewGame()
    {
        UIManager.HideUI();
        RPC_SetState(GameState.Start);
        board.SetController(this);
        CreatePiecesFromLayout(startingBoardLayout);        
        activePlayer = whitePlayer;
        GenerateAllPossiblePlayerMoves(activePlayer);
        RPC_SetState(GameState.Play);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RestartGame()
    {
        DestroyPieces();
        board.OnGameRestarted();
        whitePlayer.OnGameRestarted();
        blackPlayer.OnGameRestarted();
        RPC_StartNewGame();
    }

    private void DestroyPieces()
    {
        whitePlayer.activePieces.ForEach(x => Destroy(x.gameObject));
        blackPlayer.activePieces.ForEach(x => Destroy(x.gameObject));
    }

    public bool IsGameInProgress()
    {
        return state == GameState.Play;
    }

    void CreatePiecesFromLayout(BoardLayout startingBoard)
    {
        for (int i = 0; i < startingBoard.GetPiecesCount(); i++)
        {
            Vector2Int squareCoords = startingBoard.GetSquareCoordsAtIndex(i);
            TeamColor team = startingBoard.GetSquareTeamColorAtIndex(i);
            string typeName = startingBoard.GetSquarePieceNameAtIndex(i);

            Type type = Type.GetType(typeName);
            CreatePieceAndInitialize(squareCoords, team, type);
        }
    }

    public void CreatePieceAndInitialize(Vector2Int coords, TeamColor teamColor, Type type)
    {
        Piece newPiece = pieceCreator.CreatePiece(type).GetComponent<Piece>();
        newPiece.SetData(coords, teamColor, board);

        Material teamMaterial = pieceCreator.TeamMaterial(teamColor);
        newPiece.SetMaterial(teamMaterial);

        board.SetPieceOnBoard(coords, newPiece);

        ChessPlayer currentPlayer = teamColor == TeamColor.White ? whitePlayer : blackPlayer;
        currentPlayer.AddPiece(newPiece);
    }


    private void GenerateAllPossiblePlayerMoves(ChessPlayer player)
    {
        player.GenerateAllPossibleMoves();
    }

    public bool IsTeamTurnActive(TeamColor team)
    {
        return activePlayer.team == team;
    }

    public void EndTurn()
    {
        GenerateAllPossiblePlayerMoves(activePlayer);
        GenerateAllPossiblePlayerMoves(GetOpponentToPlayer(activePlayer));
        if (CheckIfGameIsFinished())
        {
            EndGame();
        }            
        else
            ChangeActiveTeam();
    }

    private void EndGame()
    {
        RPC_SetState(GameState.Finished);
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_SetState(GameState newState)
    {
        state = newState;
    }

    private bool CheckIfGameIsFinished()
    {
        Piece[] kingAttackingPieces = activePlayer.GetPiecesAttackingOppositePieceOfType<King>();
        if(kingAttackingPieces.Length > 0)
        {
            ChessPlayer oppositePlayer = GetOpponentToPlayer(activePlayer);
            Piece attackedKing = oppositePlayer.GetPiecesOfType<King>().FirstOrDefault();
            oppositePlayer.RemoveMovesEnablingAttackingOnPiece<King>(activePlayer, attackedKing);

            int availableKingMoves = attackedKing.availableMoves.Count;
            if(availableKingMoves == 0)
            {
                bool canCoverKing = oppositePlayer.CanHidePieceFromAttack<King>(activePlayer);
                if (!canCoverKing)
                {
                    UIManager.OnGameFinished(activePlayer);
                    return true;
                }                    
            }
        }
        return false;
    }

    public void OnPieceRemoved(Piece piece)
    {
        ChessPlayer pieceOwner = (piece.team == TeamColor.White) ? whitePlayer : blackPlayer;
        pieceOwner.RemovePiece(piece);
        Destroy(piece.gameObject);
    }

    private ChessPlayer GetOpponentToPlayer(ChessPlayer player)
    {
        return player == whitePlayer ? blackPlayer : whitePlayer;
    }

    private void ChangeActiveTeam()
    {
        activePlayer = activePlayer == whitePlayer ? blackPlayer : whitePlayer;
    }

    public void RemoveMovesEnablingAttackOnPieceOfType<T>(Piece piece) where T : Piece
    {
        activePlayer.RemoveMovesEnablingAttackingOnPiece<T>(GetOpponentToPlayer(activePlayer), piece);
    }
}
