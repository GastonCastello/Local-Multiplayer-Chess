using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(-1,1),
        new Vector2Int(0,1),
        new Vector2Int(1,1),
        new Vector2Int(-1,0),
        new Vector2Int(1,0),
        new Vector2Int(-1,-1),
        new Vector2Int(0,-1),
        new Vector2Int(1,-1)
    };

    Vector2Int leftCastling;
    Vector2Int rightCastling;
    Piece leftRook;
    Piece rightRook;

    public override List<Vector2Int> SelectAvailableSquares()
    {
        availableMoves.Clear();
        AssignStandardMoves();
        AssignCastlingMoves();
        return availableMoves;
    }

    void AssignStandardMoves()
    {
        float range = 1;
        foreach (var item in directions)
        {
            for (int i = 1; i <= range; i++)
            {
                Vector2Int nextCoords = occupiedSquare + item * i;
                Piece piece = board.GetPieceOnSquare(nextCoords);

                if (!board.CheckIfCoordsAreOnBoard(nextCoords))
                    break;
                if (piece == null)
                    TryToAddMove(nextCoords);
                else if (!piece.isFromSameTeam(this))
                {
                    TryToAddMove(nextCoords);
                    break;
                }
                else if (piece.isFromSameTeam(this))
                    break;
            }
        }
    }

    void AssignCastlingMoves()
    {
        if (hasMoved)
            return;

        leftRook = GetPieceInDirection<Rook>(team, Vector2Int.left);

        if(leftRook && !leftRook.hasMoved)
        {
            leftCastling = occupiedSquare + Vector2Int.left * 2;
            availableMoves.Add(leftCastling);
        }

        rightRook = GetPieceInDirection<Rook>(team, Vector2Int.right);

        if (rightRook && !rightRook.hasMoved)
        {
            rightCastling = occupiedSquare + Vector2Int.right * 2;
            availableMoves.Add(rightCastling);
        }
    }

    public override void MovePiece(Vector2Int coords)
    {
        base.MovePiece(coords);
        if(coords == leftCastling)
        {
            board.UpdateBoardOnPieceMove(coords + Vector2Int.right, leftRook.occupiedSquare, leftRook, null);
            leftRook.MovePiece(coords + Vector2Int.right);
        }
        else if (coords == rightCastling)
        {
            board.UpdateBoardOnPieceMove(coords + Vector2Int.left, rightRook.occupiedSquare, rightRook, null);
            rightRook.MovePiece(coords + Vector2Int.left);
        }
    }
}
