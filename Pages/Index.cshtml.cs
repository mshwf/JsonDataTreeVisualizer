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

    public class NodeGroup
    {
        public GroupHeader Header { get; set; }
        public List<SmartNodeViewModel> Nodes { get; set; }
    }
    public class SmartNode
    {
        public string Key { get; }
        public object Value { get; }
        public JsonValueKind ValueKind { get; set; }
        public bool IsFinal { get; set; }
        public int Level { get; set; }
        public List<SmartNode> SubNodes { get; set; } = new List<SmartNode>();

        public SmartNode(string key, object value, JsonValueKind valueKind, int level)
        {
            Key = key;
            Value = value;
            ValueKind = valueKind;
            IsFinal = true;
            Level = level;
        }
        public SmartNode(string key, int level)
        {
            Key = key;
            Level = level;
        }
    }

    [DebuggerDisplay("Key = {Key}, Value = {Value}, Level = {Level}")]
    public class SmartNodeViewModel
    {
        public SmartNodeViewModel() { }//required for model binding to work

        public string Key { get; set; }
        public string Value { get; set; }
        public int Level { get; set; }
        public JsonValueKind ValueKind { get; set; }
        public string GroupName { get; set; }
        public SmartNodeViewModel Parent { get; set; }
        public SmartNodeViewModel(SmartNode node)
        {
            Key = node.Key;
            Value = node.Value?.ToString();
            Level = node.Level;
            ValueKind = node.ValueKind;
        }
    }

    public class IndexModel : PageModel
    {
        public IndexModel()
        {
        }

        public SmartNode HeadNode { get; set; }
        [BindProperty]
        public List<SmartNodeViewModel> FlattenedNodes { get; set; } = new List<SmartNodeViewModel>();
        public List<NodeGroup> NodeGroups { get; set; } = new List<NodeGroup>();

        public void OnGet()
        {
            var dic = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(JsonSamples.ComplexMultiLevelJson);

            var unorderedSmartNode = CreateNodeTree(dic, "Configurations");
            HeadNode = OrderSubNodes(unorderedSmartNode);
            FlatenNodes(HeadNode, FlattenedNodes, null);
            NodeGroups = GroupByLevelOrdered(FlattenedNodes);
        }

        private static List<NodeGroup> GroupByLevelOrdered(List<SmartNodeViewModel> flattenedNodes)
        {
            List<NodeGroup> nodeGroups = new();

            var gr = flattenedNodes.GroupBy(x => new { x.GroupName, x.Level })
                .ToDictionary(x => new GroupHeader { GroupName = x.Key.GroupName, Level = x.Key.Level }, v => v.ToList())
                .ToList();

            foreach (var item in gr)
            {
                nodeGroups.Add(new NodeGroup { Header = item.Key, Nodes = item.Value });
            }
            nodeGroups = nodeGroups.OrderBy(x => x.Header.Level).ToList();

            return nodeGroups;
        }

        private void FlatenNodes(SmartNode node,
            List<SmartNodeViewModel> flattened,
            SmartNode parentNode)
        {
            if (node.IsFinal)
            {
                var newnode = new SmartNodeViewModel(node)
                {
                    Parent = new SmartNodeViewModel(parentNode),
                    GroupName = parentNode.Key
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

        private static SmartNode CreateNodeTree(Dictionary<string, JsonElement> dic, string topNodeName)
        {
            SmartNode parentNode = new(topNodeName, 0);
            NodeCreator(dic, parentNode);
            return parentNode;
        }

        private static void NodeCreator(Dictionary<string, JsonElement> dic, SmartNode parentNode)
        {
            int level = parentNode.Level + 1;
            foreach (var item in dic)
            {
                SmartNode node;
                switch (item.Value.ValueKind)
                {
                    case JsonValueKind.Object:
                        node = new SmartNode(item.Key, level);
                        NodeCreator(JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(item.Value.GetRawText()), node);
                        parentNode.SubNodes.Add(node);
                        break;
                    case JsonValueKind.String:
                        node = new(item.Key, item.Value.GetString(), item.Value.ValueKind, level);
                        parentNode.SubNodes.Add(node);
                        break;
                    case JsonValueKind.Number:
                        node = new(item.Key, item.Value.GetDouble(), item.Value.ValueKind, level);
                        parentNode.SubNodes.Add(node);
                        break;
                    case JsonValueKind.True:
                        node = new(item.Key, true, item.Value.ValueKind, level);
                        parentNode.SubNodes.Add(node);
                        break;
                    case JsonValueKind.False:
                        node = new(item.Key, false, item.Value.ValueKind, level);
                        parentNode.SubNodes.Add(node);
                        break;

                    case JsonValueKind.Null:
                    case JsonValueKind.Undefined:
                    case JsonValueKind.Array:
                        break;
                    default:
                        break;
                }
            }
        }


        public void OnPost()
        {
            var orderedGroup = GroupByLevelOrdered(FlattenedNodes);
            string jsn = ToJson(orderedGroup);
        }

        private static string ToJson(List<NodeGroup> groupsTree)
        {
            var hierarchicalGroup = groupsTree.GroupBy(x => new { x.Header.Level }, x => new { x.Nodes, x.Header.GroupName }).ToList();

            var topJson = new Dictionary<string, object>();
            var lst = new List<Dictionary<string, object>>();

            for (int i = 0; i < hierarchicalGroup.Count; i++)
            {
                var levelGroup = hierarchicalGroup[i];
                var allJsonLevel = new Dictionary<string, object>();
                foreach (var group in levelGroup.ToList())
                {
                    var jsonLevel = new Dictionary<string, object>();
                    foreach (var node in group.Nodes)
                    {
                        jsonLevel[node.Key] = GetTypedValue(node.Value, node.ValueKind);
                    }
                    allJsonLevel[group.GroupName] = jsonLevel;
                }
                lst.Add(allJsonLevel);
            }

            var json = JsonSerializer.Serialize(topJson);
            var json2 = JsonSerializer.Serialize(lst);
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