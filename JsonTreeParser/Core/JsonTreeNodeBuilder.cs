using JsonTreeParser.Extensions;
using System;
using System.Collections.Generic;
using System.Text.Json;
 
namespace JsonTreeParser.Core
{
    public class JsonTreeNodeBuilder
    {
        public static JsonTreeNode BuildFromJson(string json, bool orderByChildrenCount, string fallbackObjectName)
        {
            var element = JsonSerializer.Deserialize<JsonElement>(json);
            var headNode = CreateJsonTree(element, fallbackObjectName, null);
            if (orderByChildrenCount)
                headNode.OrderByChildren();
            return headNode;
        }
        
        private static JsonTreeNode CreateJsonTree(JsonElement jsonElement, string fallbackObjectName, Guid? parentId)
        {
            JsonTreeNode parentNode;
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.Array:
                    parentNode = JsonTreeNode.CreateArrayNode(key: fallbackObjectName, level: 0, parentId: parentId);
                    foreach (var childJsonElement in jsonElement.EnumerateArray())
                    {
                        var node = CreateJsonTree(childJsonElement, null, parentNode.ID);
                        parentNode.AddChild(node);
                    }
                    break;
                case JsonValueKind.Object:
                    parentNode = JsonTreeNode.CreateObjectNode(key: fallbackObjectName, level: 0, parentId: parentId);
                    var objectDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonElement.GetRawText());
                    PropagateObjectNodeTree(objectDict, parentNode);
                    break;
                default:
                    parentNode = JsonTreeNode.CreateFinalNode(key: fallbackObjectName, level: 0, parentId: parentId, GetSimpleObjectValue(jsonElement), jsonElement.ValueKind);
                    break;
            }
            return parentNode;
        }
        private static void PropagateObjectNodeTree(Dictionary<string, JsonElement> dic, JsonTreeNode parentNode)
        {
            int level = parentNode.Level + 1;
            foreach (var item in dic)
            {
                JsonTreeNode node;
                switch (item.Value.ValueKind)
                {
                    case JsonValueKind.Object:
                        node = JsonTreeNode.CreateObjectNode(item.Key, level, parentId: parentNode.ID);
                        var subDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(item.Value.GetRawText());
                        PropagateObjectNodeTree(subDict, node);
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.String:
                        node = JsonTreeNode.CreateFinalNode(item.Key, level, parentId: parentNode.ID, item.Value.GetString(), item.Value.ValueKind);
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.Number:
                        node = JsonTreeNode.CreateFinalNode(item.Key, level, parentId: parentNode.ID, item.Value.GetDouble(), item.Value.ValueKind);
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.True:
                        node = JsonTreeNode.CreateFinalNode(item.Key, level, parentId: parentNode.ID, true, item.Value.ValueKind);
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.False:
                        node = JsonTreeNode.CreateFinalNode(item.Key, level, parentId: parentNode.ID, false, item.Value.ValueKind);
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.Null:
                        node = JsonTreeNode.CreateFinalNode(item.Key, level, parentId: parentNode.ID, null, item.Value.ValueKind);
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.Array:
                        node = JsonTreeNode.CreateArrayNode(item.Key, level, parentId: parentNode.ID);

                        foreach (var jsonElement in item.Value.EnumerateArray())
                        {
                            JsonTreeNode nodeEl;
                            if (jsonElement.ValueKind == JsonValueKind.Object)
                            {
                                nodeEl = JsonTreeNode.CreateObjectNode(null, level, parentId: node.ID);
                                var subDict2 = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonElement.GetRawText());
                                PropagateObjectNodeTree(subDict2, nodeEl);
                            }
                            else
                            {
                                object val = GetSimpleObjectValue(jsonElement);
                                nodeEl = JsonTreeNode.CreateFinalNode(null, level + 1, parentId: node.ID, val, jsonElement.ValueKind);
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
    }
}