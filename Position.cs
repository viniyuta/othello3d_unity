public class Position
{
    public int Col { get; }
    public int Row { get; }

    public Position (int col, int row)
    {
        Col = col;
        Row = row;
    }

    public override bool Equals(object obj)
    {
        if (obj is Position other) { return Col == other.Col && Row == other.Row; }
        return false;
    }

    public override int GetHashCode()
    {
        return 8 * Col + Row;
    }

    public static Position operator +(Position pos, Direction dir)
    {
        return new Position(pos.Col + dir.ColDelta, pos.Row + dir.RowDelta);
    }
}
