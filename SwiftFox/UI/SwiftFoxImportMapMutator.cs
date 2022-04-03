namespace SwiftFox.UI
{
    [Service(typeof(IValueMutator<ImportMap>), ServiceLifetime.Singleton)]
    public class SwiftFoxImportMapMutator : IValueMutator<ImportMap>
    {
        public ValueTask MutateAsync(ImportMap value)
        {
            // TODO: Use build time or something better
            var random = new Random();

            value.Imports["bootstrap"] = "https://cdn.skypack.dev/bootstrap";
            value.Imports["jquery-validation-unobtrusive"] = "https://cdn.skypack.dev/jquery-validation-unobtrusive";
            value.Imports["sinuous"] = "https://cdn.skypack.dev/sinuous";
            value.Imports["sinuous/map"] = "https://cdn.skypack.dev/sinuous/map";
            value.Imports["swiftfox/app"] = $"/_content/SwiftFox/app.js?v={random.Next()}";
            value.Imports["swiftfox/table"] = $"/_content/SwiftFox/table.js?v={random.Next()}";

            return ValueTask.CompletedTask;
        }
    }
}
