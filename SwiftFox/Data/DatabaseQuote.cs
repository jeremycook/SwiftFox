namespace SwiftFox.Data.Schema
{
    /// <summary>
    /// A singleton service that uses <see cref="DatabaseSchema"/> for reference to
    /// quote components of SQL like column, table and schema names.
    /// </summary>
    [Service(ServiceLifetime.Singleton)]
    public class DatabaseQuote
    {
        private readonly Dictionary<string, string> cache = new(StringComparer.InvariantCultureIgnoreCase);
        private readonly DatabaseSchema schema;

        public DatabaseQuote(DatabaseSchema schema)
        {
            this.schema = schema;
        }

        /// <summary>
        /// Quote one or more column names.
        /// </summary>
        /// <param name="tableSchema"></param>
        /// <param name="tableName"></param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public string Columns(string tableSchema, string tableName, IEnumerable<string> columnNames)
        {
            if (columnNames is null)
            {
                throw new ArgumentNullException(nameof(columnNames));
            }
            if (!columnNames.Any())
            {
                throw new ArgumentException($"The {nameof(columnNames)} argument cannot be empty.", nameof(columnNames));
            }

            List<string> names = new();

            foreach (var columnName in columnNames)
            {
                string sql = Column(tableSchema, tableName, columnName);
                names.Add(sql);
            }

            return string.Join(",", names);
        }

        /// <summary>
        /// Quote a column name.
        /// </summary>
        /// <param name="tableSchema"></param>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string Column(string tableSchema, string tableName, string columnName)
        {
            string key = $"{tableSchema}/{tableName}/{columnName}";

            if (!cache.TryGetValue(key, out string? sql))
            {
                DbTable table = schema.GetTable(tableSchema, tableName);
                DbColumn column = table.GetColumn(columnName);

                sql = $"[{column.ColumnName}]";
                cache[key] = sql;
            }

            return sql;
        }

        /// <summary>
        /// Quote an order by clause.
        /// </summary>
        /// <param name="tableSchema"></param>
        /// <param name="tableName"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public string OrderBy(string tableSchema, string tableName, IEnumerable<KeyValuePair<string, SortDirection>> orderBy)
        {
            if (orderBy is null)
            {
                throw new ArgumentNullException(nameof(orderBy));
            }
            if (!orderBy.Any())
            {
                throw new ArgumentException($"The {nameof(orderBy)} argument cannot be empty.", nameof(orderBy));
            }

            List<string> names = new();

            foreach (var item in orderBy)
            {
                string key = $"{tableSchema}/{tableName}/{item}";

                if (!cache.TryGetValue(key, out string? sql))
                {
                    DbTable table = schema.GetTable(tableSchema, tableName);
                    DbColumn column = table.GetColumn(item.Key);

                    sql = $"[{column.ColumnName}]{(item.Value == SortDirection.Desc ? " DESC" : "")}";
                    cache[key] = sql;
                }

                names.Add(sql);
            }

            return string.Join(",", names);
        }

        /// <summary>
        /// Quote a schema qualified table name.
        /// </summary>
        /// <param name="tableSchema"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string Table(string tableSchema, string tableName)
        {
            string key = $"{tableSchema}/{tableName}";

            if (!cache.TryGetValue(key, out string? sql))
            {
                DbTable table = schema.GetTable(tableSchema, tableName);

                sql = $"[{table.SchemaName}].[{table.TableName}]";
                cache[key] = sql;
            }

            return sql;
        }
    }
}
