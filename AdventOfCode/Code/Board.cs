using System.Text;

namespace Code;

public class Board
{
    private record Number(int Value, bool Marked);

    private readonly Number[][] _board;

    public string Id { get; }

    public Board(IEnumerable<string> stringBoard)
    {
        var stringBoardArray = stringBoard.ToArray();
        _board = ParseBoard(stringBoardArray);
        Id = string.Join(' ', stringBoardArray);
    }

    public bool MarkNumber(int number)
    {
        for (var y = 0; y < _board.Length; y++)
        {
            for (var x = 0; x < _board[y].Length; x++)
            {
                if (_board[y][x].Value != number) continue;
                    
                _board[y][x] = _board[y][x] with { Marked = true };
                return CheckColumn(x) || CheckRow(y);
            }
        }

        return false;
    }

    public int CalculateScore(int lastCalledNumber)
    {
        var sumOfUnmarkedNumbers = _board.Aggregate(0, (sum, numbers) => sum + SumUnmarkedNumbers(numbers));

        return sumOfUnmarkedNumbers * lastCalledNumber;
    }

    private static int SumUnmarkedNumbers(IEnumerable<Number> row) =>
        row.Aggregate(0, (sum, number) => number.Marked ? sum : number.Value + sum);

    private bool CheckRow(int index) => _board[index].Aggregate(true, (marked, number) => number.Marked && marked);

    private bool CheckColumn(int index) => _board.Select(row => row[index])
        .Aggregate(true, (marked, number) => number.Marked && marked);

    private static Number[][] ParseBoard(IEnumerable<string> stringBoard) =>
        stringBoard.Select(ParseBoardRow).ToArray();

    private static Number[] ParseBoardRow(string stringBoardRow) => stringBoardRow
        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Select(s => Convert.ToInt32(s))
        .Select(i => new Number(i, false))
        .ToArray();

    public override string ToString()
    {
        var result = new StringBuilder();
        foreach (var row in _board)
        {
            foreach (var number in row)
            {
                result.Append(number.Marked ? $"[{number.Value,2}]" : $" {number.Value,2} ");
            }

            result.Append('\n');
        }

        return result.ToString();
    }
}