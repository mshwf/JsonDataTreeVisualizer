using JsonDataTreeVisualizer.ViewComponents;
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
        public Guid? Id { get; set; }
        public Guid? WrappingNodeId { get; set; }

        public static SmartNode CreateFinalNode(string key, object value, JsonValueKind valueKind, int level)
            => new()
            {
                Key = key,
                Value = value,
                ValueKind = valueKind,
                IsFinal = true,
                Level = level
            };

        public static SmartNode CreateObjectNode(string key, int level, Guid? wrappingId)
            => new()
            {
                Key = key,
                Level = level,
                Id = Guid.NewGuid(),
                WrappingNodeId = wrappingId
            };

        public SmartNode OrderSubNodes()
        {
            SubNodes = SubNodes.OrderBy(x => !x.IsFinal).ToList();

            for (int i = 0; i < SubNodes.Count; i++)
                SubNodes[i] = SubNodes[i].OrderSubNodes();

            return this;
        }
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
        public string ParentName { get; set; }
        public Guid? WrappingId { get; set; }

        public SimpleDataNode(SmartNode node)
        {
            if (node == null) return;
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

        [BindProperty]
        public SmartNode HeadNode { get; set; }
        [BindProperty]
        public SmartNode HeadNodeValues { get; set; }
        [BindProperty]
        public List<SimpleDataNode> FlattenedNodes { get; set; } = new List<SimpleDataNode>();
        public List<NodeGroup> NodeGroups { get; set; } = new List<NodeGroup>();

        public void OnGet()
        {
            var jsonObject = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(JsonSamples.ComplexMultiLevelJson);
            HeadNode = CreateJsonTree(jsonObject, null);
            FlattenNodes(HeadNode, FlattenedNodes, null);
            NodeGroups = GroupByLevelOrdered(FlattenedNodes);
        }

        private static SmartNode CreateJsonTree(Dictionary<string, JsonElement> dic, string fallbackObjectName)
        {
            SmartNode parentNode = SmartNode.CreateObjectNode(fallbackObjectName, 0, Guid.NewGuid());
            PropagateNodeTree(dic, parentNode, parentNode);
            return parentNode;
        }

        private static void PropagateNodeTree(Dictionary<string, JsonElement> dic, SmartNode parentNode, SmartNode grandParent)
        {
            int level = parentNode.Level + 1;
            foreach (var item in dic)
            {
                SmartNode node = null;
                switch (item.Value.ValueKind)
                {
                    case JsonValueKind.Object:
                        node = SmartNode.CreateObjectNode(item.Key, level, parentNode.Id);
                        node.ParentNode = parentNode;
                        node.WrappingNodeId = parentNode.Id;
                        var subDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(item.Value.GetRawText());
                        PropagateNodeTree(subDict, node, parentNode);
                        parentNode.SubNodes.Add(node);
                        break;
                    case JsonValueKind.String:
                        node = SmartNode.CreateFinalNode(item.Key, item.Value.GetString(), item.Value.ValueKind, level);
                        node.ParentNode = parentNode.ParentNode;
                        node.WrappingNodeId = grandParent.Id;
                        parentNode.SubNodes.Add(node);
                        break;
                    case JsonValueKind.Number:
                        node = SmartNode.CreateFinalNode(item.Key, item.Value.GetDouble(), item.Value.ValueKind, level);
                        node.ParentNode = parentNode.ParentNode;
                        node.WrappingNodeId = grandParent.Id;
                        parentNode.SubNodes.Add(node);
                        break;
                    case JsonValueKind.True:
                        node = SmartNode.CreateFinalNode(item.Key, true, item.Value.ValueKind, level);
                        node.ParentNode = parentNode.ParentNode;
                        node.WrappingNodeId = grandParent.Id;
                        parentNode.SubNodes.Add(node);
                        break;
                    case JsonValueKind.False:
                        node = SmartNode.CreateFinalNode(item.Key, false, item.Value.ValueKind, level);
                        node.ParentNode = parentNode.ParentNode;
                        node.WrappingNodeId = grandParent.Id;
                        parentNode.SubNodes.Add(node);
                        break;
                    case JsonValueKind.Null:
                        node = SmartNode.CreateFinalNode(item.Key, null, item.Value.ValueKind, level);
                        node.ParentNode = parentNode.ParentNode;
                        node.WrappingNodeId = grandParent.Id;
                        parentNode.SubNodes.Add(node);
                        break;
                    case JsonValueKind.Undefined:
                    case JsonValueKind.Array:
                        break;
                    default:
                        break;
                }
            }
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

        private void FlattenNodes(SmartNode node,
            List<SimpleDataNode> flattened,
            SmartNode wrappingNode)
        {
            if (node.IsFinal)
            {
                var newnode = new SimpleDataNode(node)
                {
                    Parent = new SimpleDataNode(node.ParentNode),
                    ParentName = node.ParentNode?.ParentNode?.Key,
                    GroupName = wrappingNode.Key,
                    WrappingId = node.WrappingNodeId
                };
                flattened.Add(newnode);
            }
            else
            {
                foreach (var subNode in node.SubNodes)
                {
                    FlattenNodes(subNode, flattened, node);
                }
            }
        }



        public void OnPost()
        {
            string jsn = ToJson();
        }

        private string ToJson()
        {
            var descendantsTree = GroupByLevelOrdered(FlattenedNodes);

            //var generationsTree = descendantsTree.GroupBy(
            //    nodeGroup => new { nodeGroup.Header.Level },
            //    nodeGroup2 => new { nodeGroup2.Nodes, nodeGroup2.Header.GroupName })
            //        .Select(group =>
            //    new NodesBatches
            //    {
            //        Header = new GroupHeader
            //        {
            //            Level = group.Key.Level,
            //            GroupName = group.FirstOrDefault()?.GroupName
            //        },
            //        Nodes = group.Select(x => x.Nodes).ToList()
            //    }).ToList();

            var generationsTree = descendantsTree.GroupBy(
                nodeGroup => new { nodeGroup.Header.Level },
                nodeGroup2 => new { nodeGroup2.Nodes, nodeGroup2.Header.GroupName })
                    .Select(group =>
                new NodesBatches
                {
                    Header = new GroupHeader
                    {
                        Level = group.Key.Level,
                        GroupName = group.FirstOrDefault()?.GroupName,
                    },
                    NodeGroups = group.Select(x => x.Nodes).ToList()
                }).ToList();

            var topJson = new Dictionary<string, object>();
            //Dictionary<string, object> _cache = null;
            List<string> groupsPre = null;
            NodesBatches _cacheBatches = null;

            foreach (var generation in generationsTree)
            {
                foreach (var group in generation.NodeGroups)
                {
                    var jsonLevel = new Dictionary<string, object>();
                    foreach (var node in group)
                    {
                        jsonLevel[node.Key] = GetTypedValue(node.Value, node.ValueKind);
                    }
                    if (_cacheBatches != null)
                    {
                        //_cache[generation.Header.GroupName] = jsonLevel;
                        //_cacheBatches.Nodes.Where(x=>x.)
                        topJson[generation.Header.GroupName] = jsonLevel;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(generation.Header.GroupName))
                        {
                            foreach (var item in jsonLevel)
                            {
                                topJson[item.Key] = item.Value;
                            }
                        }
                        else
                            topJson[generation.Header.GroupName] = jsonLevel;
                        //_cache = jsonLevel;
                    }

                }
                //_cacheBatches = generation;

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

    class NodesBatches
    {
        public GroupHeader Header { get; set; }
        public List<List<SimpleDataNode>> NodeGroups { get; set; }
    }

    class NodesByParent : List<SimpleDataNode>
    {
        //public List<SimpleDataNode> Nodes { get; set; }
    }
}