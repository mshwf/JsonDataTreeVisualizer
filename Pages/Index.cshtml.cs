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
            var jsonObject = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(JsonSamples.ComplexMultiLevelJson);
            HeadNode = CreateJsonTree(jsonObject, null);
            FlattenNodes(HeadNode, FlattenedNodes);
        }

        private static TreeNode CreateJsonTree(Dictionary<string, JsonElement> dic, string fallbackObjectName)
        {
            TreeNode parentNode = TreeNode.CreateObjectNode(key: fallbackObjectName, level: 0,  id: Guid.NewGuid(), parentId: null);
            PropagateNodeTree(dic, parentNode);
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
                        node = TreeNode.CreateObjectNode(item.Key, level, id: Guid.NewGuid(), parentId: parentNode.ID);
                        node.ParentNode = parentNode;
                        var subDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(item.Value.GetRawText());
                        PropagateNodeTree(subDict, node);
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.String:
                        node = TreeNode.CreateFinalNode(item.Key, item.Value.GetString(), item.Value.ValueKind, level, parentId: parentNode.ID);
                        node.ParentNode = parentNode.ParentNode;
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.Number:
                        node = TreeNode.CreateFinalNode(item.Key, item.Value.GetDouble(), item.Value.ValueKind, level, parentId: parentNode.ID);
                        node.ParentNode = parentNode.ParentNode;
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.True:
                        node = TreeNode.CreateFinalNode(item.Key, true, item.Value.ValueKind, level, parentId: parentNode.ID);
                        node.ParentNode = parentNode.ParentNode;
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.False:
                        node = TreeNode.CreateFinalNode(item.Key, false, item.Value.ValueKind, level, parentId: parentNode.ID);
                        node.ParentNode = parentNode.ParentNode;
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.Null:
                        node = TreeNode.CreateFinalNode(item.Key, null, item.Value.ValueKind, level, parentId: parentNode.ID);
                        node.ParentNode = parentNode.ParentNode;
                        parentNode.Children.Add(node);
                        break;
                    case JsonValueKind.Undefined:
                    case JsonValueKind.Array:
                        break;
                    default:
                        break;
                }
            }
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

        public void OnPost()
        {
            string json = ToJson();
        }
        private static void AddDescendants(IReadOnlyCollection<TreeNode> nodes, TreeNode node)
        {
            var children = node.AddChildren(nodes.Where(i => i.ParentID == node.ID).ToArray());

            foreach (var subnode in children)
            {
                AddDescendants(nodes, subnode);
            }
        }
        private string ToJson()
        {
            var root = FlattenedNodes.First(x => x.ParentID == null);
            AddDescendants(FlattenedNodes.Where(n => n.Key != null).ToList(), root);
            Dictionary<string, object> dic = root.ToDictionary();
            var json = JsonSerializer.Serialize(dic);
            return json;
        }
    }
}