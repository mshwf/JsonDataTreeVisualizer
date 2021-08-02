using JsonDataTreeVisualizer.Pages;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JsonDataTreeVisualizer.ViewComponents
{
    public class TreeDrawerViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(SmartNode headNode)
        {
            return View(headNode);
        }

        async Task<IViewComponentResult> DrawNode(SmartNode node)
        {
            var margin = (node.Level * 20).ToString() + "px";
            if (!node.IsFinal)
            {
                foreach (var subnode in node.SubNodes)
                {
                    await DrawNode(subnode);
                }
            }
            else
            {
                int i = 0;
                var id = node.Key + "_" + node.Level;

            }
            return View(node);

        }

    }
}
