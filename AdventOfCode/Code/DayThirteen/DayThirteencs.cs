using System.Text;

namespace Code;

internal static class DayThirteen
{
    private record FoldInstruction(char Axis, int Value);
    
    public static void Run()
    {
        var input = File.ReadLines("./Resources/DayThirteenInput.txt").ToArray();
        var instructions = ParseInstructions(input).ToArray();
        var dots = ParseDots(input).ToArray();

        var maxHeight = dots.Max(coordinate => coordinate.Y) + 1;
        var maxWidth = dots.Max(coordinate => coordinate.X) + 1;

        var page = BuildPage(dots, maxHeight, maxWidth);
        
        page = FoldPage(page, instructions[0]);

        var sum = PageIterator(page).Count(coordinate => page[coordinate.Y, coordinate.X] == '#');

        Console.WriteLine($"Number of dots: {sum}");
    }

    private static char[,] FoldPage(char[,] page, FoldInstruction instruction)
    {
        var (axis, value) = instruction;
        return axis switch
        {
            'x' => FoldAlongX(page, value),
            'y' => FoldAlongY(page, value),
            _ => page
        };
    }

    private static char[,] FoldAlongY(char[,] page, int foldRow)
    {
        var pageOne = new char[page.GetLength(0) - foldRow - 1, page.GetLength(1)];
        var pageTwo = new char[page.GetLength(0) - foldRow - 1, page.GetLength(1)];
        
        foreach (var (x, y) in PageIterator(page))
        {
            if (y < pageOne.GetLength(0))
            {
                pageOne[y, x] = page[y, x];
            }

            if (y > pageOne.GetLength(0))
            {
                pageTwo[y - foldRow - 1, x] = page[y, x];
            }
        }

        foreach (var coordinate in PageIterator(pageOne))
        {
            var (foldedX, foldedY) = coordinate with {Y = pageTwo.GetLength(0) - coordinate.Y - 1};
            if (pageTwo[foldedY, foldedX] == '#')
            {
                pageOne[coordinate.Y, coordinate.X] = pageTwo[foldedY, foldedX];
            }
        }
        
        return pageOne;
    }

    private static char[,] FoldAlongX(char[,] page, int foldColumn)
    {
        var pageOne = new char[page.GetLength(0), page.GetLength(1) - foldColumn - 1];
        var pageTwo = new char[page.GetLength(0), page.GetLength(1) - foldColumn - 1];
        
        foreach (var (x, y) in PageIterator(page))
        {
            if (x < pageOne.GetLength(1))
            {
                pageOne[y, x] = page[y, x];
            }

            if (x > pageOne.GetLength(1))
            {
                pageTwo[y, x  - foldColumn - 1] = page[y, x];
            }
        }
        
        foreach (var coordinate in PageIterator(pageOne))
        {
            var (foldedX, foldedY) = coordinate with {X = pageTwo.GetLength(1) - coordinate.X - 1};
            if (pageTwo[foldedY, foldedX] == '#')
            {
                pageOne[coordinate.Y, coordinate.X] = pageTwo[foldedY, foldedX];
            }
        }
        
        return pageOne;
    }

    private static string PrintPage(char[,] page)
    {
        var builder = new StringBuilder();
        var currentRow = 0;
        
        foreach (var (x, y) in PageIterator(page))
        {
            if (currentRow != y)
            {
                builder.Append('\n');
                currentRow = y;
            }

            builder.Append(page[y, x]);
        }

        return builder.ToString();
    }

    private static IEnumerable<Coordinate> PageIterator(char[,] page)
    {
        var maxHeight = page.GetLength(0);
        var maxWidth = page.GetLength(1);

        for (var y = 0; y < maxHeight; y++)
        {
            for (var x = 0; x < maxWidth; x++)
            {
                yield return new Coordinate(x, y);
            }
        }
    }

    private static char[,] BuildPage(IEnumerable<Coordinate> dots, int maxHeight, int maxWidth)
    {
        var page = new char[maxHeight, maxWidth];
        
        foreach (var (x, y) in PageIterator(page))
        {
            page[y, x] = '.';
        }

        foreach (var (x, y) in dots)
        {
            page[y, x] = '#';
        }
        
        return page;
    }

    private static IEnumerable<FoldInstruction> ParseInstructions(IEnumerable<string> input)
    {
        return input.Where(row => row.Contains("fold along")).Select(ParseInstruction);
    }

    private static IEnumerable<Coordinate> ParseDots(IEnumerable<string> input)
    {
        return input.Where(row => row.Contains(',')).Select(ParseDotRow);
    }

    private static Coordinate ParseDotRow(string input)
    {
        var x = Convert.ToInt32(input.Split(',')[0]);
        var y = Convert.ToInt32(input.Split(',')[1]);

        return new Coordinate(x, y);
    }

    private static FoldInstruction ParseInstruction(string input)
    {
        var value = Convert.ToInt32(input.Split('=')[^1]);
        var axis = input.Split(' ')[^1].Split('=')[0].ToCharArray()[0];

        return new FoldInstruction(axis, value);
    }
}