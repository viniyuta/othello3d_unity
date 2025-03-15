using System;
using System.Collections.Generic;

public class GameState
{
    public const int COLS = 8;
    public const int ROWS = 8;

    public Player[,] Board { get; }
    public Dictionary<Player, int> DiscCount { get; }
    public Player CurrentPlayer { get; private set; }
    public bool GameOver { get; private set; }
    public Player Winner { get; private set; }
    public Dictionary<Position, List<Position>> LegalMoves { get; private set; }

    public GameState()
    {
        Board = new Player[COLS, ROWS];
        Board[3,3] = Player.White;
        Board[3,4] = Player.Black;
        Board[4,3] = Player.Black;
        Board[4,4] = Player.White;

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

        Board[pos.Col, pos.Row] = movePlayer;
        FlipDiscs(outflanked);
        UpdateDiscCounts(movePlayer, outflanked.Count);
        PassTurn();

        moveInfo = new MoveInfo { Player = movePlayer, Position = pos, Outflanked = outflanked};

        return true;
    }

    public IEnumerable<Position> OccupiedPositions()
    {
        for (int col = 0; col < COLS; col++)
        {
            for (int row = 0; row < ROWS; row++)
            {
                if (Board[col, row] != Player.None)
                {
                    yield return new Position(col, row);
                }
            }
        }
    }

    private void FlipDiscs(List<Position> positions)
    {
        foreach(Position pos in positions)
        {
            Board[pos.Col, pos.Row] = Board[pos.Col, pos.Row].Opponent();
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

    private bool isInsideBoard(int row, int col)
    {
        return col >= 0 && col < COLS && row >= 0 && row < ROWS;
    }

    private bool isInsideBoard(Position pos)
    {
        return pos.Col >= 0 && pos.Col < COLS && pos.Row >= 0 && pos.Row < ROWS;
    }

    private List<Position> OutflankedInDir(Position initialPos, Player player, Direction dir)
    {
        List<Position> outflanked = new List<Position>();
        Position pos = initialPos + dir;

        while (isInsideBoard(pos) && Board[pos.Col, pos.Row] != Player.None)
        {
            if (Board[pos.Col, pos.Row] == player.Opponent())
            {
                outflanked.Add(pos);
                pos += dir;
            }
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
        if (Board[pos.Col, pos.Row] != Player.None)
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
        
        for (int col = 0; col < COLS; col++)
        {
            for (int row = 0; row < ROWS; row++)
            {
                Position pos = new Position(col, row);

                if (IsMoveLegal(player, pos, out List<Position> outflanked))
                {
                    legalMoves[pos] = outflanked;
                }
            }
        }

        return legalMoves;
    }
}
