using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ledger.Core
{
    public class ItemCollection<TItem> : IEnumerable<TItem> where TItem : class, IItem
    {
        private readonly IIndexable _owner;
        private Node _rootNode;

        protected ItemCollection(IIndexable owner)
        {
            _owner = owner;
            _rootNode = new Node(owner);
        }

        protected ItemCollection(IIndexable owner, ItemCollection<TItem> source)
        {
            _owner = owner;
            _rootNode = source._rootNode;
        }

        public void Add(TItem item)
        {
            AddOrUpdate(item, null);
        }

        public void AddOrUpdate(TItem item, TItem existingItem)
        {
            // Performance.MarkStart("ItemCollection.AddOrUpdate");

            _rootNode = CopyNode(_rootNode);

            var accountIdParts = ((Account)item.Account).IdParts;
            var node = _rootNode;

            for (var accountIdPartIndex = 0; accountIdPartIndex < accountIdParts.Length; accountIdPartIndex++)
            {
                var idPart = accountIdParts[accountIdPartIndex];
                Node childNode;

                if (node.ChildNodes.TryGetValue(idPart, out childNode))
                    childNode = CopyNode(childNode);
                else
                    childNode = new Node(_owner);

                node.ChildNodes.Set(idPart, childNode);
                node = childNode;
            }

            if (existingItem != null)
                node.Items.Remove(existingItem);

            node.Items.Add(item);

            // Performance.MarkEnd("ItemCollection.AddOrUpdate");
        }

        private Node CopyNode(Node node)
        {
            if (node.Owner == _owner)
                return node;

            return node.Copy(_owner);
        }

        protected ICollection<TItem> FindItems(IAccount account)
        {
            // Performance.MarkStart("ItemCollection.FindItems(account)");

            var result = new List<TItem>();

            if (account is Account)
            {
                FindItemsByIdParts(_rootNode, ((Account)account).IdParts, result);
            }
            else
            {
                _rootNode.GetAllItems(result);

                result = result.Where(item => account.Equals(item.Account)).ToList();
            }

            // Performance.MarkEnd("ItemCollection.FindItems(account)");

            return result;
        }

        protected ICollection<TItem> FindItems(IAccountPredicate predicate)
        {
            // Performance.MarkStart("ItemCollection.FindItems(predicate)");

            var result = new List<TItem>();

            if (predicate is QueryAccountPredicate)
            {
                FindItemsByQueryParts(_rootNode, ((QueryAccountPredicate)predicate).QueryParts, 0, result);
            }
            else if (predicate is OrAccountPredicate)
            {
                result = ((OrAccountPredicate)predicate).Predicates.SelectMany(FindItems).Distinct().ToList();
            }
            else if (predicate is AndAccountPredicate)
            {
                throw new NotImplementedException();
            }
            else
            {
                _rootNode.GetAllItems(result);

                result = result.Where(item => predicate.Matches(item.Account)).ToList();
            }

            // Performance.MarkEnd("ItemCollection.FindItems(predicate)");

            return result;
        }

        private void FindItemsByIdParts(Node node, string[] idParts, List<TItem> result)
        {
            for (var idPartIndex = 0; idPartIndex < idParts.Length; idPartIndex++)
                if (!node.ChildNodes.TryGetValue(idParts[idPartIndex], out node))
                    return;

            result.AddRange(node.Items);
        }

        private void FindItemsByQueryParts(Node node, string[] queryParts, int queryPartsStartIndex, List<TItem> result)
        {
            for (var queryPartIndex = queryPartsStartIndex; queryPartIndex < queryParts.Length; queryPartIndex++)
            {
                var queryPart = queryParts[queryPartIndex];

                if (queryPart == "**")
                {
                    node.GetAllItems(result);

                    return;
                }

                if (queryPart == "*")
                {
                    foreach (var childNode in node.ChildNodes.GetValues())
                        FindItemsByQueryParts(childNode, queryParts, queryPartIndex + 1, result);

                    return;
                }

                if (Contains(queryPart, '|'))
                {
                    var orQueryParts = queryPart.Split('|');

                    foreach (var childNode in node.ChildNodes.GetPairs())
                    {
                        for (var orQueryPartIndex = 0; orQueryPartIndex < orQueryParts.Length; orQueryPartIndex++)
                        {
                            var orQueryPart = orQueryParts[orQueryPartIndex];
                            if (childNode.Key == orQueryPart)
                                FindItemsByQueryParts(childNode.Value, queryParts, queryPartIndex + 1, result);
                        }
                    }

                    return;
                }

                if (queryPart[0] == '^')
                {
                    var notQueryPart = queryPart.Substring(1);

                    foreach (var childNode in node.ChildNodes.GetPairs())
                        if (childNode.Key != notQueryPart)
                            FindItemsByQueryParts(childNode.Value, queryParts, queryPartIndex + 1, result);

                    return;
                }

                if (!node.ChildNodes.TryGetValue(queryPart, out node))
                {
                    return;
                }
            }

            result.AddRange(node.Items);
        }

        private static bool Contains(string str, char character)
        {
            for (var i = 0; i < str.Length; i++)
                if (str[i] == character)
                    return true;

            return false;
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            var result = new List<TItem>();

            _rootNode.GetAllItems(result);

            return result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected class Node
        {
            public IIndexable Owner
            {
                get;
            }

            public NodeDictionary ChildNodes
            {
                get;
            }

            public List<TItem> Items
            {
                get;
            }

            public Node(IIndexable owner)
            {
                Owner = owner;
                ChildNodes = new NodeDictionary();
                Items = new List<TItem>();
            }

            public Node(IIndexable owner, Node source)
            {
                Owner = owner;
                ChildNodes = new NodeDictionary(source.ChildNodes);
                Items = new List<TItem>(source.Items);
            }

            public void GetAllItems(List<TItem> result)
            {
                // Performance.MarkStart("ItemCollection.Node.GetAllItems");

                var nodes = new Stack<Node>();
                nodes.Push(this);

                while (nodes.Count > 0)
                {
                    var node = nodes.Pop();

                    if (node.Items.Count > 0)
                        result.AddRange(node.Items);

                    if (node.ChildNodes.GetCount() > 0)
                    {
                        foreach (var childNode in node.ChildNodes.GetValues())
                        {
                            if (childNode.ChildNodes.GetCount() == 0)
                            {
                                if (childNode.Items.Count > 0)
                                    result.AddRange(childNode.Items);

                                continue;
                            }

                            nodes.Push(childNode);
                        }
                    }
                }

                // Performance.MarkEnd("ItemCollection.Node.GetAllItems");
            }

            public Node Copy(IIndexable newOwner)
            {
                // Performance.MarkStart("ItemCollection.Node.Copy");

                var copy = new Node(newOwner, this);

                // Performance.MarkEnd("ItemCollection.Node.Copy");

                return copy;
            }
        }

        protected class NodeDictionary
        {
            private const int Capacity = 8;
            private Bucket[] _buckets;
            private int _size = 0;
            private NodeDictionary _source;

            public NodeDictionary()
            {
                _buckets = new Bucket[Capacity];
            }

            public NodeDictionary(NodeDictionary source)
            {
                _buckets = source._buckets;
                _size = source._size;
                _source = source;
            }

            public int GetCount()
            {
                return _size;
            }

            public void Set(string key, Node value)
            {
                if (_source != null)
                {
                    _buckets = new Bucket[Capacity];

                    for (var i = 0; i < Capacity; i++)
                    {
                        var sourceBucket = _source._buckets[i];

                        if (sourceBucket != null)
                            _buckets[i] = new Bucket(sourceBucket);
                    }

                    _source = null;
                }

                var hashCode = key.GetHashCode() & 0x7FFFFFFF;
                var index = hashCode % Capacity;

                var bucket = _buckets[index];

                if (bucket == null) {
                    bucket = new Bucket();
                    _buckets[index] = bucket;
                }

                if (bucket.Set(key, value))
                    _size++;
            }

            public bool TryGetValue(string key, out Node value)
            {
                var hashCode = key.GetHashCode() & 0x7FFFFFFF;
                var index = hashCode % Capacity;

                var bucket = _buckets[index];

                if (bucket == null)
                {
                    value = null;
                    return false;
                }

                return bucket.TryGetValue(key, out value);
            }

            public IEnumerable<Node> GetValues()
            {
                for (var i = 0; i < Capacity; i++)
                {
                    var bucket = _buckets[i];

                    if (bucket != null)
                        foreach (var value in bucket.GetValues())
                            yield return value;
                }
            }

            public IEnumerable<KeyValuePair<string, Node>> GetPairs()
            {
                for (var i = 0; i < Capacity; i++)
                {
                    var bucket = _buckets[i];

                    if (bucket != null)
                        foreach (var pair in bucket.GetPairs())
                            yield return pair;
                }
            }

            private class Bucket
            {
                private int _capacity = 1;
                private int _size = 0;
                private string[] _keys;
                private Node[] _values;
                private Bucket _source;

                public Bucket()
                {
                    _keys = new string[_capacity];
                    _values = new Node[_capacity];
                }

                public Bucket(Bucket source)
                {
                    _capacity = source._capacity;
                    _size = source._size;
                    _keys = source._keys;
                    _values = source._values;

                    _source = source;
                }

                public int GetCount()
                {
                    return _size;
                }

                public bool Set(string key, Node value)
                {
                    if (_source != null)
                    {
                        _keys = new string[_capacity];
                        _values = new Node[_capacity];

                        Array.Copy(_source._keys, _keys, _size);
                        Array.Copy(_source._values, _values, _size);

                        _source = null;
                    }

                    for (var i = 0; i < _size; i++)
                    {
                        if (_keys[i] == key)
                        {
                            _values[i] = value;
                            return false;
                        }
                    }

                    if (_size == _capacity)
                    {
                        _capacity *= 2;

                        var oldKeys = _keys;
                        var oldValues = _values;

                        _keys = new string[_capacity];
                        _values = new Node[_capacity];

                        Array.Copy(oldKeys, _keys, _size);
                        Array.Copy(oldValues, _values, _size);
                    }

                    _keys[_size] = key;
                    _values[_size] = value;
                    _size++;

                    return true;
                }

                public bool TryGetValue(string key, out Node value)
                {
                    for (var i = 0; i < _size; i++)
                    {
                        if (_keys[i] == key)
                        {
                            value = _values[i];
                            return true;
                        }
                    }

                    value = null;
                    return false;
                }

                public IEnumerable<Node> GetValues()
                {
                    for (var i = 0; i < _size; i++)
                        yield return _values[i];
                }

                public IEnumerable<KeyValuePair<string, Node>> GetPairs()
                {
                    for (var i = 0; i < _size; i++)
                        yield return new KeyValuePair<string, Node>(_keys[i], _values[i]);
                }
            }
        }
    }
}
