namespace SwiftFox.Data.Schema
{
    public class DbRelationship
    {
        public string ConstraintCatalogName { get; set; }
        public string ConstraintSchemaName { get; set; }
        public string ConstraintName { get; set; }

        public string ForeignCatalogName { get; set; }
        public string ForeignSchemaName { get; set; }
        public string ForeignTableName { get; set; }

        public string PrimaryCatalogName { get; set; }
        public string PrimarySchemaName { get; set; }
        public string PrimaryTableName { get; set; }

        public DbRelationshipRule DeleteRule { get; set; }
        public DbRelationshipRule UpdateRule { get; set; }

        public List<DbRelationshipColumn> Columns { get; } = new List<DbRelationshipColumn>();
    }
}
