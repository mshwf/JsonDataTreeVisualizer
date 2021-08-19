using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace JsonTreeParser.Core
{
    public class JsonTreeNode
    {
        public Guid? ID { get; set; }
        public Guid? ParentID { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public JsonValueKind ValueKind { get; set; }
        public int Level { get; set; }
        
        internal List<JsonTreeNode> Children { get; set; } = new List<JsonTreeNode>();
        internal JsonTreeNode AddChild(JsonTreeNode node)
        {
            Children.Add(node);
            return node;
        }
        internal JsonTreeNode[] AddChildren(JsonTreeNode[] children)
        {
            return children.Select(AddChild).ToArray();
        }

        internal static JsonTreeNode CreateFinalNode(string key, int level, Guid? parentId, object value, JsonValueKind valueKind)
            => new()
            {
                Key = key,
                Value = value,
                ValueKind = valueKind,
                Level = level,
                ParentID = parentId
            };
        internal static JsonTreeNode CreateObjectNode(string key, int level, Guid? parentId)
            => new()
            {
                Key = key,
                Level = level,
                ValueKind = JsonValueKind.Object,
                ID = Guid.NewGuid(),
                ParentID = parentId
            };
        internal static JsonTreeNode CreateArrayNode(string key, int level, Guid? parentId)
            => new()
            {
                Key = key,
                Level = level,
                ValueKind = JsonValueKind.Array,
                ID = Guid.NewGuid(),
                ParentID = parentId
            };
    }
}