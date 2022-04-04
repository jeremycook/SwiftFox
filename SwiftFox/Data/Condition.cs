namespace Swiftfox.Data
{
    public class Condition
    {
        public ConditionOperator Operator { get; set; } = ConditionOperator.EqualTo;
        public string ColumnName { get; set; } = null!;
        public List<string> Values { get; set; } = new();
    }
}