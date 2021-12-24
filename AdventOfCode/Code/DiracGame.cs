namespace Code.Resources;

public class DiracGame
{
    public class DiracGamerComparer : IEqualityComparer<DiracGame>
    {
        public bool Equals(DiracGame? x, DiracGame? y)
        {
            if (x == null && y == null) return true;
            if (x == null) return false;
            if (y == null) return false;

            var xScoreBoard = $"{x.PlayerOne.Score}|{x.PlayerOne.Position}|{x.PlayerTwo.Score}|{x.PlayerTwo.Position}";
            var yScoreBoard = $"{y.PlayerOne.Score}|{y.PlayerOne.Position}|{y.PlayerTwo.Score}|{y.PlayerTwo.Position}";

            return string.Equals(xScoreBoard, yScoreBoard, StringComparison.Ordinal);
        }

        public int GetHashCode(DiracGame obj)
        {
            var scoreBoard = $"{obj.PlayerOne.Score}|{obj.PlayerOne.Position}|{obj.PlayerTwo.Score}|{obj.PlayerTwo.Position}";
            return scoreBoard.GetHashCode();
        }
    }

    private readonly uint _winningScore;

    public record Player(string Name, uint Position, uint Score = 0);

    public Player PlayerOne { get; private set; }
    public Player PlayerTwo { get; private set; }

    private DiracGame(uint winningScore, Player playerOne, Player playerTwo)
    {
        _winningScore = winningScore;
        PlayerOne = playerOne;
        PlayerTwo = playerTwo;
    }

    public static DiracGame NewGame(IEnumerable<string> input, uint winningScore = 1000)
    {
        var players = new Dictionary<string, Player>();

        foreach (var row in input)
        {
            var tokens = row.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            players[tokens[1]] = new Player(tokens[1], Convert.ToUInt32(tokens[^1]));
        }

        return new DiracGame(winningScore, players["1"], players["2"]);
    }

    public bool Move(string playerName, uint moves)
    {
        if (playerName != "1" && playerName != "2")
            throw new ArgumentOutOfRangeException(nameof(playerName), "No player of that name in the game.");
        var player = playerName == "1" ? PlayerOne : PlayerTwo;

        var newPosition = player.Position + moves;
        while (newPosition > 10)
        {
            newPosition -= 10;
        }

        player = player with {Position = newPosition, Score = player.Score + newPosition};

        if (playerName == "1")
        {
            PlayerOne = player;
        }
        else
        {
            PlayerTwo = player;
        }

        return player.Score >= _winningScore;
    }

    public (bool, DiracGame) DiracMove(string playerName, uint moves)
    {
        var copy = CopyGame();
        
        return (copy.Move(playerName, moves), copy);
    }

    public bool GameEnded()
    {
        return PlayerOne.Score >= _winningScore || PlayerTwo.Score >= _winningScore;
    }

    public Player GetWinner()
    {
        return PlayerOne.Score > PlayerTwo.Score ? PlayerOne : PlayerTwo;
    }

    public Player GetLoser()
    {
        return PlayerOne.Score < PlayerTwo.Score ? PlayerOne : PlayerTwo;
    }

    private DiracGame CopyGame()
    {
        return new DiracGame(_winningScore, PlayerOne, PlayerTwo);
    }
}