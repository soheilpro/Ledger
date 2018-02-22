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
            var bucket = _rootNode;

            foreach (var idPart in accountIdParts)
            {
                Node childNode;

                if (bucket.ChildNodes.TryGetValue(idPart, out childNode))
                    childNode = CopyNode(childNode);
                else
                    childNode = new Node(_owner);

                bucket.ChildNodes[idPart] = childNode;
                bucket = childNode;
            }

            if (existingItem != null)
                bucket.Items.Remove(existingItem);

            bucket.Items.Add(item);

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

            ICollection<TItem> result;

            if (account is Account)
            {
                result = FindItemsByIdParts(_rootNode, ((Account)account).IdParts);
            }
            else
            {
                result = _rootNode.GetAllItems().Where(item => account.Equals(item.Account)).ToList();
            }

            // Performance.MarkEnd("ItemCollection.FindItems(account)");

            return result;
        }

        protected ICollection<TItem> FindItems(IAccountPredicate predicate)
        {
            // Performance.MarkStart("ItemCollection.FindItems(predicate)");

            ICollection<TItem> result;

            if (predicate is QueryAccountPredicate)
            {
                result = FindItemsByQueryParts(_rootNode, ((QueryAccountPredicate)predicate).QueryParts);
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
                result = _rootNode.GetAllItems().Where(item => predicate.Matches(item.Account)).ToList();
            }

            // Performance.MarkEnd("ItemCollection.FindItems(predicate)");

            return result;
        }

        private ICollection<TItem> FindItemsByIdParts(Node node, ICollection<string> idParts)
        {
            foreach (var idPart in idParts)
                if (!node.ChildNodes.TryGetValue(idPart, out node))
                    return new List<TItem>();

            return node.Items;
        }

        private ICollection<TItem> FindItemsByQueryParts(Node node, ICollection<string> queryParts)
        {
            var index = 0;

            foreach (var queryPart in queryParts)
            {
                if (queryPart.Equals("**", StringComparison.Ordinal))
                    return node.GetAllItems();

                if (queryPart.Equals("*", StringComparison.Ordinal))
                {
                    var items = new List<TItem>();

                    foreach (var childNode in node.ChildNodes)
                        items.AddRange(FindItemsByQueryParts(childNode.Value, queryParts.Skip(index + 1).ToArray()));

                    return items;
                }

                if (queryPart.Contains('|'))
                {
                    var items = new List<TItem>();
                    var orQueryParts = queryPart.Split('|');

                    foreach (var childNode in node.ChildNodes)
                        foreach (var orQueryPart in orQueryParts)
                            if (childNode.Key == orQueryPart)
                                items.AddRange(FindItemsByQueryParts(childNode.Value, queryParts.Skip(index + 1).ToArray()));

                    return items;
                }

                if (queryPart.StartsWith("^", StringComparison.Ordinal))
                {
                    var items = new List<TItem>();
                    var notQueryPart = queryPart.Substring(1);

                    foreach (var childNode in node.ChildNodes)
                        if (childNode.Key != notQueryPart)
                            items.AddRange(FindItemsByQueryParts(childNode.Value, queryParts.Skip(index + 1).ToArray()));

                    return items;
                }

                if (!node.ChildNodes.TryGetValue(queryPart, out node))
                    return new List<TItem>();

                index++;
            }

            return node.Items;
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            var items = _rootNode.GetAllItems();

            return items.GetEnumerator();
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

            public ICollection<TItem> GetAllItems()
            {
                // Performance.MarkStart("ItemCollection.Node.GetAllItems");

                var items = new List<TItem>();

                var buckets = new Stack<Node>();
                buckets.Push(this);

                while (buckets.Count > 0)
                {
                    var node = buckets.Pop();

                    items.AddRange(node.Items);

                    foreach (var childNode in node.ChildNodes.Values)
                        buckets.Push(childNode);
                }

                // Performance.MarkEnd("ItemCollection.Node.GetAllItems");

                return items;
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
