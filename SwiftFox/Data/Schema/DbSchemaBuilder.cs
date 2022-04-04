using Dapper;
using Humanizer;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace Swiftfox.Data.Schema
{
    [Service(ServiceLifetime.Singleton)]
    public class DbSchemaBuilder
    {
        public DbSchemaBuilder() : this(DbSchema.Defaults.BuiltInTypes, DbSchema.Defaults.ClrTypes) { }
        public DbSchemaBuilder(Dictionary<Type, string> builtInTypes, Dictionary<string, Type> clrTypes)
        {
            BuiltInTypes = builtInTypes;
            ClrTypes = clrTypes;
        }

        public Dictionary<Type, string> BuiltInTypes { get; }
        public Dictionary<string, Type> ClrTypes { get; }

        public async Task BuildAsync(DbSchema schema, DbConnection connection)
        {
            schema.Tables.AddRange(await connection.QueryAsync<DbTable>(
$@"SELECT
    CatalogName = TABLE_CATALOG,
    SchemaName = TABLE_SCHEMA,
    TableName = TABLE_NAME,
    TableType = CASE TABLE_TYPE
		WHEN 'VIEW' THEN {(int)DbTableType.View}
		WHEN 'BASE TABLE' THEN {(int)DbTableType.BaseTable}
		ELSE {(int)DbTableType.Unknown}
	END
from INFORMATION_SCHEMA.TABLES
"));

            foreach (var table in schema.Tables)
            {
                table.SingularName = table.TableName.Titleize();
                table.PluralName = table.SingularName.Pluralize();

                table.Columns.AddRange(await connection.QueryAsync<DbColumn>(
@"SELECT
    ColumnName = COLUMN_NAME,
	ColumnType = DATA_TYPE,
	ColumnDefault = COLUMN_DEFAULT,
	ColumnPosition = ORDINAL_POSITION,
	IsNullable = CASE WHEN IS_NULLABLE = 'YES' THEN 1 ELSE 0 END,
	MaxLength = CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE 
    @CatalogName = TABLE_CATALOG AND
    @SchemaName = TABLE_SCHEMA AND
    @TableName = TABLE_NAME
", table));
                foreach (var column in table.Columns)
                {
                    column.ClrType = GetClrType(column);
                    column.ClrTypeName = GetClrTypeName(column);
                    column.SingularName = Regex.Replace(column.ColumnName.Titleize(), @" Id$", "");
                    column.PluralName = column.SingularName.Pluralize();
                }

                var indexes = await connection.QueryAsync<Internals.DbIndex>(
@"SELECT
    IndexName = i.name,
    IndexType = i.type,
    IndexTypeDescription = i.type_desc,
    IsPrimaryKey = i.is_primary_key,
    IsUnique = i.is_unique,
    IsUniqueConstraint = i.is_unique_constraint,
    ColumnName = COL_NAME(ic.object_id,ic.column_id),
    Position = ic.key_ordinal
FROM sys.indexes i
JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
WHERE i.is_hypothetical = 0 AND
	OBJECT_SCHEMA_NAME(i.object_id) = @SchemaName AND
	OBJECT_NAME(i.object_id) = @TableName
", table);
                foreach (var group in indexes.GroupBy(r => new
                {
                    r.IndexName,
                    r.IndexType,
                    r.IsPrimaryKey,
                    r.IsUnique,
                    r.IsUniqueConstraint
                }))
                {
                    var index = new DbIndex
                    {
                        IndexName = group.Key.IndexName,
                        IndexType = group.Key.IndexType,
                        IsPrimaryKey = group.Key.IsPrimaryKey,
                        IsUnique = group.Key.IsUnique,
                        IsUniqueConstraint = group.Key.IsUniqueConstraint
                    };
                    index.Columns.AddRange(group.Select(r => new DbIndexColumn
                    {
                        ColumnName = r.ColumnName,
                        Position = r.Position,
                    }));
                    table.Indexes.Add(index);
                }

                var tableConstraints = await connection.QueryAsync(
@"SELECT
    ConstraintName = ccu.CONSTRAINT_NAME, 
    ConstraintType = tc.CONSTRAINT_TYPE,
    ColumnNames = STRING_AGG(c.COLUMN_NAME, ',')
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
    join INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu on 
        ccu.CONSTRAINT_CATALOG = tc.CONSTRAINT_CATALOG and
        ccu.CONSTRAINT_SCHEMA = tc.CONSTRAINT_SCHEMA and
        ccu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
        join INFORMATION_SCHEMA.COLUMNS c on 
            c.TABLE_CATALOG = ccu.CONSTRAINT_CATALOG and
            c.TABLE_SCHEMA = ccu.CONSTRAINT_SCHEMA and
            c.TABLE_NAME = ccu.TABLE_NAME and
            c.COLUMN_NAME = ccu.COLUMN_NAME
WHERE 
    @CatalogName = c.TABLE_CATALOG AND
    @SchemaName = c.TABLE_SCHEMA AND
    @TableName = c.TABLE_NAME
GROUP BY c.TABLE_CATALOG, c.TABLE_SCHEMA, c.TABLE_NAME, ccu.CONSTRAINT_NAME, tc.CONSTRAINT_TYPE
", table);
                foreach (var tableConstraint in tableConstraints)
                {
                    var dbConstraint = new DbTableConstraint
                    {
                        ConstraintName = tableConstraint.ConstraintName,
                        ConstraintType = (DbConstraintType)Enum.Parse(typeof(DbConstraintType), tableConstraint.ConstraintType.Replace(" ", ""), ignoreCase: true),
                    };
                    dbConstraint.ColumnNames.AddRange(tableConstraint.ColumnNames.Split(','));
                    table.Constraints.Add(dbConstraint);
                }
            }

            var relationships = await connection.QueryAsync<Internals.TableColumnRelationship>(
@"SELECT 
    ConstraintCatalogName = tc.CONSTRAINT_CATALOG,
    ConstraintSchemaName = tc.CONSTRAINT_SCHEMA,
    ConstraintName = tc.CONSTRAINT_NAME,
    ForeignCatalogName = tc.TABLE_CATALOG,
    ForeignSchemaName = tc.TABLE_SCHEMA,
    ForeignTableName = tc.TABLE_NAME,
    ForeignColumnName = kcu.COLUMN_NAME,
    PrimaryCatalogName = tcPrimary.TABLE_CATALOG,
    PrimarySchemaName = tcPrimary.TABLE_SCHEMA,
    PrimaryTableName = tcPrimary.TABLE_NAME,
    PrimaryColumnName = kcuPrimary.COLUMN_NAME,
    DeleteRule = rc.DELETE_RULE,
    UpdateRule = rc.UPDATE_RULE
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
    JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu 
        ON tc.CONSTRAINT_CATALOG = kcu.CONSTRAINT_CATALOG
        AND tc.CONSTRAINT_SCHEMA = kcu.CONSTRAINT_SCHEMA 
        AND tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME 
    JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc 
        ON tc.CONSTRAINT_CATALOG = rc.CONSTRAINT_CATALOG
        AND tc.CONSTRAINT_SCHEMA = rc.CONSTRAINT_SCHEMA 
        AND tc.CONSTRAINT_NAME = rc.CONSTRAINT_NAME 
    JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tcPrimary 
        ON rc.CONSTRAINT_CATALOG = tcPrimary.CONSTRAINT_CATALOG
        AND rc.UNIQUE_CONSTRAINT_SCHEMA = tcPrimary.CONSTRAINT_SCHEMA 
        AND rc.UNIQUE_CONSTRAINT_NAME = tcPrimary.CONSTRAINT_NAME 
    JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcuPrimary 
        ON tcPrimary.CONSTRAINT_CATALOG = kcuPrimary.CONSTRAINT_CATALOG
        AND tcPrimary.CONSTRAINT_SCHEMA = kcuPrimary.CONSTRAINT_SCHEMA 
        AND tcPrimary.CONSTRAINT_NAME = kcuPrimary.CONSTRAINT_NAME 
        AND kcu.ORDINAL_POSITION = kcuPrimary.ORDINAL_POSITION
");
            foreach (var rel in relationships.GroupBy(r => new DbRelationship
            {
                ConstraintCatalogName = r.ConstraintCatalogName,
                ConstraintSchemaName = r.ConstraintSchemaName,
                ConstraintName = r.ConstraintName,

                ForeignCatalogName = r.ForeignCatalogName,
                ForeignSchemaName = r.ForeignSchemaName,
                ForeignTableName = r.ForeignTableName,

                PrimaryCatalogName = r.PrimaryCatalogName,
                PrimarySchemaName = r.PrimarySchemaName,
                PrimaryTableName = r.PrimaryTableName,

                DeleteRule = (DbRelationshipRule)Enum.Parse(typeof(DbRelationshipRule), r.DeleteRule.Replace(" ", ""), ignoreCase: true),
                UpdateRule = (DbRelationshipRule)Enum.Parse(typeof(DbRelationshipRule), r.UpdateRule.Replace(" ", ""), ignoreCase: true),
            }))
            {
                rel.Key.Columns.AddRange(rel.Select(r => new DbRelationshipColumn
                {
                    PrimaryColumnName = r.PrimaryColumnName,
                    ForeignColumnName = r.ForeignColumnName
                }));
                schema.Relationships.Add(rel.Key);
            }
        }

        public Type GetClrType(DbColumn column)
        {
            if (!ClrTypes.TryGetValue(column.ColumnType, out Type type))
            {
                throw new NotImplementedException(column.ColumnType);
            }

            return column.IsNullable && type.IsValueType ?
                typeof(Nullable<>).MakeGenericType(type) :
                type;
        }

        public string GetClrTypeName(DbColumn column)
        {
            var nullable = column.ClrType.IsGenericType && column.ClrType.GetGenericTypeDefinition() == typeof(Nullable<>);
            Type type = nullable ?
                column.ClrType.GetGenericArguments()[0] :
                column.ClrType;

            if (!BuiltInTypes.TryGetValue(type, out string? columnType))
            {
                columnType = type.Name;
            }

            return columnType + (nullable ? "?" : "");
        }

        internal static class Internals
        {
            internal class TableColumnRelationship
            {
                public string ConstraintCatalogName { get; set; }
                public string ConstraintSchemaName { get; set; }
                public string ConstraintName { get; set; }
                public string ForeignCatalogName { get; set; }
                public string ForeignSchemaName { get; set; }
                public string ForeignTableName { get; set; }
                public string ForeignColumnName { get; set; }
                public string PrimaryCatalogName { get; set; }
                public string PrimarySchemaName { get; set; }
                public string PrimaryTableName { get; set; }
                public string PrimaryColumnName { get; set; }
                public string DeleteRule { get; set; }
                public string UpdateRule { get; set; }
            }

            public class DbIndex
            {
                public string IndexName { get; set; }
                public DbIndexType IndexType { get; set; }
                public bool IsPrimaryKey { get; set; }
                public bool IsUnique { get; set; }
                public bool IsUniqueConstraint { get; set; }

                public string ColumnName { get; set; }
                public int Position { get; set; }
            }
        }
    }
}
