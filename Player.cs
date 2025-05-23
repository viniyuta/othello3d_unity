public enum Player
{
    None, Black, White, NotPlayable
}

public static class PlayerExtensions
{
    public static Player Opponent(this Player player)
    {
        if (player == Player.Black)
        {
            return Player.White;
        }
        if (player == Player.White)
        {
            return Player.Black;
        }

        return Player.None;
    }
}
