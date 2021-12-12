namespace Code;

public class PathTree
{
    private readonly Node _root;

    public class Node
    {
        public string Value { get; }
        public Node? Parent { get; private set; }
        public Node[] Children { get; private set; }

        public Node(string value, Node? parent = null)
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

    private PathTree(Node root)
    {
        _root = root;
    }

    public static PathTree BuildTree(string start, IEnumerable<(string, string)> edges)
    {
        var rootNode = new Node(start);

        rootNode.SetChildren(AddNodes(rootNode, edges.ToArray()));

        return new PathTree(rootNode);
    }

    public int CountValue(string value)
    {
        return _root.Value.Equals(value) ? 1 : CountValue(value, _root.Children);
    }

    private static int CountValue(string value, IEnumerable<Node> nodes)
    {
        var nodeArray = nodes.ToArray();

        if (!nodeArray.Any()) return 0;

        var sum = nodeArray.Count(node => node.Value.Equals(value));

        return nodeArray.Aggregate(sum, (childrenSum, node) => childrenSum + CountValue(value, node.Children));
    }

    private static IEnumerable<Node> AddNodes(Node parent, (string, string)[] edges)
    {
        if (CannotCreateNode(parent)) return Array.Empty<Node>();

        var relevantSortedEdges = edges.Where(EdgeContainsValue(parent.Value)).Select(SortEdgeByValue(parent.Value)).ToArray();

        var newNodes = relevantSortedEdges.Select(edge => new Node(edge.Item2, parent)).ToArray();

        foreach (var newNode in newNodes)
        {
            newNode.SetChildren(AddNodes(newNode, edges));
        }

        return newNodes;
    }

    private static bool CannotCreateNode(Node parent)
    {
        if (parent.Value.Equals("end"))
        {
            return true;
        }

        if (parent.Value.Equals("start") && parent.Parent != null)
        {
            return true;
        }

        if (PathViolatesCaveConstraints(parent))
        {
            return true;
        }

        return false;
    }

    private static bool PathViolatesCaveConstraints(Node parent)
    {
        var violationDictionary = new Dictionary<string, int>();
        
        var currentNode = parent;
        var smallCaveCapReached = false;
        do
        {
            if (currentNode.Value.ToUpper().Equals(currentNode.Value)) continue;

            if (!violationDictionary.ContainsKey(currentNode.Value))
            {
                violationDictionary[currentNode.Value] = 1;
                continue;
            }

            violationDictionary[currentNode.Value] += 1;

            if (violationDictionary[currentNode.Value] >= 2 && smallCaveCapReached) return true;
            
            smallCaveCapReached = true;
        } while ((currentNode = currentNode.Parent) != null);

        return false;
    }

    private static Func<(string, string), bool> EdgeContainsValue(string value)
    {
        return edge => value.Equals(edge.Item1) || value.Equals(edge.Item2);
    }

    private static Func<(string, string), (string, string)> SortEdgeByValue(string value)
    {
        return edge => edge.Item2.Equals(value) ? (edge.Item2, edge.Item1) : edge;
    }
}