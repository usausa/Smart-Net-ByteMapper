namespace Smart.IO.ByteMapper;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

using Smart.IO.ByteMapper.Helpers;

#pragma warning disable CA1062
[DebuggerDisplay("{" + nameof(Diagnostics) + "}")]
public sealed class TypeProfileKeyCache<T>
{
    private const int InitialSize = 64;

    private const int Factor = 3;

    private static readonly Node EmptyNode = new(typeof(EmptyKey), null!, default!);

#if NET9_0_OR_GREATER
    private readonly Lock sync = new();
#else
    private readonly object sync = new();
#endif

    private volatile Node[] nodes;

    private int depth;

    private int count;

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public TypeProfileKeyCache()
    {
        nodes = CreateInitialTable();
    }

    //--------------------------------------------------------------------------------
    // Private
    //--------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CalculateHash(Type type, string profile)
    {
        return type.GetHashCode() ^ StringHash.Calc(profile);
    }

    private static int CalculateCount(Node[] targetNodes)
    {
        var count = 0;
        for (var i = 0; i < targetNodes.Length; i++)
        {
            var node = targetNodes[i];
            if (node != EmptyNode)
            {
                do
                {
                    count++;
                    node = node.Next;
                }
                while (node is not null);
            }
        }

        return count;
    }

    private static int CalculateDepth(Node node)
    {
        var length = 1;
        var next = node.Next;
        while (next is not null)
        {
            length++;
            next = next.Next;
        }

        return length;
    }

    private static int CalculateDepth(Node[] targetNodes)
    {
        var depth = 0;

        for (var i = 0; i < targetNodes.Length; i++)
        {
            var node = targetNodes[i];
            if (node != EmptyNode)
            {
                depth = Math.Max(CalculateDepth(node), depth);
            }
        }

        return depth;
    }

    private static int CalculateSize(int requestSize)
    {
        return (int)BitOperations.RoundUpToPowerOf2((uint)requestSize);
    }

    private static Node[] CreateInitialTable()
    {
        var newNodes = new Node[InitialSize];
        newNodes.AsSpan().Fill(EmptyNode);
        return newNodes;
    }

    private static Node FindLastNode(Node node)
    {
        while (node.Next is not null)
        {
            node = node.Next;
        }

        return node;
    }

    private static void UpdateLink(ref Node node, Node addNode)
    {
        if (node == EmptyNode)
        {
            node = addNode;
        }
        else
        {
            var last = FindLastNode(node);
            last.Next = addNode;
        }
    }

    private static void RelocateNodes(Node[] newNodes, Node[] oldNodes, int count)
    {
        var remaining = count;
        for (var i = 0; (i < oldNodes.Length) && (remaining > 0); i++)
        {
            var current = oldNodes[i];
            if (current == EmptyNode)
            {
                continue;
            }

            do
            {
                UpdateLink(ref newNodes[CalculateHash(current.Type, current.Profile) & (newNodes.Length - 1)], new Node(current.Type, current.Profile, current.Value));

                current = current.Next;
                remaining--;
            }
            while (current is not null);
        }
    }

    private void AddNode(Node node)
    {
        var currentNodes = nodes;

        var requestSize = Math.Max(InitialSize, (count + 1) * Factor);
        var size = CalculateSize(requestSize);
        if (size > currentNodes.Length)
        {
            var newNodes = new Node[size];
            newNodes.AsSpan().Fill(EmptyNode);

            RelocateNodes(newNodes, currentNodes, count);

            UpdateLink(ref newNodes[CalculateHash(node.Type, node.Profile) & (newNodes.Length - 1)], node);

            nodes = newNodes;
            depth = CalculateDepth(newNodes);
            count = CalculateCount(newNodes);
        }
        else
        {
            Interlocked.MemoryBarrier();

            var hash = CalculateHash(node.Type, node.Profile);

            UpdateLink(ref currentNodes[hash & (currentNodes.Length - 1)], node);

            depth = Math.Max(CalculateDepth(currentNodes[hash & (currentNodes.Length - 1)]), depth);
            count++;
        }
    }

    //--------------------------------------------------------------------------------
    // Public
    //--------------------------------------------------------------------------------

    public DiagnosticsInfo Diagnostics
    {
        get
        {
            lock (sync)
            {
                return new DiagnosticsInfo(nodes.Length, depth, count);
            }
        }
    }

    public void Clear()
    {
        lock (sync)
        {
            var newNodes = CreateInitialTable();

            nodes = newNodes;
            depth = 0;
            count = 0;
        }
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(Type type, string profile, [MaybeNullWhen(false)] out T value)
    {
        var temp = nodes;
        var node = temp[CalculateHash(type, profile) & (temp.Length - 1)];
        do
        {
            if ((node.Type == type) && (node.Profile == profile))
            {
                value = node.Value;
                return true;
            }
            node = node.Next;
        }
        while (node is not null);

        value = default;
        return false;
    }

    public T AddIfNotExist(Type type, string profile, Func<Type, string, T> valueFactory)
    {
        lock (sync)
        {
            // Double-checked locking
            if (TryGetValue(type, profile, out var currentValue))
            {
                return currentValue;
            }

            var value = valueFactory(type, profile);

            // Check if added by recursive
            if (TryGetValue(type, profile, out currentValue))
            {
                return currentValue;
            }

            AddNode(new Node(type, profile, value));

            return value;
        }
    }

    //--------------------------------------------------------------------------------
    // Inner
    //--------------------------------------------------------------------------------

    private sealed class EmptyKey
    {
    }

#pragma warning disable SA1401
    private sealed class Node
    {
        public readonly Type Type;

        public readonly string Profile;

        public readonly T Value;

        public Node Next;

        public Node(Type type, string profile, T value)
        {
            Type = type;
            Profile = profile;
            Value = value;
        }
    }
#pragma warning restore SA1401

    //--------------------------------------------------------------------------------
    // Diagnostics
    //--------------------------------------------------------------------------------

    public sealed class DiagnosticsInfo
    {
        public int Width { get; }

        public int Depth { get; }

        public int Count { get; }

        public DiagnosticsInfo(int width, int depth, int count)
        {
            Width = width;
            Depth = depth;
            Count = count;
        }

        public override string ToString() => $"Count={Count}, Width={Width}, Depth={Depth}";
    }
}
