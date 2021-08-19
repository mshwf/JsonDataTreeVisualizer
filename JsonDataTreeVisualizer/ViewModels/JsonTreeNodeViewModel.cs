using JsonTreeParser.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;

namespace JsonDataTreeVisualizer
{
    [DebuggerDisplay("Key: {Key}, Value: {Value}")]
    public class JsonTreeNodeViewModel
    {
        public Guid? ID { get; set; }
        public Guid? ParentID { get; set; }
        public string Key { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Value { get; set; }
        public JsonValueKind ValueKind { get; set; }
        public int Level { get; set; }

        public static JsonTreeNodeViewModel FromJsonTreeNode(JsonTreeNode node)
        {
            if (node == null) return null;
            return new()
            {
                ID = node.ID,
                Key = node.Key,
                Level = node.Level,
                ParentID = node.ParentID,
                ValueKind = node.ValueKind,
                Value = node.Value?.ToString()
            };
        }

        public static JsonTreeNode ToJsonTreeNode(JsonTreeNodeViewModel node)
        {
            if (node == null) return null;
            return new()
            {
                ID = node.ID,
                Key = node.Key,
                Level = node.Level,
                ParentID = node.ParentID,
                ValueKind = node.ValueKind,
                Value = GetTypedValue(node.Value, node.ValueKind)
            };
        }

        private static object GetTypedValue(string value, JsonValueKind valueKind)
        {
            switch (valueKind)
            {
                case JsonValueKind.Number:
                    return double.Parse(value);
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return value == bool.TrueString;
                default:
                    break;
            }
            return value;
        }
    }
}