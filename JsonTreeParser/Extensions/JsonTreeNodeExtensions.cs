using JsonTreeParser.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace JsonTreeParser.Extensions
{
    internal static class JsonTreeNodeExtensions
    {
        internal static void AddDescendants(this IReadOnlyCollection<JsonTreeNode> nodes, JsonTreeNode parent)
        {
            var children = parent.AddChildren(nodes.Where(n => n.ParentID == parent.ID).ToArray());

            foreach (var subnode in children)
            {
                nodes.AddDescendants(subnode);
            }
        }
        internal static void OrderByChildren(this JsonTreeNode node)
        {
            node.Children = node.Children.OrderBy(x => x.Children.Count).ToList();

            for (int i = 0; i < node.Children.Count; i++)
                node.Children[i].OrderByChildren();
        }
        internal static List<JsonTreeNode> FlattenNodes(this JsonTreeNode node)
        {
            List<JsonTreeNode> flattened = new();

            if (node.Children.Count == 0)
            {
                flattened.Add(node);
            }
            else
            {
                flattened.Add(node);
                foreach (var subNode in node.Children)
                {
                    flattened.AddRange(subNode.FlattenNodes());
                }
            }
            return flattened;
        }

        internal static object ToSerializableObject(this JsonTreeNode node)
        {
            object ser;
            if (node.ValueKind == JsonValueKind.Array)
            {
                object[] arr = new object[node.Children.Count];
                for (int i = 0; i < node.Children.Count; i++)
                {
                    var arrNode = node.Children[i];
                    var serObj = arrNode.ToSerializableObject();
                    arr[i] = serObj;
                }
                ser = arr;
            }
            else if (node.ValueKind == JsonValueKind.Object)
            {
                var dic = node.ToDictionary();
                ser = dic;
            }
            else
            {
                ser = GetTypedValue(node.StringValue, node.ValueKind);
            }
            return ser;
        }
        private static Dictionary<string, object> ToDictionary(this JsonTreeNode node)
        {
            var dic = new Dictionary<string, object>();

            foreach (var child in node.Children)
            {
                if (child.ValueKind != JsonValueKind.Array)
                    dic[child.Key] = child.Children.Count == 0 ? GetTypedValue(child.StringValue, child.ValueKind) : child.ToDictionary();
                else
                {
                    var serObj = child.ToSerializableObject();
                    dic[child.Key] = serObj;
                }
            }

            return dic;
        }
        private static object GetTypedValue(string value, JsonValueKind valueKind)
        {
            switch (valueKind)
            {
                case JsonValueKind.Number:
                    return double.Parse(value);
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return value.ToString() == "True";
                default:
                    break;
            }
            return value;
        }
    }
}