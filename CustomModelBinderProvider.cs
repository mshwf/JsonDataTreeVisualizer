using JsonDataTreeVisualizer.Pages;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace JsonDataTreeVisualizer
{
    internal class CustomModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var modelType = context.Metadata.UnderlyingOrModelType;
            if (modelType == typeof(SmartNode))
            {
                return new CustomModelBinder();
            }

            return null;
        }
    }
}