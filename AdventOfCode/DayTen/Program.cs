namespace DayTen
{
    internal static class Program
    {
        public static void Main()
        {
            var scoreTable = new Dictionary<int, int>
            {
                {3, 0},
                {57, 0},
                {1197, 0},
                {25137, 0}
            };
            
            foreach (var line in File.ReadLines("./Resources/input.txt"))
            {
                var parser = SyntaxParser.ParseInput(line);
                if (parser.IsCorrupted)
                {
                    scoreTable[parser.Score] += 1;
                }
            }
            
            var total = scoreTable.Aggregate(0, (sum, pair) => sum + pair.Key * pair.Value);
            
            Console.WriteLine(total);
        }
    }

    public class SyntaxParser
    {
        public int Score { get; }
        public bool IsCorrupted { get; }

        private SyntaxParser(int score, bool isCorrupted)
        {
            Score = score;
            IsCorrupted = isCorrupted;
        }

        public static SyntaxParser ParseInput(string input)
        {
            var chars = input.ToCharArray();
            var stack = new Stack<char>();
            var isCorrupted = false;
            var score = 0;

            foreach (var c in chars)
            {
                if (IsOpenCharacter(c))
                {
                    stack.Push(c);
                }
                else if (IsCloseCharacter(c))
                {
                    if (stack.Count > 0 && ArePair(stack.Peek(), c))
                    {
                        stack.Pop();
                    }
                    else
                    {
                        isCorrupted = true;
                        score = ScoreIllegalCharacter(c);
                        return new SyntaxParser(score, isCorrupted);
                    }
                }
            }

            return new SyntaxParser(score, isCorrupted);
        }

        private static bool IsOpenCharacter(char c)
        {
            return c is '(' or '[' or '{' or '<';
        }

        private static bool IsCloseCharacter(char c)
        {
            return c is ')' or ']' or '}' or '>';
        }

        private static bool ArePair(char open, char close)
        {
            switch (open)
            {
                case '(' when close is ')':
                case '[' when close is ']':
                case '{' when close is '}':
                case '<' when close is '>':
                    return true;
                default:
                    return false;
            }
        }

        private static int ScoreIllegalCharacter(char c)
        {
            return c switch
            {
                ')' => 3,
                ']' => 57,
                '}' => 1197,
                '>' => 25137,
                _ => 0
            };
        }
    }
}