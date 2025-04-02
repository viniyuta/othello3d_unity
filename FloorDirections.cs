using System.Linq;

public class FloorDirections
{
    private static readonly int[] COLS = Board.COLS;
    private static readonly int[] ROWS = Board.ROWS;
    private static readonly int FLOORS = Board.FLOORS;

    private static readonly Position[] floorPositions = Board.floorPositions;

    // 上の階層へ行くべきかを判断するメソッド
    private static bool IsNextFloor(Position pos)
    {
        if (pos.Floor >= FLOORS - 1) { return false; }

        Position nextFloorPosition = floorPositions[pos.Floor];
        int nextFloorCols = COLS[pos.Floor + 1];
        int nextFloorRows = ROWS[pos.Floor + 1];
        
        return 
            pos.Col > nextFloorPosition.Col &&
            pos.Row > nextFloorPosition.Row &&
            pos.Col < nextFloorPosition.Col + nextFloorCols - 1 &&
            pos.Row < nextFloorPosition.Row + nextFloorRows - 1;
    }

    // 下の階層へ行くべきかを判断するメソッド
    private static bool IsPreviousFloor(Position pos)
    {
        if (pos.Floor == 0) { return false; }
        if (Board.IsFloorCorner(pos)) { return true; }  // ベース階層以外で角の場合は下の階層へ移動する

        return !Board.IsInside(pos);
    }

    public static Position AddDirToPos(Position pos, Direction dir)
    {
        Position addPos = new(pos.Col + dir.ColDelta, pos.Row + dir.RowDelta, pos.Floor);

        if (IsNextFloor(addPos)) { return GoToNextFloor(pos, dir); }
        if (IsPreviousFloor(addPos)) { return GoToPreviousFloor(pos, dir); }
        return addPos;
    } 

    private static Position GoToNextFloor(Position pos, Direction dir)
    {
        Position floorPos = floorPositions[pos.Floor];
        int newCol = pos.Col - floorPos.Col;
        int newRow = pos.Row - floorPos.Row;
        int newFloor = pos.Floor + 1;

        if (Direction.DiagonalDirections.Contains(dir))
        {
            int floorCol = COLS[newFloor];
            int floorRow = ROWS[newFloor];
            if (newCol == 0 || newCol == floorCol - 1)
            {
                if (newRow == 0 || newRow == floorRow - 1)
                {
                    return new Position(newCol + dir.ColDelta, newRow + dir.RowDelta, newFloor);
                }
                newRow += dir.RowDelta;
            }
            else
            {
                newCol += dir.ColDelta;
            }
        }

        return new Position(newCol, newRow, newFloor);
    }

    private static Position GoToPreviousFloor(Position pos, Direction dir)
    {
        int newFloor = pos.Floor - 1;
        Position floorPos = floorPositions[newFloor];
        int newCol = pos.Col + floorPos.Col;
        int newRow = pos.Row + floorPos.Row;

        if (Direction.DiagonalDirections.Contains(dir))
        {
            int floorCols = COLS[pos.Floor];
            int floorRows = ROWS[pos.Floor];
            if (pos.Col == 0 || pos.Col == floorCols - 1)
            {
                if (pos.Row == 0 || pos.Row == floorRows - 1)
                {
                    return new Position(newCol + floorPos.Col, newRow + floorPos.Row, newFloor);
                }
                newRow += dir.RowDelta;
            }
            else 
            {
                newCol += dir.ColDelta;
            }
        }

        if (Board.IsFloorCorner(pos.Col + dir.ColDelta, pos.Row + dir.RowDelta, pos.Floor)) { return new Position(-1,-1,0); }
        return new Position(newCol, newRow, newFloor);
    }
}
