using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace JsonDataTreeVisualizer.TreeModels
{
    [DebuggerDisplay("Key = {Key}, Value = {Value}")]
    public class TreeNode
    {
        public Guid? ID { get; set; }
        public Guid? ParentID { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public string StringValue { get; set; }
        public JsonValueKind ValueKind { get; set; }
        public int Level { get; set; }
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

        public static TreeNode CreateObjectNode(string key, int level, Guid? parentId)
            => new()
            {
                Key = key,
                Level = level,
                ValueKind = JsonValueKind.Object,
                ID = Guid.NewGuid(),
                ParentID = parentId
            };

        internal static TreeNode CreateArrayNode(string key, int level, Guid? parentId)
            => new()
            {
                Key = key,
                Level = level,
                ValueKind = JsonValueKind.Array,
                ID = Guid.NewGuid(),
                ParentID = parentId
            };

        public void OrderSubNodes()
        {
            Children = Children.OrderBy(x => x.Children.Count).ToList();

            for (int i = 0; i < Children.Count; i++)
                Children[i].OrderSubNodes();
        }
        public TreeNode AddChild(TreeNode node)
        {
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
                if (node.ValueKind != JsonValueKind.Array)
                    dic[node.Key] = node.Children.Count == 0 ? GetTypedValue(node.StringValue, node.ValueKind) : node.ToDictionary();
                else
                {
                    var ff = node.ToSerializableObject();
                    dic[node.Key] = ff;
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

        internal object ToSerializableObject()
        {
            object ser;
            if (ValueKind == JsonValueKind.Array)
            {
                object[] arr = new object[Children.Count];
                for (int i = 0; i < Children.Count; i++)
                {
                    var arrNode = Children[i];
                    var serObj = arrNode.ToSerializableObject();
                    arr[i] = serObj;
                }
                ser = arr;
            }
            else if (ValueKind == JsonValueKind.Object)
            {
                var dic = ToDictionary();
                ser = dic;
            }
            else
            {
                ser = StringValue;
            }
            return ser;
        }
    }
}