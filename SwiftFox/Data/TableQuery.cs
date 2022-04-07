using System.ComponentModel;

namespace Swiftfox.Data
{
    public class TableQuery
    {
        public string TableSchema { get; set; } = default!;
        public string TableName { get; set; } = default!;
        public List<string> Columns { get; set; } = new();
        public List<ColumnCondition> ColumnConditions { get; set; } = new();
        public List<OrderByPart> OrderBy { get; set; } = new();
        public int Skip { get; set; }
        [DefaultValue(100)]
        public int Take { get; set; } = 100;
        public bool Verbose { get; set; }
    }
}
