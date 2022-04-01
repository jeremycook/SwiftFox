namespace SwiftFox.Data.Schema
{
    public class DbTable
    {
        public string CatalogName { get; set; }
        // TODO: Rename to TableSchema
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public DbTableType TableType { get; set; }
        public string SingularName { get; set; }
        public string PluralName { get; set; }

        public List<DbColumn> Columns { get; } = new List<DbColumn>();
        public List<DbIndex> Indexes { get; } = new List<DbIndex>();
        public List<DbTableConstraint> Constraints { get; } = new List<DbTableConstraint>();

        public DbColumn GetColumn(string columnName)
        {
            return
                Columns.SingleOrDefault(c => c.ColumnName.Equals(columnName, StringComparison.InvariantCultureIgnoreCase)) ??
                throw new ArgumentException($"Column not found: {columnName}");
        }
    }
}
