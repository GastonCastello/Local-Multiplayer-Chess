using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChessPlayer
{
    public TeamColor team { get; set; }
    public Board board { get; set; }
    public List<Piece> activePieces { get; private set; }


    public ChessPlayer(TeamColor team, Board board)
    {
        this.board = board;
        this.team = team;
        activePieces = new List<Piece>();
    }

    public void AddPiece(Piece piece)
    {
        if (!activePieces.Contains(piece))
            activePieces.Add(piece);
    }

    public void RemovePiece(Piece piece)
    {
        if (activePieces.Contains(piece))
            activePieces.Remove(piece);
    }

    public void GenerateAllPossibleMoves()
    {
        foreach (var item in activePieces)
        {
            if (board.HasPiece(item))
                item.SelectAvailableSquares();
        }
    }

    public Piece[] GetPiecesAttackingOppositePieceOfType<T>() where T : Piece
    {
        return activePieces.Where(x => x.IsAttackingPieceOfType<T>()).ToArray();
    }

    public Piece[] GetPiecesOfType<T>() where T : Piece
    {
        return activePieces.Where(x => x is T).ToArray();
    }

    public void OnGameRestarted()
    {
        activePieces.Clear();
    }

    public void RemoveMovesEnablingAttackingOnPiece<T>(ChessPlayer opponent, Piece selectedPiece) where T : Piece
    {
        List<Vector2Int> coordsToRemove = new List<Vector2Int>();
        foreach (var item in selectedPiece.availableMoves)
        {
            Piece pieceOnSquare = board.GetPieceOnSquare(item);
            board.UpdateBoardOnPieceMove(item, selectedPiece.occupiedSquare, selectedPiece, null);
            opponent.GenerateAllPossibleMoves();
            if (opponent.CheckIfIsAttackingPiece<T>())
                coordsToRemove.Add(item);
            board.UpdateBoardOnPieceMove(selectedPiece.occupiedSquare, item, selectedPiece, pieceOnSquare);
        }
        foreach (var item in coordsToRemove)
        {
            selectedPiece.availableMoves.Remove(item);
        }
    }

    private bool CheckIfIsAttackingPiece<T>() where T : Piece
    {
        foreach (var item in activePieces)
        {
            if (board.HasPiece(item) && item.IsAttackingPieceOfType<T>())
                return true;
        }
        return false;
    }

    public bool CanHidePieceFromAttack<T>(ChessPlayer opponent) where T : Piece
    {
        foreach (var item in activePieces)
        {
            foreach (var coords in item.availableMoves)
            {
                Piece pieceOnCoords = board.GetPieceOnSquare(coords);
                board.UpdateBoardOnPieceMove(coords, item.occupiedSquare, item, null);
                opponent.GenerateAllPossibleMoves();
                if (!opponent.CheckIfIsAttackingPiece<T>())
                {
                    board.UpdateBoardOnPieceMove(item.occupiedSquare, coords, item, pieceOnCoords);
                    return true;
                }
                board.UpdateBoardOnPieceMove(item.occupiedSquare, coords, item, pieceOnCoords);
            }
        }
        return false;
    }
}
