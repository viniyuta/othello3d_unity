using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class Direction
{
    public readonly static Direction North = new Direction(0, -1);
    public readonly static Direction South = new Direction(0, 1);
    public readonly static Direction East = new Direction(1, 0);
    public readonly static Direction West = new Direction(-1, 0);
    public readonly static Direction[] StraightDirections = new Direction[] {North, South, East, West};
    public readonly static Direction NorthEast = North + East;
    public readonly static Direction NorthWest = North + West;
    public readonly static Direction SouthEast = South + East;
    public readonly static Direction SouthWest = South + West;
    public readonly static Direction[] DiagonalDirections = new Direction[] {NorthEast, NorthWest, SouthEast, SouthWest};
    public readonly static Direction[] AllDirections = StraightDirections.Concat(DiagonalDirections).ToArray();

    
    public int ColDelta { get; }
    public int RowDelta { get; }

    public Direction(int colDelta, int rowDelta)
    {
        ColDelta = colDelta;
        RowDelta = rowDelta;
    }

    public static Direction operator +(Direction dir1, Direction dir2)
    {
        return new Direction(dir1.ColDelta + dir2.ColDelta, dir1.RowDelta + dir2.RowDelta);
    }

    public static Direction operator *(int scalar, Direction dir)
    {
        return new Direction(scalar * dir.ColDelta, scalar * dir.RowDelta);
    }
}