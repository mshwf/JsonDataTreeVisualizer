using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace JsonDataTreeVisualizer
{
    public class TreeNodeHelper
    {
        public static TreeNode CreateJsonTree(JsonElement jsonElement, string fallbackObjectName, Guid? parentId)
        {
            TreeNode parentNode;
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.Array:
                    parentNode = TreeNode.CreateArrayNode(key: fallbackObjectName, level: 0, parentId: parentId);
                    foreach (var childJsonElement in jsonElement.EnumerateArray())
                    {
                        var node = CreateJsonTree(childJsonElement, null, parentNode.ID);
                        parentNode.AddChild(node);
                    }
                    break;
                case JsonValueKind.Object:
                    parentNode = TreeNode.CreateObjectNode(key: fallbackObjectName, level: 0, parentId: parentId);
                    var objectDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonElement.GetRawText());
                    PropagateNodeTree(objectDict, parentNode);
                    break;
                default:
                    parentNode = TreeNode.CreateFinalNode(key: fallbackObjectName, GetSimpleObjectValue(jsonElement), jsonElement.ValueKind, level: 0, parentId: parentId);
                    break;
            }
            return parentNode;
        }
        public static string FromFlattenedTreeToJson(List<TreeNode> flattenedNodes)
        {
            var root = flattenedNodes.First(x => x.ParentID == null);
            AddDescendants(flattenedNodes.Where(n => n.ParentID != null).ToList(), root);
            object jsonObj = root.ToSerializableObject();
            var json = JsonSerializer.Serialize(jsonObj);
            return json;
        }
        public static TreeNode FromJsonToTree(string json, bool orderByChildrenCount)
        {
            var element = JsonSerializer.Deserialize<JsonElement>(json);
            var headNode = CreateJsonTree(element, null, null);
            if (orderByChildrenCount)
                headNode.OrderByChildren();
            return headNode;
        }
        public static List<TreeNode> FromJsonToFlattenedTree(string json, bool orderByChildrenCount)
        {

            var flattenedNodes = new List<TreeNode>();
            var headNode = FromJsonToTree(json, orderByChildrenCount);
            headNode.FlattenNodes(flattenedNodes);
            return flattenedNodes;
        }

        private static void PropagateNodeTree(Dictionary<string, JsonElement> dic, TreeNode parentNode)
        {
            int level = parentNode.Level + 1;
            foreach (var item in dic)
            {
                TreeNode node;
                switch (item.Value.ValueKind)
                {
                    case JsonValueKind.Object:
                        node = TreeNode.CreateObjectNode(item.Key, level, parentId: parentNode.ID);
                        var subDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(item.Value.GetRawText());
                        PropagateNodeTree(subDict, node);
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.String:
                        node = TreeNode.CreateFinalNode(item.Key, item.Value.GetString(), item.Value.ValueKind, level, parentId: parentNode.ID);
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.Number:
                        node = TreeNode.CreateFinalNode(item.Key, item.Value.GetDouble(), item.Value.ValueKind, level, parentId: parentNode.ID);
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.True:
                        node = TreeNode.CreateFinalNode(item.Key, true, item.Value.ValueKind, level, parentId: parentNode.ID);
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.False:
                        node = TreeNode.CreateFinalNode(item.Key, false, item.Value.ValueKind, level, parentId: parentNode.ID);
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.Null:
                        node = TreeNode.CreateFinalNode(item.Key, null, item.Value.ValueKind, level, parentId: parentNode.ID);
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.Array:
                        node = TreeNode.CreateArrayNode(item.Key, level, parentId: parentNode.ID);

                        foreach (var jsonElement in item.Value.EnumerateArray())
                        {
                            TreeNode nodeEl;
                            if (jsonElement.ValueKind == JsonValueKind.Object)
                            {
                                nodeEl = TreeNode.CreateObjectNode(null, level, parentId: node.ID);
                                var subDict2 = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonElement.GetRawText());
                                PropagateNodeTree(subDict2, nodeEl);
                            }
                            else
                            {
                                object val = GetSimpleObjectValue(jsonElement);
                                nodeEl = TreeNode.CreateFinalNode(null, val, jsonElement.ValueKind, level + 1, parentId: node.ID);
                            }
                            node.Children.Add(nodeEl);
                        }

                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.Undefined:
                    default:
                        break;
                }
            }
        }
        private static object GetSimpleObjectValue(JsonElement jsonElement)
        {
            object val = null;
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.String:
                    val = jsonElement.GetString();
                    break;
                case JsonValueKind.Number:
                    val = jsonElement.GetDouble();
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    val = jsonElement.GetBoolean();
                    break;
                default:
                    break;
            }
            return val;
        }
        private static void AddDescendants(IReadOnlyCollection<TreeNode> nodes, TreeNode parent)
        {
            var children = parent.AddChildren(nodes.Where(n => n.ParentID == parent.ID).ToArray());

            foreach (var subnode in children)
            {
                AddDescendants(nodes, subnode);
            }
        }
    }
}