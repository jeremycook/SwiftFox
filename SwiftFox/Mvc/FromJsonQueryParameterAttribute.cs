using Swiftfox.Mvc;

namespace Microsoft.AspNetCore.Mvc
{
    public sealed class FromJsonQueryParameterAttribute : ModelBinderAttribute
    {
        public FromJsonQueryParameterAttribute()
        {
            BinderType = typeof(JsonQueryParameterBinder);
        }
    }
}
