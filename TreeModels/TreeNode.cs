using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace JsonDataTreeVisualizer.TreeModels
{
    [DebuggerDisplay("Key = {Key}, ParentNode = {ParentNode?.Key}")]
    public class TreeNode
    {
        public Guid? ID { get; set; }
        public Guid? ParentID { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public string StringValue { get; set; }
        public JsonValueKind ValueKind { get; set; }
        public int Level { get; set; }
        public TreeNode ParentNode { get; set; }
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();

        public static TreeNode CreateFinalNode(string key, object value, JsonValueKind valueKind, int level, Guid? parentId)
            => new()
            {
                Key = key,
                Value = value,
                StringValue = value?.ToString(),
                ValueKind = valueKind,
                Level = level,
                ParentID = parentId
            };

        public static TreeNode CreateObjectNode(string key, int level, Guid id, Guid? parentId)
            => new()
            {
                Key = key,
                Level = level,
                ValueKind = JsonValueKind.Object,
                ID = id,
                ParentID = parentId
            };

        public TreeNode OrderSubNodes()
        {
            Children = Children.OrderBy(x => x.Children.Count).ToList();

            for (int i = 0; i < Children.Count; i++)
                Children[i] = Children[i].OrderSubNodes();

            return this;
        }
        public TreeNode AddChild(TreeNode node)
        {
            node.ParentNode = this;
            Children.Add(node);
            return node;
        }
        internal TreeNode[] AddChildren(TreeNode[] children)
        {
            return children.Select(AddChild).ToArray();
        }

        internal Dictionary<string, object> ToDictionary()
        {
            var dic = new Dictionary<string, object>();
            foreach (var node in Children)
            {
                dic[node.Key] = node.Children.Count == 0 ? GetTypedValue(node.StringValue, node.ValueKind) : node.ToDictionary();
            }

            return dic;
        }

        private object GetTypedValue(string value, JsonValueKind valueKind)
        {
            switch (valueKind)
            {
                case JsonValueKind.Undefined:
                    break;
                case JsonValueKind.Object:
                    break;
                case JsonValueKind.Array:
                    break;
                case JsonValueKind.String:
                    break;
                case JsonValueKind.Number:
                    return double.Parse(value);
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return value.ToString() == "True";
                case JsonValueKind.Null:
                    break;
                default:
                    break;
            }
            return value;
        }

    }
}