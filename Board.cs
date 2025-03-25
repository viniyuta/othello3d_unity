using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public static readonly int COLS = 8;
    public static readonly int ROWS = 8;
    private readonly Player[,] board = new Player[COLS, ROWS];

    public Player this[int col, int row]
    {
        get { return board[col, row]; }
        set { board[col, row] = value; }
    }

    public Player this[Position pos]
    {
        get { return this[pos.Col, pos.Row]; }
        set { this[pos.Col, pos.Row] = value; }
    }

    public static Board Initial()
    {
        Board board = new();
        board.AddStartPieces();
        return board;
    }

    private void AddStartPieces()
    {
        this[3, 3] = Player.White;
        this[3, 4] = Player.Black;
        this[4, 3] = Player.Black;
        this[4, 4] = Player.White;
    }

    public bool IsEmpty(Position pos)
    {
        return this[pos] == Player.None;
    }

}
