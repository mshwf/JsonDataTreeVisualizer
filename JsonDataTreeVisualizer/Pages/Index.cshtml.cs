using JsonTreeParser.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using JsonTreeParser.Extensions;
using System;

namespace JsonDataTreeVisualizer.Pages
{
    [RequestFormLimits(ValueCountLimit = int.MaxValue)]
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        readonly bool orderNodesByChildrenCount;
        public IndexModel(IConfiguration configuration)
        {
            orderNodesByChildrenCount = configuration.GetValue<bool>("OrderNodesByChildrenCount");
            JsonSamplesSelect = JsonSamplesStore.Samples.Select(x => new SelectListItem(x.Name, x.Id, x.Id == SelectedSampleId));
        }
        public IEnumerable<SelectListItem> JsonSamplesSelect { get; set; }
        [BindProperty]
        public List<JsonTreeNodeViewModel> FlattenedNodes { get; set; } = new List<JsonTreeNodeViewModel>();
        [BindProperty]
        public string UserJson { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SelectedSampleId { get; set; } = "0";
        public void OnGet()
        {
            try
            {
                UserJson = JsonSamplesStore.Samples.First(x => x.Id == SelectedSampleId).Value;
                FlattenedNodes = BuildFlattenedNodesFromJson(UserJson).Select(JsonTreeNodeViewModel.FromJsonTreeNode).ToList();
            }
            catch
            {
                return;
            }
        }

        public void OnPostBuildJsonTree()
        {
            try
            {
                if (string.IsNullOrEmpty(UserJson)) return;
                FlattenedNodes = BuildFlattenedNodesFromJson(UserJson).Select(JsonTreeNodeViewModel.FromJsonTreeNode).ToList();
            }
            catch (JsonException)
            {
                return;
            }
        }
        public void OnPostBuildJsonObject()
        {
            var root = FlattenedNodes.Select(JsonTreeNodeViewModel.ToJsonTreeNode).BuildRootNode();
            UserJson = root.Serialize();
        }

        private static List<JsonTreeNode> BuildFlattenedNodesFromJson(string userJson)
        {
            var root = JsonTreeNodeBuilder.BuildFromJson(userJson);
            return root.FlattenNodes();
        }
    }
}