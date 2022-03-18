namespace SwiftFox.UI
{
    [Service(typeof(IValueMutator<ImportMap>), ServiceLifetime.Singleton)]
    public class DefaultImportMapMutator : IValueMutator<ImportMap>
    {
        public ValueTask MutateAsync(ImportMap value)
        {
            value.Imports["bootstrap"] = "https://cdn.skypack.dev/bootstrap";
            value.Imports["jquery-validation-unobtrusive"] = "https://cdn.skypack.dev/jquery-validation-unobtrusive";
            value.Imports["sinuous"] = "https://cdn.skypack.dev/sinuous";

            return ValueTask.CompletedTask;
        }
    }
}
