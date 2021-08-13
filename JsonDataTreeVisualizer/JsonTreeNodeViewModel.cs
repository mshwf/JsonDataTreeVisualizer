using JsonTreeParser.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace JsonDataTreeVisualizer
{
    [ModelBinder(BinderType = typeof(TreeNodeModelBinder), Name = "NodeModel")]

    public class JsonTreeNodeViewModel
    {
        public Guid? ID { get; set; }
        public Guid? ParentID { get; set; }
        public string Key { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public object Value { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
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
                Value = node.Value,
                ValueKind = node.ValueKind
            };
        }

        internal static JsonTreeNode ToJsonTreeNode(JsonTreeNodeViewModel node)
        {
            if (node == null) return null;
            return new()
            {
                ID = node.ID,
                Key = node.Key,
                Level = node.Level,
                ParentID = node.ParentID,
                Value = node.Value,
                ValueKind = node.ValueKind
            };
        }
    }
}
