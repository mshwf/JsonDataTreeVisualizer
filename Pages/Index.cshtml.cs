using JsonDataTreeVisualizer.TreeModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace JsonDataTreeVisualizer.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel()
        {
        }
        public TreeNode HeadNode { get; set; }

        [BindProperty]
        public List<TreeNode> FlattenedNodes { get; set; } = new List<TreeNode>();

        public void OnGet()
        {
            UserJson = JsonSamples.ComplexMultiLevelJson;
            FromJsonToTree(UserJson);
        }

        private void FromJsonToTree(string json)
        {
            JsonElement element;
            try
            {
                element = JsonSerializer.Deserialize<JsonElement>(json);
            }
            catch (Exception)
            {
                return;
            }
            HeadNode = CreateJsonTree(element, null, null);
            HeadNode.OrderSubNodes();
            FlattenNodes(HeadNode, FlattenedNodes);
        }

        private static TreeNode CreateJsonTree(JsonElement jsonElement, string fallbackObjectName, Guid? parentId)
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
                                nodeEl = TreeNode.CreateFinalNode(null, val, jsonElement.ValueKind, level, parentId: node.ID);
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

        private void FlattenNodes(TreeNode node, List<TreeNode> flattened)
        {
            if (node.Children.Count == 0)
            {
                flattened.Add(node);
            }
            else
            {
                flattened.Add(node);
                foreach (var subNode in node.Children)
                {
                    FlattenNodes(subNode, flattened);
                }
            }
        }

        public void OnPostFromForm()
        {
            string json = ToJson(FlattenedNodes);
            UserJson = json;
        }
        private static void AddDescendants(IReadOnlyCollection<TreeNode> nodes, TreeNode parent)
        {
            var children = parent.AddChildren(nodes.Where(n => n.ParentID == parent.ID).ToArray());

            foreach (var subnode in children)
            {
                AddDescendants(nodes, subnode);
            }
        }
        private static string ToJson(List<TreeNode> flattenedNodes)
        {
            var root = flattenedNodes.First(x => x.ParentID == null);
            AddDescendants(flattenedNodes.Where(n => n.ParentID != null).ToList(), root);
            object jsonObj = root.ToSerializableObject();
            var json = JsonSerializer.Serialize(jsonObj);
            return json;
        }

        [BindProperty]
        public string UserJson { get; set; }
        public void OnPostFromJson()
        {
            FromJsonToTree(UserJson);
        }
    }
}