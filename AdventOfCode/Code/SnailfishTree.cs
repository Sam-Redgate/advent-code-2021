namespace Code;

public class SnailfishTree
{
    private readonly Node _root;

    private SnailfishTree(Node root)
    {
        _root = root;
    }

    public static SnailfishTree Add(SnailfishTree left, SnailfishTree right)
    {
        var newRoot = new Node(0)
        {
            LeftChild = left._root.DeepCopy(),
            RightChild = right._root.DeepCopy()
        };
        newRoot.LeftChild.Parent = newRoot;
        newRoot.RightChild.Parent = newRoot;

        var newTree = new SnailfishTree(newRoot);
        newTree.ReduceIfRequired();
        return newTree;
    }

    private void ReduceIfRequired()
    {
        var operationPerformed = true;
        while (operationPerformed)
        {
            if (TreeCanBeExploded())
            {
                ExplodeFirstPair();
                continue;
            }

            if (TreeCanBeSplit())
            {
                var leaf = IterateNodes().First(node => node.CanSplit());
                var (leftValue, rightValue) = leaf.SplitValue();

                leaf.Value = 0u;
                leaf.LeftChild = new Node(leftValue, leaf);
                leaf.RightChild = new Node(rightValue, leaf);
                continue;
            }

            operationPerformed = false;
        }
    }

    public uint CalculateMagnitude()
    {
        return _root.CalculateMagnitude();
    }

    private void ExplodeFirstPair()
    {
        var nodes = IterateNodes().ToArray();
        var pairToExplode = nodes.First(node => node.CanExplode());

        var leftmostLeaf = FindLeftmostLeaf(pairToExplode);
        if (leftmostLeaf != null) leftmostLeaf.Value += pairToExplode.LeftChild!.Value;

        var rightmostLeaf = FindRightmostLeaf(pairToExplode);
        if (rightmostLeaf != null) rightmostLeaf.Value += pairToExplode.RightChild!.Value;

        pairToExplode.LeftChild = null;
        pairToExplode.RightChild = null;
        pairToExplode.Value = 0u;
    }

