using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : NetworkBehaviour
{
    public const int boardSize = 8;

    [SerializeField] Transform bottomLeftSquareTransform;
    [SerializeField] float squareSize;

    Piece[,] grid;
    Piece selectedPiece;
    ChessGame chessController;
    SquareSelectorCreator squareSelector;

    protected virtual void Awake()
    {
        squareSelector = GetComponent<SquareSelectorCreator>();
        CreateGrid();
    }

    public void SetController(ChessGame chessController)
    {
        this.chessController = chessController;
    }

    void CreateGrid()
    {
        grid = new Piece[boardSize, boardSize];
    }

    public Vector3 CalculatePositionFromCoords(Vector2Int coords)
    {
        return bottomLeftSquareTransform.position + new Vector3(coords.x * squareSize, 0f, coords.y * squareSize);
    }

    public Vector2Int CalculateCoordsFromPosition(Vector3 inputPos)
    {
        int x = Mathf.FloorToInt(inputPos.x / squareSize) + boardSize / 2;
        int y = Mathf.FloorToInt(inputPos.z / squareSize) + boardSize / 2;
        return new Vector2Int(x-1, y); // error raro solucion rara
    }

    public void OnGameRestarted()
    {
        selectedPiece = null;
        CreateGrid();
    }

    public void OnSquareSelected(Vector3 inputPosition)
    {
        if (!chessController.IsGameInProgress())
            return;

        Vector2Int coords = CalculateCoordsFromPosition(inputPosition);
        Piece piece = GetPieceOnSquare(coords);

        if (ChessGame.instance.IsTeamTurnActive(TeamColor.White) && !HasStateAuthority) return;
        if (ChessGame.instance.IsTeamTurnActive(TeamColor.Black) && HasStateAuthority) return;

        if (selectedPiece)
        {
            if (piece != null && selectedPiece == piece)
                DeselectPiece();
            else if (piece != null && selectedPiece != piece && chessController.IsTeamTurnActive(piece.team))
                SelectPiece(coords);
            else if (selectedPiece.CanMoveTo(coords))
                RPC_SelectPieceMoved(coords);
        }
        else
        {
            if (piece != null && chessController.IsTeamTurnActive(piece.team))
                SelectPiece(coords);
        }
    }

    public void PromotePiece(Piece piece)
    {
        TakePiece(piece);
        chessController.CreatePieceAndInitialize(piece.occupiedSquare, piece.team, typeof(Queen)); // automatic queen
    }

    private void SelectPiece(Vector2Int coords)
    {
        Piece piece = GetPieceOnSquare(coords);
        chessController.RemoveMovesEnablingAttackOnPieceOfType<King>(piece);
        RPC_SetSelectedPiece(coords);
        List<Vector2Int> selection = selectedPiece.availableMoves;
        ShowSelectionSquares(selection);
    }

    private void ShowSelectionSquares(List<Vector2Int> selection)
    {
        Dictionary<Vector3, bool> squaresData = new Dictionary<Vector3, bool>();
        for (int i = 0; i < selection.Count; i++)
        {
            Vector3 pos = CalculatePositionFromCoords(selection[i]);
            bool isSquareFree = GetPieceOnSquare(selection[i]) == null;
            squaresData.Add(pos, isSquareFree);
        }
        squareSelector.ShowSelection(squaresData);
    }

    private void DeselectPiece()
    {
        selectedPiece = null;
        squareSelector.ClearSelection();
    }

    public void OnSelectedPieceMoved(Vector2Int coords)
    {
        TryToTakeOppositePiece(coords);
        UpdateBoardOnPieceMove(coords, selectedPiece.occupiedSquare, selectedPiece, null);
        selectedPiece.MovePiece(coords);
        DeselectPiece();
        EndTurn();
    }


    public void OnSetSelectedPieceMoved(Vector2Int intCoords)
    {
        Piece piece = GetPieceOnSquare(intCoords);
        selectedPiece = piece;
    }

    private void TryToTakeOppositePiece(Vector2Int coords)
    {
        Piece piece = GetPieceOnSquare(coords);
        if (piece != null && !selectedPiece.isFromSameTeam(piece))
            TakePiece(piece);
    }

    private void TakePiece(Piece piece)
    {
        if (piece)
        {
            grid[piece.occupiedSquare.x, piece.occupiedSquare.y] = null;
            chessController.OnPieceRemoved(piece);
        }
    }

    public void UpdateBoardOnPieceMove(Vector2Int newCoords, Vector2Int oldCoords, Piece newPiece, Piece oldPiece)
    {
        grid[oldCoords.x, oldCoords.y] = oldPiece;
        grid[newCoords.x, newCoords.y] = newPiece;
    }

    private void EndTurn()
    {
        chessController.EndTurn();
    }

    public Piece GetPieceOnSquare(Vector2Int coords)
    {
        if (CheckIfCoordsAreOnBoard(coords))
            return grid[coords.x, coords.y];
        return null;
    }

    public bool CheckIfCoordsAreOnBoard(Vector2Int coords)
    {
        if (coords.x < 0 || coords.y < 0 || coords.x >= boardSize || coords.y >= boardSize)
            return false;
        return true;
    }

    public void SetPieceOnBoard(Vector2Int coords, Piece newPiece)
    {
        if (CheckIfCoordsAreOnBoard(coords))
            grid[coords.x, coords.y] = newPiece;
    }

    public bool HasPiece(Piece piece)
    {
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                if (grid[i, j] == piece)
                    return true;
            }
        }
        return false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SelectPieceMoved(Vector2 coords)
    {
        Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
        OnSelectedPieceMoved(intCoords);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetSelectedPiece(Vector2 coords)
    {
        Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
        OnSetSelectedPieceMoved(intCoords);
    }
}
