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

                node.ChildNodes[idPart] = childNode;
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
                    foreach (var childNode in node.ChildNodes.Values)
                        FindItemsByQueryParts(childNode, queryParts, queryPartIndex + 1, result);

                    return;
                }

                if (Contains(queryPart, '|'))
                {
                    var orQueryParts = queryPart.Split('|');

                    foreach (var childNode in node.ChildNodes)
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

                    foreach (var childNode in node.ChildNodes)
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

            public Dictionary<string, Node> ChildNodes
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
                ChildNodes = new Dictionary<string, Node>();
                Items = new List<TItem>();
            }

            public Node(IIndexable owner, Node source)
            {
                Owner = owner;
                ChildNodes = new Dictionary<string, Node>(source.ChildNodes);
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

                    if (node.ChildNodes.Count > 0)
                    {
                        foreach (var childNode in node.ChildNodes.Values)
                        {
                            if (childNode.ChildNodes.Count == 0)
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
    }
}