    private static Node? FindLeftmostLeaf(Node node)
    {
        var visitedNodes = new HashSet<Node> {node};
        if (node.Parent == null) return null;

        var currentNode = node.Parent;
        while (currentNode != null)
        {
            if (currentNode.LeftChild != null && !visitedNodes.Contains(currentNode.LeftChild))
            {
                currentNode = currentNode.LeftChild;
                break;
            }

            visitedNodes.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        if (currentNode == null) return null; // none found
        
        // find the right-most leaf of this new subtree.
        var tempTree = new SnailfishTree(currentNode);
        var leaves = tempTree.IterateNodes().Where(n => n.IsLeaf()).ToArray();
        
        return leaves.Length == 0 ? null : leaves[^1];
    }

    private static Node? FindRightmostLeaf(Node node)
    {
        var visitedNodes = new HashSet<Node> {node};
        if (node.Parent == null) return null;

        var currentNode = node.Parent;
        while (currentNode != null)
        {
            if (currentNode.RightChild != null && !visitedNodes.Contains(currentNode.RightChild))
            {
                currentNode = currentNode.RightChild;
                break;
            }

            visitedNodes.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        if (currentNode == null) return null; // none found
        
        // find the left-most leaf of this new subtree.
        var tempTree = new SnailfishTree(currentNode);
        var leaves = tempTree.IterateNodes().Where(n => n.IsLeaf()).ToArray();
        
        return leaves.Length == 0 ? null : leaves[0];
    }

    private bool TreeCanBeSplit()
    {
        return FindMaxValue(_root) > 9;
    }

    public uint Max()
    {
        return FindMaxValue(_root);
    }

    private static uint FindMaxValue(Node node)
    {
        if (node.LeftChild == null && node.RightChild == null)
        {
            return node.Value;
        }

        var leftValue = node.LeftChild == null ? 0u : FindMaxValue(node.LeftChild);            
        var rightValue = node.RightChild == null ? 0u : FindMaxValue(node.RightChild);

        return leftValue > rightValue ? leftValue : rightValue;
    }

    private bool TreeCanBeExploded()
    {
        return IterateNodes().Aggregate(false, (canBeExploded, node) => canBeExploded || node.CanExplode());
    }
    
    // we subtract one as leaves aren't considered another level.
    public uint TreeDepth() => NodeDepth(_root) - 1u;

    private uint NodeDepth(Node node)
    {
        const uint depth = 1u;
        
        if (node.LeftChild == null && node.RightChild == null)
        {
            return depth;
        }

        var leftDepth = 0u;
        var rightDepth = 0u;

        if (node.LeftChild != null)
        {
            leftDepth = NodeDepth(node.LeftChild) + depth;
        }

        if (node.RightChild != null)
        {
            rightDepth = NodeDepth(node.RightChild) + depth;
        }

        return leftDepth > rightDepth ? leftDepth : rightDepth;
    }

    private IEnumerable<Node> IterateNodes()
    {
        var queue = new Queue<Node>();
        
        Dfs(_root, queue);

        return queue.ToArray();
    }

    private static void Dfs(Node node, Queue<Node> queue)
    {
        queue.Enqueue(node);

        if (node.LeftChild != null)
        {
            Dfs(node.LeftChild, queue);
        }

        if (node.RightChild != null)
        {
            Dfs(node.RightChild, queue);
        }
    }

    public static SnailfishTree BuildSnailfishTree(string snailfishNumber)
    {
        var characters = snailfishNumber.ToCharArray();
        var rootNode = new Node(0);
        var currentNode = rootNode;
        foreach (var character in characters)
        {
            if (character == '[')
            {
                currentNode.LeftChild = new Node(0, parent: currentNode);
                currentNode = currentNode.LeftChild;
                continue;
            }

            if (character == ']')
            {
                currentNode = currentNode.Parent;
                if (currentNode!.Parent == null) return new SnailfishTree(rootNode);
                continue;
            }

            if (character == ',')
            {
                currentNode!.Parent!.RightChild = new Node(0, parent: currentNode.Parent);
                currentNode = currentNode.Parent.RightChild;
                continue;
            }

            // if we get to here it's a number, so current node is a leaf.
            currentNode.Value = Convert.ToUInt32(character.ToString());
        }

        return new SnailfishTree(rootNode);
    }

    public override string ToString()
    {
        return _root.ToString();
    }

    public class Node
    {
        public uint Value { get; set; }
        public Node? Parent { get; set; }
        public Node? LeftChild { get; set; }
        public Node? RightChild { get; set; }

        public Node(uint value, Node? parent = null)
        {
            Value = value;
            Parent = parent;
        }

        private uint Depth()
        {
            var depth = 0u;

            var current = this;
            while (current.Parent != null)
            {
                current = current.Parent;
                depth++;
            }

            return depth;
        }

        private bool IsRegularPair()
        {
            if (LeftChild == null) return false;
            if (RightChild == null) return false;
            
            return LeftChild.IsLeaf() && RightChild.IsLeaf();
        }

        public bool CanSplit()
        {
            if (!IsLeaf()) return false;
            return Value > 9;
        }

        public bool CanExplode()
        {
            return IsRegularPair() && Depth() >= 4;
        }

        public bool IsLeaf()
        {
            return LeftChild == null && RightChild == null;
        }

        public (uint, uint) SplitValue()
        {
            var doubleValue = (double) Value;
            var left = (uint) Math.Round(doubleValue / 2, MidpointRounding.ToNegativeInfinity);
            var right = (uint) Math.Round(doubleValue / 2, MidpointRounding.ToPositiveInfinity);

            return (left, right);
        }

        public Node DeepCopy()
        {
            return DeepCopy(this)!;
        }

        private static Node? DeepCopy(Node? node)
        {
            if (node == null) return null;
            
            var copy = new Node(node.Value)
            {
                LeftChild = DeepCopy(node.LeftChild),
                RightChild = DeepCopy(node.RightChild),
            };

            if (copy.LeftChild != null) copy.LeftChild.Parent = copy;
            if (copy.RightChild != null) copy.RightChild.Parent = copy;

            return copy;
        }

        public uint CalculateMagnitude()
        {
            if (LeftChild == null || RightChild == null) return 0u;

            var leftValue = LeftChild.IsLeaf() ? 3 * LeftChild.Value : 3 * LeftChild.CalculateMagnitude();
            var rightValue = RightChild.IsLeaf() ? 2 * RightChild.Value : 2 * RightChild.CalculateMagnitude();

            return leftValue + rightValue;
        }

        public override string ToString()
        {
            if (LeftChild == null && RightChild == null)
            {
                return Value.ToString();
            }

            var nextString = string.Empty;

            if (LeftChild != null)
            {
                nextString = $"[{LeftChild},";
            }

            if (RightChild != null)
            {
                nextString = string.Concat(nextString, $"{RightChild}]");
            }

            return nextString;
        }
    }
}