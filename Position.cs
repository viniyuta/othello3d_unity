using UnityEngine;

public class Position
{
    public int Col { get; }
    public int Row { get; }
    public int Floor { get; }

    public Position (int col, int row, int floor)
    {
        Col = col;
        Row = row;
        Floor = floor;
    }

    public override bool Equals(object obj)
    {
        if (obj is Position other) { return Col == other.Col && Row == other.Row && Floor == other.Floor; }
        return false;
    }

    public override int GetHashCode()
    {
        return 11 * Col + 3 * Row + Floor;
    }

    public static Position operator +(Position pos, Direction dir)
    {
        Position addPos = FloorDirections.AddDirToPos(pos, dir);
        return addPos;
    }
}
