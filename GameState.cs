using System.Collections.Generic;

public class GameState
{
    private readonly int[] COLS = Board.COLS;
    private readonly int[] ROWS = Board.ROWS;
    private readonly int FLOORS = Board.FLOORS;

    public Board Board { get; }
    public Dictionary<Player, int> DiscCount { get; }
    public Player CurrentPlayer { get; private set; }
    public bool GameOver { get; private set; }
    public Player Winner { get; private set; }
    public Dictionary<Position, List<Position>> LegalMoves { get; private set; }

    public GameState()
    {
        Board = Board.Initial();

        DiscCount = new Dictionary<Player, int>()
        {
            { Player.Black, 2},
            { Player.White, 2}
        };

        CurrentPlayer = Player.Black;
        LegalMoves = FindLegalMoves(CurrentPlayer);
    }

    public bool MakeMove(Position pos, out MoveInfo moveInfo)
    {
        if (!LegalMoves.ContainsKey(pos))
        {
            moveInfo = null;
            return false;
        }
        
        Player movePlayer = CurrentPlayer;
        List<Position> outflanked = LegalMoves[pos];

        Board[pos] = movePlayer;
        FlipDiscs(outflanked);
        UpdateDiscCounts(movePlayer, outflanked.Count);
        PassTurn();

        moveInfo = new MoveInfo { Player = movePlayer, Position = pos, Outflanked = outflanked};

        return true;
    }

    public IEnumerable<Position> OccupiedPositions()
    {
        foreach (Position pos in Board.AllPositions)
        {
            if (!Board.IsEmpty(pos))
            {
                yield return pos;
            }
        }
    }

    private void FlipDiscs(List<Position> positions)
    {
        foreach(Position pos in positions)
        {
            Board[pos] = Board[pos].Opponent();
        }
    }

    private void UpdateDiscCounts(Player movePlayer, int outflankedCount)
    {
        DiscCount[movePlayer] += outflankedCount + 1;
        DiscCount[movePlayer.Opponent()] -= outflankedCount;
    }

    private void ChangePlayer()
    {
        CurrentPlayer = CurrentPlayer.Opponent();
        LegalMoves = FindLegalMoves(CurrentPlayer);
    }

    private Player FindWinner()
    {
        if (DiscCount[Player.Black] > DiscCount[Player.White]) { return Player.Black; }
        if (DiscCount[Player.Black] < DiscCount[Player.White]) { return Player.White; }
        return Player.None;
    }

    private void PassTurn()
    {
        ChangePlayer();

        if(LegalMoves.Count > 0)
        {
            return;
        }

        ChangePlayer();  // CurrentPlayerをスキップ
        if(LegalMoves.Count == 0)  // 次のPlayerもプレイできないならGameOver処理を行う
        {
            CurrentPlayer = Player.None;
            GameOver = true;
            Winner = FindWinner();
        }
    }

    private List<Position> OutflankedInDir(Position initialPos, Player player, Direction dir)
    {
        List<Position> outflanked = new List<Position>();
        Position pos = initialPos + dir;

        while (Board.IsInside(pos) && !Board.IsEmpty(pos))
        {
            if (Board[pos] == player.Opponent())
            {
                outflanked.Add(pos);
                pos += dir;
            }
            else if (Board[pos] == Player.NotPlayable) { pos += dir; }
            else 
            {
                return outflanked;
            }
        }

        return new List<Position>();
    }

    private List<Position> Outflanked(Position pos, Player player)
    {
        List<Position> outflanked = new List<Position>();
        foreach (Direction dir in Direction.AllDirections)
        {
            outflanked.AddRange(OutflankedInDir(pos, player, dir));
        }
        return outflanked;
    }

    private bool IsMoveLegal(Player player, Position pos, out List<Position> outflanked)
    {
        if (!Board.IsEmpty(pos))
        {
            outflanked = null;
            return false;
        }

        outflanked = Outflanked(pos, player);
        return outflanked.Count > 0;
    }

    private Dictionary<Position, List<Position>> FindLegalMoves(Player player)
    {
        Dictionary<Position, List<Position>> legalMoves = new();

        foreach (Position pos in Board.AllPositions)
        {
            if (IsMoveLegal(player, pos, out List<Position> outflanked))
            {
                legalMoves[pos] = outflanked;
            }
        }

        return legalMoves;
    }
}
