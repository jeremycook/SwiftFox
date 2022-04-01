using Microsoft.Extensions.Options;
using SwiftFox.Data.Schema;
using System.Data.SqlClient;

namespace SwiftFox.Data
{
    [Service]
    public class Database
    {
        private readonly IOptions<SwiftFoxOptions> options;
        private readonly DatabaseQuote quote;

        public Database(IOptions<SwiftFoxOptions> options, DatabaseQuote quote)
        {
            this.options = options;
            this.quote = quote;
        }

        public async Task<TableQueryResult> QueryAsync(TableQuery query)
        {
            var result = new TableQueryResult();

            // TODO: Where filter

            string sql =
$@"SELECT {(query.Columns.Any() ? quote.Columns(query.TableSchema, query.TableName, query.Columns) : "*")}
FROM {quote.Table(query.TableSchema, query.TableName)}
WHERE 1=1
ORDER BY {(query.OrderBy.Any() ?
    quote.OrderBy(query.TableSchema, query.TableName, query.OrderBy) :
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
            using var reader = await cmd.ExecuteReaderAsync();

            var dbColumns = await reader.GetColumnSchemaAsync();
            var numberOfColumns = dbColumns.Count;
            result.ColumnNames.AddRange(dbColumns.Select(o => o.ColumnName));

            while (await reader.ReadAsync())
            {
                var record = new object[numberOfColumns];

                for (int i = 0; i < numberOfColumns; i++)
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
