namespace Swiftfox.Data
{
    public class OrderByPart
    {
        public string ColumnName { get; set; } = default!;
        public SortDirection SortDirection { get; set; }
    }
}