using System.Collections.Concurrent;
using FluentAssertions;
using NUnit.Framework;

namespace Code.Resources;

public static class DayTwentyOne
{
    [Test]
    public static void TestOne()
    {
        var game = DiracGame.NewGame(File.ReadLines("./Resources/DayTwentyOneTest.txt"));
        var die = Die.GetDie(100, Die.DieType.Sequential);
        var dieRolls = 0;

        var gameEnded = false;
        while (!gameEnded)
        {
            foreach (var playerName in new[] {game.PlayerOne.Name, game.PlayerTwo.Name})
            {
                var dieResult = Enumerable.Range(0, 3).Aggregate(0u, (result, _) => result + die.Roll());
                dieRolls += 3;
                gameEnded = game.Move(playerName, dieResult);

                if (!gameEnded) continue;

                Console.WriteLine($"Player {playerName} won!");
                var loser = game.GetLoser();
                Console.WriteLine($"Player {loser.Name} lost. {loser.Score} * {dieRolls} = {loser.Score * dieRolls}");
                (loser.Score * dieRolls).Should().Be(739785u);
                return;
            }
        }
    }

    [Test]
    public static void StarOne()
    {
        var game = DiracGame.NewGame(File.ReadLines("./Resources/DayTwentyOneInput.txt"));
        var die = Die.GetDie(100, Die.DieType.Sequential);
        var dieRolls = 0;

        var gameEnded = false;
        while (!gameEnded)
        {
            foreach (var playerName in new[] {game.PlayerOne.Name, game.PlayerTwo.Name})
            {
                var dieResult = Enumerable.Range(0, 3).Aggregate(0u, (result, _) => result + die.Roll());
                dieRolls += 3;
                gameEnded = game.Move(playerName, dieResult);

                if (!gameEnded) continue;

                Console.WriteLine($"Player {playerName} won!");
                var loser = game.GetLoser();
                Console.WriteLine($"Player {loser.Name} lost. {loser.Score} * {dieRolls} = {loser.Score * dieRolls}");
                (loser.Score * dieRolls).Should().Be(921585u);
                return;
            }
        }
    }

    [Test]
    public static void TestTwo()
    {
        var startingGame = DiracGame.NewGame(File.ReadLines("./Resources/DayTwentyOneTest.txt"), 21);
        var stateSpace = new Dictionary<GameState, ulong>();
        var playersScore = new ConcurrentDictionary<string, ulong>
        {
            ["1"] = 0ul,
            ["2"] = 0ul
        };

        var initialState =
            new GameState(
                new PlayerState((int) startingGame.PlayerOne.Position, (int) startingGame.PlayerOne.Score),
                new PlayerState((int) startingGame.PlayerTwo.Position, (int) startingGame.PlayerTwo.Score));
        
        stateSpace[initialState] = 1ul;
        while (stateSpace.Count > 0)
        {
            var (state, currentStateCount) = stateSpace.First();
            stateSpace.Remove(state);

            foreach (var nextState in CalculateNextStates(state))
            {
                if (nextState.One.Score >= 21 || nextState.Two.Score >= 21)
                {
                    var winner = nextState.One.Score > nextState.Two.Score ? "1" : "2";
                    playersScore[winner] += currentStateCount;
                    continue;
                }

                if (stateSpace.ContainsKey(nextState))
                {
                    stateSpace[nextState] += currentStateCount;
                    continue;
                }

                stateSpace[nextState] = currentStateCount;
            }
        }

        Console.WriteLine($"Player 1 wins: {playersScore["1"]}");
        Console.WriteLine($"Player 2 wins: {playersScore["2"]}");
        Console.WriteLine($"Player {playersScore.Keys.MaxBy(name => playersScore[name])} wins!");
        playersScore.MaxBy(pair => playersScore[pair.Key]).Value.Should().Be(444356092776315ul);
    }

    [Test]
    public static void StarTwo()
    {
        var startingGame = DiracGame.NewGame(File.ReadLines("./Resources/DayTwentyOneInput.txt"), 21);
        var stateSpace = new Dictionary<GameState, ulong>();
        var playersScore = new ConcurrentDictionary<string, ulong>
        {
            ["1"] = 0ul,
            ["2"] = 0ul
        };

        var initialState =
            new GameState(
                new PlayerState((int) startingGame.PlayerOne.Position, (int) startingGame.PlayerOne.Score),
                new PlayerState((int) startingGame.PlayerTwo.Position, (int) startingGame.PlayerTwo.Score));
        
        stateSpace[initialState] = 1ul;
        while (stateSpace.Count > 0)
        {
            var (state, currentStateCount) = stateSpace.First();
            stateSpace.Remove(state);

            foreach (var nextState in CalculateNextStates(state))
            {
                if (nextState.One.Score >= 21 || nextState.Two.Score >= 21)
                {
                    var winner = nextState.One.Score > nextState.Two.Score ? "1" : "2";
                    playersScore[winner] += currentStateCount;
                    continue;
                }

                if (stateSpace.ContainsKey(nextState))
                {
                    stateSpace[nextState] += currentStateCount;
                    continue;
                }

                stateSpace[nextState] = currentStateCount;
            }
        }

        Console.WriteLine($"Player 1 wins: {playersScore["1"]}");
        Console.WriteLine($"Player 2 wins: {playersScore["2"]}");
        Console.WriteLine($"Player {playersScore.Keys.MaxBy(name => playersScore[name])} wins!");
        playersScore.MaxBy(pair => playersScore[pair.Key]).Value.Should().Be(911090395997650ul);
    }

    private static IEnumerable<GameState> CalculateNextStates(GameState state)
    {
        var playerOneStates = GetNextStates(state.One);
        var playerTwoStates = GetNextStates(state.Two);

        foreach (var playerOneState in playerOneStates)
        {
            // player 2 doesn't get a go if player one won.
            if (playerOneState.Score < 21)
            {
                foreach (var playerTwoState in playerTwoStates)
                {
                    yield return new GameState(playerOneState, playerTwoState);
                }
                continue;
            }

            yield return state with {One = playerOneState};
        }
    }

    private record GameState(PlayerState One, PlayerState Two);

    private record PlayerState(int Position, int Score);

    private static readonly ConcurrentDictionary<PlayerState, PlayerState[]> Lookup = new();

    private static PlayerState[] GetNextStates(PlayerState currentState)
    {
        if (Lookup.TryGetValue(currentState, out var nextStates))
        {
            return nextStates;
        }

        var newStates = new List<PlayerState>();
        for (var firstRoll = 1; firstRoll <= 3; firstRoll++)
        {
            for (var secondRoll = 1; secondRoll <= 3; secondRoll++)
            {
                for (var thirdRoll = 1; thirdRoll <= 3; thirdRoll++)
                {
                    var newPosition = AddPosition(currentState.Position, firstRoll + secondRoll + thirdRoll);
                    var newState = currentState with
                    {
                        Position = newPosition, Score = currentState.Score + newPosition
                    };

                    newStates.Add(newState);
                }
            }
        }

        var states = newStates.ToArray();
        Lookup[currentState] = states;

        return states;
    }

    private static int AddPosition(int currentPosition, int amount)
    {
        return (currentPosition + amount - 1) % 10 + 1;
    }
}