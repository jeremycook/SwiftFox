namespace Swiftfox.Data.Schema
{
    public class DbColumn
    {
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public string ColumnDefault { get; set; }
        public int ColumnPosition { get; set; }
        public bool IsNullable { get; set; }
        public int? MaxLength { get; set; }
        public Type ClrType { get; set; }
        public string ClrTypeName { get; set; }
        public string SingularName { get; set; }
        public string PluralName { get; set; }
    }
}
