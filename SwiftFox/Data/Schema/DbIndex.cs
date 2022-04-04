namespace Swiftfox.Data.Schema
{
    public class DbIndex
    {
        public string IndexName { get; set; }
        public DbIndexType IndexType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsUnique { get; set; }
        public bool IsUniqueConstraint { get; set; }

        public List<DbIndexColumn> Columns { get; } = new List<DbIndexColumn>();
    }
}
