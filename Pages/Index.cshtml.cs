using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace JsonDataTreeVisualizer.Pages
{

    [DebuggerDisplay("GroupName = {GroupName}, Level = {Level}")]
    public class GroupHeader
    {
        public int Level { get; set; }
        public string GroupName { get; set; }
    }

    [DebuggerDisplay("Header = {Header.GroupName}, Nodes = {Nodes.Count}")]
    public class NodeGroup
    {
        public GroupHeader Header { get; set; }
        public List<SimpleDataNode> Nodes { get; set; }
    }
    [DebuggerDisplay("Key = {Key}, ParentNode = {ParentNode?.Key}")]
    public class SmartNode
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public JsonValueKind ValueKind { get; set; }
        public bool IsFinal { get; set; }
        public int Level { get; set; }
        public SmartNode ParentNode { get; set; }
        public List<SmartNode> SubNodes { get; set; } = new List<SmartNode>();
        public static SmartNode CreateFinalNode(string key, object value, JsonValueKind valueKind, int level)
            => new()
            {
                Key = key,
                Value = value,
                ValueKind = valueKind,
                IsFinal = true,
                Level = level
            };

        public static SmartNode CreateObjectNode(string key, int level)
            => new()
            {
                Key = key,
                Level = level
            };
    }

    [DebuggerDisplay("Key = {Key}, Value = {Value}, Level = {Level}")]
    public class SimpleDataNode
    {
        public SimpleDataNode() { }//required for model binding to work

        public string Key { get; set; }
        public string Value { get; set; }
        public int Level { get; set; }
        public JsonValueKind ValueKind { get; set; }
        public string GroupName { get; set; }
        public SimpleDataNode Parent { get; set; }
        public SimpleDataNode(SmartNode node)
        {
            Key = node.Key;
            Value = node.Value?.ToString();
            Level = node.Level;
            ValueKind = node.ValueKind;
            if (node.ParentNode != null)
                Parent = new SimpleDataNode(node.ParentNode);
        }
    }

    public class IndexModel : PageModel
    {
        public IndexModel()
        {
        }

        public SmartNode HeadNode { get; set; }
        [BindProperty]
        public List<SimpleDataNode> FlattenedNodes { get; set; } = new List<SimpleDataNode>();
        public List<NodeGroup> NodeGroups { get; set; } = new List<NodeGroup>();

        public void OnGet()
        {
            var dic = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(JsonSamples.ComplexMultiLevelJson);

            var unorderedSmartNode = CreateNodeTree(dic, "Configurations");
            HeadNode = OrderSubNodes(unorderedSmartNode);
            FlatenNodes(HeadNode, FlattenedNodes, null);
            NodeGroups = GroupByLevelOrdered(FlattenedNodes);
        }

        private static List<NodeGroup> GroupByLevelOrdered(List<SimpleDataNode> flattenedNodes)
        {
            List<NodeGroup> nodeGroups = new();

            var gr = flattenedNodes.GroupBy(x => new { x.GroupName, x.Level })
                .ToDictionary(x => new GroupHeader { GroupName = x.Key.GroupName, Level = x.Key.Level }, v => v.ToList())
                .ToList();

            foreach (var item in gr)
            {
                nodeGroups.Add(new NodeGroup { Header = item.Key, Nodes = item.Value });
            }

            return nodeGroups;
        }

        private void FlatenNodes(SmartNode node,
            List<SimpleDataNode> flattened,
            SmartNode wrappingNode)
        {
            if (node.IsFinal)
            {
                var newnode = new SimpleDataNode(node)
                {
                    Parent = new SimpleDataNode(node.ParentNode),
                    GroupName = wrappingNode.Key
                };
                flattened.Add(newnode);
            }
            else
            {
                foreach (var subNode in node.SubNodes)
                {
                    FlatenNodes(subNode, flattened, node);
                }
            }
        }

        private SmartNode OrderSubNodes(SmartNode node)
        {
            node.SubNodes = node.SubNodes.OrderBy(x => !x.IsFinal).ToList();

            for (int i = 0; i < node.SubNodes.Count; i++)
                node.SubNodes[i] = OrderSubNodes(node.SubNodes[i]);

            return node;
        }

        private static SmartNode CreateNodeTree(Dictionary<string, JsonElement> dic, string fallbackObjectName)
        {
            SmartNode parentNode = SmartNode.CreateObjectNode(fallbackObjectName, 0);
            NodeCreator(dic, parentNode);
            return parentNode;
        }

        private static void NodeCreator(Dictionary<string, JsonElement> dic, SmartNode parentNode)
        {
            int level = parentNode.Level + 1;
            foreach (var item in dic)
            {
                SmartNode node = null;
                switch (item.Value.ValueKind)
                {
                    case JsonValueKind.Object:
                        node = SmartNode.CreateObjectNode(item.Key, level);
                        node.ParentNode = parentNode;
                        NodeCreator(JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(item.Value.GetRawText()), node);
                        parentNode.SubNodes.Add(node);
                        break;
                    case JsonValueKind.String:
                        node = SmartNode.CreateFinalNode(item.Key, item.Value.GetString(), item.Value.ValueKind, level);
                        node.ParentNode = parentNode.ParentNode;
                        parentNode.SubNodes.Add(node);
                        break;
                    case JsonValueKind.Number:
                        node = SmartNode.CreateFinalNode(item.Key, item.Value.GetDouble(), item.Value.ValueKind, level);
                        node.ParentNode = parentNode.ParentNode;
                        parentNode.SubNodes.Add(node);
                        break;
                    case JsonValueKind.True:
                        node = SmartNode.CreateFinalNode(item.Key, true, item.Value.ValueKind, level);
                        node.ParentNode = parentNode.ParentNode;
                        parentNode.SubNodes.Add(node);
                        break;
                    case JsonValueKind.False:
                        node = SmartNode.CreateFinalNode(item.Key, false, item.Value.ValueKind, level);
                        node.ParentNode = parentNode.ParentNode;
                        parentNode.SubNodes.Add(node);
                        break;
                    case JsonValueKind.Null:
                        node = SmartNode.CreateFinalNode(item.Key, null, item.Value.ValueKind, level);
                        node.ParentNode = parentNode.ParentNode;
                        parentNode.SubNodes.Add(node);
                        break;
                    case JsonValueKind.Undefined:
                    case JsonValueKind.Array:
                        break;
                    default:
                        break;
                }
                node.ParentNode = parentNode;
            }
        }


        public void OnPost()
        {
            string jsn = ToJson();
        }

        private string ToJson()
        {
            var descendantsTree = GroupByLevelOrdered(FlattenedNodes);

            var generationsTree = descendantsTree.GroupBy(x => new { x.Header.Level }, x => new { x.Nodes, x.Header.GroupName }).ToList();

            var topJson = new Dictionary<string, object>();

            for (int i = 0; i < generationsTree.Count; i++)
            {
                var generation = generationsTree[i];
                var allJsonLevel = new Dictionary<string, object>();
                foreach (var group in generation.ToList())
                {
                    var jsonLevel = new Dictionary<string, object>();
                    foreach (var node in group.Nodes)
                    {
                        jsonLevel[node.Key] = GetTypedValue(node.Value, node.ValueKind);
                    }
                    allJsonLevel[group.GroupName] = jsonLevel;
                }
                topJson[i.ToString()] = allJsonLevel;
            }

            var json = JsonSerializer.Serialize(topJson);
            return json;
        }

        private static object GetTypedValue(string value, JsonValueKind valueKind)
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
                    return value == "True";
                case JsonValueKind.Null:
                    break;
                default:
                    break;
            }
            return value;
        }
    }
}