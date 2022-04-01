using Microsoft.Extensions.Options;
using SwiftFox.Data.Schema;
using System.Data.SqlClient;

namespace SwiftFox.Data
{
    [Service]
    public class Database
    {
        private readonly IOptions<SwiftFoxOptions> options;
        private readonly SqlSchemaBuilder dbSchemaBuilder;

        public Database(IOptions<SwiftFoxOptions> options, SqlSchemaBuilder dbSchemaBuilder)
        {
            this.options = options;
            this.dbSchemaBuilder = dbSchemaBuilder;
        }

        public async Task<TableQueryResult> QueryAsync(TableQuery query)
        {
            var result = new TableQueryResult();

            // TODO: Use schema info to validate columns, tables and schemas
            // TODO: Where filter

            string sql =
$@"SELECT {(query.Columns.Any() ? string.Join(",", query.Columns.Select(name => "[" + name + "]")) : "*")}
FROM [{query.TableSchema}].[{query.TableName}]
WHERE 1=1
ORDER BY {(query.OrderBy.Any() ?
    string.Join(",", query.OrderBy.Select(x => "[" + x.Key + "] " + x.Value.ToString().ToUpper())) :
    "0")}
OFFSET {query.Skip} ROWS
FETCH NEXT {query.Take} ROWS ONLY;";

            if (query.Verbose)
            {
                result.Sql = sql;
            }

            using var connection = new SqlConnection(options.Value.MainConnectionString);
            using var cmd = new SqlCommand(sql, connection);
            await connection.OpenAsync();

            var reader = await cmd.ExecuteReaderAsync();

            var dbColumns = await reader.GetColumnSchemaAsync();
            var columns = dbColumns.Count;
            result.ColumnNames.AddRange(dbColumns.Select(o => o.ColumnName));

            while (await reader.ReadAsync())
            {
                var record = new object[columns];

                for (int i = 0; i < columns; i++)
                {
                    record[i] = reader[i];
                    if (record[i] == DBNull.Value)
                    {
                        record[i] = null!;
                    }
                }

                result.Records.Add(record);
            }

            return result;
        }
    }
}
