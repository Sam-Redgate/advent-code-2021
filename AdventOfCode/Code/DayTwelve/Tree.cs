namespace Code;

public class Tree<T> where T : IEquatable<T>
{
    private readonly Node _root;

    private class Node
    {
        public T Value { get; }
        public Node? Parent { get; private set; }
        public IEnumerable<Node> Children { get; private set; }

        public Node(T value, Node? parent = null)
        {
            Value = value;
            Parent = parent;
            Children = Array.Empty<Node>();
        }

        public void SetChildren(IEnumerable<Node> children)
        {
            var childrenArray = children.ToArray();
            foreach (var child in childrenArray)
            {
                child.Parent = this;
            }

            Children = childrenArray;
        }
    }

    private Tree(Node root)
    {
        _root = root;
    }

    public static Tree<T> BuildTree(T start, IEnumerable<(T, T)> edges, Func<T, T, bool> permittedDuplicateFilter,
        Func<T, bool> isLeafValue)
    {
        var rootNode = new Node(start);

        rootNode.SetChildren(AddNodes(rootNode, edges.ToArray(), permittedDuplicateFilter, isLeafValue));

        return new Tree<T>(rootNode);
    }

    public int CountValue(T value)
    {
        return _root.Value.Equals(value) ? 1 : CountValue(value, _root.Children);
    }

    public void PrintTree()
    {
        BuildString(_root, new List<string>());
    }

    private static void BuildString(Node node, List<string> path)
    {
        path.Add(node.Value.ToString() ?? string.Empty);
        if (!node.Children.Any())
        {
            Console.WriteLine(string.Join(',', path));
        }

        foreach (var nodeChild in node.Children)
        {
            BuildString(nodeChild, new List<string>(path));
        }
    }

    private static int CountValue(T value, IEnumerable<Node> nodes)
    {
        var nodeArray = nodes.ToArray();

        if (!nodeArray.Any()) return 0;

        var sum = nodeArray.Count(node => node.Value.Equals(value));

        return nodeArray.Aggregate(sum, (childrenSum, node) => childrenSum + CountValue(value, node.Children));
    }

    private static IEnumerable<Node> AddNodes(Node parent, (T, T)[] edges, Func<T, T, bool> permittedDuplicateFilter,
        Func<T, bool> isLeafValue)
    {
        if (ContainsInvalidDuplicate(parent, permittedDuplicateFilter) || isLeafValue(parent.Value))
            return Array.Empty<Node>();

        var relevantSortedEdges =
            edges.Where(EdgeContainsValue(parent.Value)).Select(SortEdgeByValue(parent.Value)).ToArray();

        var newNodes = relevantSortedEdges.Select(edge => new Node(edge.Item2, parent)).ToArray();

        foreach (var newNode in newNodes)
        {
            newNode.SetChildren(AddNodes(newNode, edges, permittedDuplicateFilter, isLeafValue));
        }

        return newNodes;
    }

    private static bool ContainsInvalidDuplicate(Node parent, Func<T, T, bool> permittedDuplicateFilter)
    {
        var currentNode = parent;
        while (currentNode.Parent != null)
        {
            currentNode = currentNode.Parent;
            if (!permittedDuplicateFilter(parent.Value, currentNode.Value))
            {
                // In this instance, we've been told we can't add this value twice.
                return true;
            }
        }

        return false;
    }

    private static Func<(T, T), bool> EdgeContainsValue(T value)
    {
        return edge => value.Equals(edge.Item1) || value.Equals(edge.Item2);
    }

    private static Func<(T, T), (T, T)> SortEdgeByValue(T value)
    {
        return edge => edge.Item2.Equals(value) ? (edge.Item2, edge.Item1) : edge;
    }
}