using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net.Mime;

namespace SwiftFox.Mvc
{
    public static class JsonQueryParameterSwaggerGenExtensions
    {
        public static SwaggerGenOptions AddJsonQueryParameterSupport(this SwaggerGenOptions options)
        {
            options.OperationFilter<JsonQueryParameterOperationFilter>();
            return options;
        }

        private sealed class JsonQueryParameterOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var jsonQueryParams = context.ApiDescription.ActionDescriptor.Parameters
                    .Where(ad => ad.BindingInfo.BinderType == typeof(JsonQueryParameterBinder))
                    .Select(ad => ad.Name);

                if (!jsonQueryParams.Any())
                {
                    return;
                }

                foreach (var p in operation.Parameters.Where(p => jsonQueryParams.Contains(p.Name)))
                {
                    // move the schema under application/json content type
                    p.Content = new Dictionary<string, OpenApiMediaType>()
                    {
                        [MediaTypeNames.Application.Json] = new OpenApiMediaType()
                        {
                            Schema = p.Schema
                        }
                    };
                    // then clear it
                    p.Schema = null;
                }
            }
        }
    }
}
