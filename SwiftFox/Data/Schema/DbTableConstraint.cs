namespace Swiftfox.Data.Schema
{
    public class DbTableConstraint
    {
        public string ConstraintName { get; set; }
        public DbConstraintType ConstraintType { get; set; }
        public List<string> ColumnNames { get; } = new List<string>();
    }
}
