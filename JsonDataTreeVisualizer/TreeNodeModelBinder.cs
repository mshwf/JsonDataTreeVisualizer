using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JsonDataTreeVisualizer
{
    public class TreeNodeModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            object model = null;
            var request = bindingContext.HttpContext.Request;
            if (request.HasFormContentType)
            {
                var form = await request.ReadFormAsync();
                model = form;
            }

            return;
        }
    }
}
