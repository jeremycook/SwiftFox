namespace SwiftFox.Configuration
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class OptionsAttribute : Attribute
    {
        public string? SectionName { get; set; }
    }
}
