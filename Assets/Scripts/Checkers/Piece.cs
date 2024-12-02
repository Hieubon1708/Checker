using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Piece : MonoBehaviour
{

    public bool isBlue;
    public bool isKing;

    // kiem tra co su kien an xay ra khong?
    public bool IsForceToMove(Piece[,] board, int x, int y)
    {
        if (isBlue || isKing)
        {
            //top right
            if (x <= 7 && y >= 2)
            {
                Piece p = board[x + 1, y - 1];
                //if there is a piece and it is not the same color as ours
                if (p != null && p.isBlue != isBlue)
                {
                    // check if its possible to land after the jump
                    if (board[x + 2, y - 2] == null)
                        return true;
                }
            }

            // down right
            if (x <= 7 && y <= 7)
            {
                Piece p = board[x + 1, y + 1];
                //if there is a piece and it is not the same color as ours
                if (p != null && p.isBlue != isBlue)
                {
                    // check if its possible to land after the jump
                    if (board[x + 2, y + 2] == null)
                        return true;
                }
            }
        }
        if (!isBlue || isKing)
        {
            //top left
            if (x >= 2 && y >= 2)
            {
                Piece p = board[x - 1, y - 1];
                //if there is a piece and it is not the same color as ours
                if (p != null && p.isBlue != isBlue)
                {
                    // check if its possible to land after the jump
                    if (board[x - 2, y - 2] == null)
                        return true;
                }
            }

            // down left
            if (x >= 2 && y <= 7)
            {
                Piece p = board[x - 1, y + 1];
                //if there is a piece and it is not the same color as ours
                if (p != null && p.isBlue != isBlue)
                {
                    // check if its possible to land after the jump
                    if (board[x - 2, y + 2] == null)
                        return true;
                }
            }
        }
        return false;
    }

    // kiem tra xem piece co di chuyen dc khong?
    public bool CantMovePiece(Piece[,] board, int x, int y)
    {
        if (isBlue || isKing)
        {
            //top right
            if (x <= 8 && y >= 1)
            {
                Piece p = board[x + 1, y - 1];
                if (p == null)
                    return true;
                if (x <= 7 && y >= 2)
                    if (y >= 2)
                        if (p != null)
                        {
                            if (p.isBlue != isBlue)
                                if (board[x + 2, y - 2] == null)
                                    return true;
                        }

            }
            // down right
            if (x <= 8 && y <= 8)
            {
                Piece p = board[x + 1, y + 1];
                if (p == null)
                    return true;
                if (x <= 7 && y <= 7)
                    if (p != null)
                    {
                        if (p.isBlue != isBlue)
                            if (board[x + 2, y + 2] == null)
                                return true;
                    }

            }

        }
        if (!isBlue || isKing)
        {
            //top left
            if (x >= 1 && y >= 1)
            {
                Piece p = board[x - 1, y - 1];
                if (p == null)
                    return true;
                if (x >= 2 && y >= 2)
                    if (p != null)
                    {
                        if (p.isBlue != isBlue)
                            if (board[x - 2, y - 2] == null)
                                return true;
                    }

            }
            // down left
            if (x >= 1 && y <= 7)
            {
                Piece p = board[x - 1, y + 1];
                if (p == null)
                    return true;
                if (x >= 2)
                    if (p != null)
                    {
                        if (p.isBlue != isBlue)
                            if (board[x - 2, y + 2] == null)
                                return true;
                    }

            }
        }
        return false;
    }

    // di chuyen piece va an piece neu co the
    public bool ValidMove(Piece[,] board, int x1, int y1, int x2, int y2)
    {
        if (board[x2, y2] != null)
        {
            return false;
        }
        int deltaMoveX = x2 - x1;
        int deltaMoveY = Mathf.Abs(y2 - y1);

        if (isBlue || isKing)
        {
            if (deltaMoveY == 1)
            {
                if (deltaMoveX == 1)
                    return true;
            }

            if (deltaMoveY == 2)
            {
                if (deltaMoveX == 2)
                {
                    Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isBlue != isBlue)
                        return true;
                }
            }
        }

        if (!isBlue || isKing)
        {
            if (deltaMoveY == 1)
            {
                if (deltaMoveX == -1)
                    return true;
            }

            if (deltaMoveY == 2)
            {
                if (deltaMoveX == -2)
                {
                    Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isBlue != isBlue)
                        return true;
                }
            }
        }
        return false;
    }
}

