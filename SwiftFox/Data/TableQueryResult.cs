namespace Swiftfox.Data
{
    public class TableQueryResult
    {
        public List<string> ColumnNames { get; set; } = new();
        public List<object[]> Records { get; set; } = new();
        public string? Sql { get; set; }
    }
}
