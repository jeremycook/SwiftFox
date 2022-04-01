using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json;

namespace SwiftFox.Mvc
{
    public class JsonQueryParameterBinder : IModelBinder
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerDefaults.Web);

        private readonly ILogger<JsonQueryParameterBinder> logger;
        private readonly IObjectModelValidator validator;

        public JsonQueryParameterBinder(ILogger<JsonQueryParameterBinder> logger, IObjectModelValidator validator)
        {
            this.logger = logger;
            this.validator = validator;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!bindingContext.HttpContext.Request.Query.TryGetValue(bindingContext.FieldName, out var value))
            {
                return Task.CompletedTask;
            }

            try
            {
                object? parsed = JsonSerializer.Deserialize(value.First(), bindingContext.ModelType, jsonSerializerOptions);
                bindingContext.Result = ModelBindingResult.Success(parsed);

                if (parsed != null)
                {
                    validator.Validate(
                        bindingContext.ActionContext,
                        validationState: bindingContext.ValidationState,
                        prefix: string.Empty,
                        model: parsed
                    );
                }
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Failed to bind parameter '{FieldName}'", bindingContext.FieldName);
                bindingContext.ActionContext.ModelState.TryAddModelError(bindingContext.FieldName, ex.Message);
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                logger.LogError(ex, "Failed to bind parameter '{FieldName}'", bindingContext.FieldName);
                bindingContext.ActionContext.ModelState.TryAddModelError(bindingContext.FieldName, ex, bindingContext.ModelMetadata);
            }

            return Task.CompletedTask;
        }
    }
}