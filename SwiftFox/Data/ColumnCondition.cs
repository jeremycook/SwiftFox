namespace Swiftfox.Data
{
    public class ColumnCondition
    {
        public string ColumnName { get; set; } = null!;
        public Operator Operator { get; set; } = Operator.EqualTo;
        public List<string> Values { get; set; } = new();
    }
}