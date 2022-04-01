namespace SwiftFox.Data
{
    public static class SwiftConvert
    {
        public static object ChangeType(string value, Type newType)
        {
            return newType.Name switch
            {
                nameof(DateOnly) => DateOnly.Parse(value),
                nameof(TimeOnly) => TimeOnly.Parse(value),
                nameof(DateTime) => DateTime.Parse(value),
                nameof(DateTimeOffset) => DateTimeOffset.Parse(value),
                nameof(Guid) => Guid.Parse(value),
                _ => Convert.ChangeType(value, newType),
            };
        }
    }
}
