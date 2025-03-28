using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Board
{
    public static readonly int[] COLS = {8, 6, 4};
    public static readonly int[] ROWS = {8, 6, 4};
    public static readonly int FLOORS = 3;

    public readonly List<Position> AllPositions = new();

    private readonly List<Player[,]> board = new()
    {
        new Player[COLS[0], ROWS[0]],
        new Player[COLS[1], ROWS[1]],
        new Player[COLS[2], ROWS[2]]
    };

    // ボード上で次の階層が置かれている位置
    public static readonly Position[] floorPositions = 
    {
        new Position(1, 1, 0),
        new Position(1, 1, 1)
    };

    public Player this[int col, int row, int floor]
    {
        get { return board[floor][col, row]; }
        set { board[floor][col, row] = value; }
    }

    public Player this[Position pos]
    {
        get { return this[pos.Col, pos.Row, pos.Floor]; }
        set { this[pos.Col, pos.Row, pos.Floor] = value; }
    }

    public static Board Initial()
    {
        Board board = new();
        board.AddStartPieces();
        board.CreateAllPositions();
        return board;
    }

    private void AddStartPieces()
    {
        this[1, 1, 2] = Player.White;
        this[1, 2, 2] = Player.Black;
        this[2, 1, 2] = Player.Black;
        this[2, 2, 2] = Player.White;

        for (int floor = 1; floor < FLOORS; floor++)
        {
            for (int col = 0; col < COLS[floor]; col++)
            {
                for (int row = 0; row < ROWS[floor]; row++)
                {
                    if (IsFloorCorner(col, row, floor))
                    {
                        this[col, row, floor] = Player.NotPlayable;
                    }
                }
            }
        }
    }

    private void CreateAllPositions()
    {
        for (int floor = 0; floor < FLOORS; floor++)
        {
            for (int col = 0; col < COLS[floor]; col++)
            {
                for (int row = 0; row < ROWS[floor]; row++)
                { 
                    if (!IsNextFloorPlaced(col, row, floor) && this[col, row, floor] != Player.NotPlayable)
                    {
                        AllPositions.Add(new Position(col, row, floor));
                    }
                }
            }
        }
    }

    private bool IsNextFloorPlaced(int col, int row, int floor)
    {
        if (floor == FLOORS - 1) { return false; }
        int floorCol = floorPositions[floor].Col;
        int floorRow = floorPositions[floor].Row;
        int nextFloor = floor + 1;

        return 
            col > floorCol &&
            row > floorRow &&
            col < floorCol + COLS[nextFloor] - 1 &&
            row < floorRow + ROWS[nextFloor] - 1;
    }

    public static bool IsInside(Position pos)
    {
        int floorCol = COLS[pos.Floor];
        int floorRow = ROWS[pos.Floor];
        return 
            pos.Col >= 0 &&
            pos.Col < floorCol &&
            pos.Row >= 0 &&
            pos.Row < floorRow;
    }

    public bool IsEmpty(Position pos)
    {
        return this[pos] == Player.None;
    }

    public static bool IsFloorCorner(int col, int row, int floor)
    {
        int floorCols = COLS[floor];
        int floorRows = ROWS[floor];
        if (col == 0 || col == floorCols - 1)
        {
            if (row == 0 || row == floorRows - 1)
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsFloorCorner(Position pos)
    {
        return IsFloorCorner(pos.Col, pos.Row, pos.Floor);
    }

    public static bool IsFloorBorder(int col, int row, int floor)
    {
        if (floor == 0) { return false; }
        if (col == 0 || col == COLS[floor] - 1 || row == 0 || row == ROWS[floor] - 1) { return true; }
        return false;
    }

    public static bool IsFloorBorder(Position pos)
    {
        return IsFloorBorder(pos.Col, pos.Row, pos.Floor);
    }
}
