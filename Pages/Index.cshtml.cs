using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace JsonDataTreeVisualizer.Pages
{
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
        public List<TreeNode> FlattenedNodes { get; set; } = new List<TreeNode>();
        [BindProperty]
        public string UserJson { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SelectedSampleId { get; set; } = "0";
        public void OnGet()
        {
            try
            {
                UserJson = JsonSamplesStore.Samples.First(x => x.Id == SelectedSampleId).Value;
                FlattenedNodes = TreeNodeHelper.FromJsonToFlattenedTree(UserJson, orderNodesByChildrenCount);
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
                FlattenedNodes = TreeNodeHelper.FromJsonToFlattenedTree(UserJson, orderNodesByChildrenCount);
            }
            catch (JsonException)
            {
                return;
            }
        }
        public void OnPostBuildJsonObject()
        {
            string json = TreeNodeHelper.FromFlattenedTreeToJson(FlattenedNodes);
            UserJson = json;
        }
    }
}