using FluentAssertions;
using NUnit.Framework;

namespace Code;

public class DayEighteen
{
    [Test]
    public void PartOneTest()
    {
        var numbers = File.ReadLines("Resources/DayEighteenTest.txt").ToArray();
        var tree = SnailfishTree.BuildSnailfishTree(numbers[0]);
        for (var i = 1; i < numbers.Length; i++)
        {
            tree = SnailfishTree.Add(tree, SnailfishTree.BuildSnailfishTree(numbers[i]));
        }

        tree.ToString().Should().BeEquivalentTo("[[[[6,6],[7,6]],[[7,7],[7,0]]],[[[7,7],[7,7]],[[7,8],[9,9]]]]");
        tree.CalculateMagnitude().Should().Be(4140u);
    }

    [Test]
    public void PartOne()
    {
        var numbers = File.ReadLines("Resources/DayEighteenInput.txt").ToArray();
        var tree = SnailfishTree.BuildSnailfishTree(numbers[0]);
        for (var i = 1; i < numbers.Length; i++)
        {
            tree = SnailfishTree.Add(tree, SnailfishTree.BuildSnailfishTree(numbers[i]));
        }

        var partOneAnswer = tree.CalculateMagnitude();
        Console.WriteLine(partOneAnswer);
        partOneAnswer.Should().Be(3725u);
    }

    [Test]
    public void PartTwoTest()
    {
        var numbers = File.ReadLines("Resources/DayEighteenTest.txt").ToArray();
        var trees = numbers.Select(SnailfishTree.BuildSnailfishTree).ToArray();
        var currentMax = 0u;

        foreach (var tree in trees)
        {
            var otherTrees = trees.Except(new []{tree});
            foreach (var otherTree in otherTrees)
            {
                var newSnailfishTree = SnailfishTree.Add(tree, otherTree);
                var magnitude = newSnailfishTree.CalculateMagnitude();
                if (magnitude > currentMax) currentMax = magnitude;
            }
        }

        currentMax.Should().Be(3993u);
    }

    [Test]
    public void PartTwo()
    {
        var numbers = File.ReadLines("Resources/DayEighteenInput.txt").ToArray();
        var trees = numbers.Select(SnailfishTree.BuildSnailfishTree).ToArray();
        var partTwoAnswer = 0u;

        foreach (var tree in trees)
        {
            var otherTrees = trees.Except(new []{tree});
            foreach (var otherTree in otherTrees)
            {
                var newSnailfishTree = SnailfishTree.Add(tree, otherTree);
                var magnitude = newSnailfishTree.CalculateMagnitude();
                if (magnitude > partTwoAnswer) partTwoAnswer = magnitude;
            }
        }

        Console.WriteLine(partTwoAnswer);

        partTwoAnswer.Should().Be(4832u);
    }

    [TestCase("[1,2]")]
    [TestCase("[[1,2],3]")]
    [TestCase("[[1,2],3]")]
    [TestCase("[[1,9],[8,5]]")]
    [TestCase("[[[[1,2],[3,4]],[[5,6],[7,8]]],9]")]
    [TestCase("[[[9,[3,8]],[[0,9],6]],[[[3,7],[4,9]],3]]")]
    [TestCase("[[[[1,3],[5,3]],[[1,3],[8,7]]],[[[4,9],[6,9]],[[8,2],[7,3]]]]")]
    [TestCase("[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]")]
    [TestCase("[[[5,[2,8]],4],[5,[[9,9],0]]]")]
    public void TestSnailfishTreeParsing(string test)
    {
        var tree = SnailfishTree.BuildSnailfishTree(test);
        tree.ToString().Should().BeEquivalentTo(test);
    }

    [TestCase("[1,2]", "[[3,4],5]", "[[1,2],[[3,4],5]]")]
    [TestCase("[[[[4,3],4],4],[7,[[8,4],9]]]", "[1,1]", "[[[[0,7],4],[[7,8],[6,0]]],[8,1]]")]
    public void TestSnailfishTreeAddition(string left, string right, string expected)
    {
        var leftTree = SnailfishTree.BuildSnailfishTree(left);
        var rightTree = SnailfishTree.BuildSnailfishTree(right);
        SnailfishTree.Add(leftTree, rightTree).ToString().Should().BeEquivalentTo(expected);
    }

    [TestCase("[1,2]", 1u)]
    [TestCase("[[1,2],3]", 2u)]
    [TestCase("[[1,2],3]", 2u)]
    [TestCase("[[1,9],[8,5]]", 2u)]
    [TestCase("[[[[1,2],[3,4]],[[5,6],[7,8]]],9]", 4u)]
    [TestCase("[[[9,[3,8]],[[0,9],6]],[[[3,7],[4,9]],3]]", 4u)]
    [TestCase("[[[[1,3],[5,3]],[[1,3],[8,7]]],[[[4,9],[6,9]],[[8,2],[7,3]]]]", 4u)]
    public void TestSnailfishTreeDepth(string number, uint expectedDepth)
    {
        SnailfishTree.BuildSnailfishTree(number).TreeDepth().Should().Be(expectedDepth);
    }

    [TestCase("[1,2]", 2u)]
    [TestCase("[[1,2],3]", 3u)]
    [TestCase("[[1,2],3]", 3u)]
    [TestCase("[[1,9],[8,5]]", 9u)]
    [TestCase("[[[[1,2],[3,4]],[[5,6],[7,8]]],9]", 9u)]
    [TestCase("[[[9,[3,8]],[[0,9],6]],[[[3,7],[4,9]],3]]", 9u)]
    [TestCase("[[[[1,3],[5,3]],[[1,3],[8,7]]],[[[4,9],[6,9]],[[8,2],[7,3]]]]", 9u)]
    public void TestSnailfishMax(string number, uint expectedMax)
    {
        SnailfishTree.BuildSnailfishTree(number).Max().Should().Be(expectedMax);
    }

    [TestCase("[[1,2],[[3,4],5]]", 143u)]
    [TestCase("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]", 1384u)]
    [TestCase("[[[[1,1],[2,2]],[3,3]],[4,4]]", 445u)]
    [TestCase("[[[[3,0],[5,3]],[4,4]],[5,5]]", 791u)]
    [TestCase("[[[[5,0],[7,4]],[5,5]],[6,6]]", 1137u)]
    [TestCase("[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]", 3488u)]
    public void TestCalculateMagnitude(string number, uint expectedMagnitude)
    {
        var tree = SnailfishTree.BuildSnailfishTree(number);
        tree.CalculateMagnitude().Should().Be(expectedMagnitude);
    }
}