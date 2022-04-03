using Microsoft.Extensions.Options;
using SwiftFox.Data.Schema;
using System.Data.SqlClient;

namespace SwiftFox.Data
{
    [Service]
    public class Database
    {
        private static readonly Dictionary<ConditionOperator, string> operatorSymbols = new()
        {
            [ConditionOperator.GreaterThan] = ">",
            [ConditionOperator.GreaterThanOrEqualTo] = ">=",
            [ConditionOperator.LessThan] = "<",
            [ConditionOperator.LessThanOrEqualTo] = "<=",
        };
        private readonly IOptions<SwiftFoxOptions> options;
        private readonly DatabaseQuote quote;

        public Database(IOptions<SwiftFoxOptions> options, DatabaseQuote quote, DatabaseSchema schema)
        {
            this.options = options;
            this.quote = quote;
            Schema = schema;
        }

        public DatabaseSchema Schema { get; }

        public async Task<TableQueryResult> QueryAsync(TableQuery query)
        {
            // Get the table to confirm it is valid before doing anything else.
            var table = Schema.GetTable(query.TableSchema, query.TableName);

            var result = new TableQueryResult();

            using var connection = new SqlConnection(options.Value.MainConnectionString);
            using var cmd = connection.CreateCommand();

            List<string> where = WhereClause(query, table, cmd.Parameters);

            cmd.CommandText =
$@"SELECT {(query.Columns.Any() ? quote.Columns(query.TableSchema, query.TableName, query.Columns) : "*")}
FROM {quote.Table(query.TableSchema, query.TableName)}
WHERE {(where.Any() ?
    string.Join(" AND ", where) :
    "1=1")}
ORDER BY {(
    query.OrderBy.Any() ? quote.OrderBy(query.TableSchema, query.TableName, query.OrderBy) :
    table.Columns.FirstOrDefault(o => o.ClrType == typeof(string)) is DbColumn column ? quote.Column(table.SchemaName, table.TableName, column.ColumnName) :
    "1")}
OFFSET {query.Skip} ROWS
FETCH NEXT {query.Take} ROWS ONLY;";

            if (query.Verbose)
            {
                result.Sql = cmd.CommandText;
            }

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

        private List<string> WhereClause(TableQuery query, DbTable table, SqlParameterCollection parameters)
        {
            var where = new List<string>();
            foreach (var condition in query.Conditions)
            {
                var column = table.GetColumn(condition.ColumnName);
                string columnSql = quote.Column(query.TableSchema, query.TableName, condition.ColumnName);

                switch (condition.Operator)
                {
                    case ConditionOperator.Between:
                        {
                            object leftValue = SwiftConvert.ChangeType(condition.Values.First(), column.ClrType);
                            object rightValue = SwiftConvert.ChangeType(condition.Values.Last(), column.ClrType);

                            var leftParameter = parameters.AddWithValue($"@p{parameters.Count}", leftValue);
                            var rightParameter = parameters.AddWithValue($"@p{parameters.Count}", rightValue);

                            where.Add($"{columnSql} BETWEEN {leftParameter.ParameterName} AND {rightParameter.ParameterName}");
                            break;
                        }
                    case ConditionOperator.Contains:
                    case ConditionOperator.NotContains:
                        {
                            bool notLike = condition.Operator == ConditionOperator.NotContains;

                            var parts = new List<string>(condition.Values.Count);
                            foreach (string value in condition.Values)
                            {
                                // Skip converting the string value since LIKE only works with strings.
                                var parameter = parameters.AddWithValue($"@p{parameters.Count}", $"%{value}%");
                                parts.Add($"{columnSql} {(notLike ? "NOT LIKE" : "LIKE")} {parameter.ParameterName}");
                            }

                            if (notLike)
                            {
                                where.Add($"({string.Join(" AND ", parts)})");
                            }
                            else
                            {
                                where.Add($"({string.Join(" OR ", parts)})");
                            }

                            break;
                        }
                    case ConditionOperator.EqualTo:
                    case ConditionOperator.NotEqualTo:
                        {
                            bool notEqual = condition.Operator == ConditionOperator.NotEqualTo;

                            var parts = new List<string>(condition.Values.Count);
                            foreach (string value in condition.Values)
                            {
                                object clrValue = SwiftConvert.ChangeType(value, column.ClrType);
                                var parameter = parameters.AddWithValue($"@p{parameters.Count}", clrValue);
                                parts.Add(parameter.ParameterName);
                            }
                            where.Add($"{columnSql} {(notEqual ? "NOT IN" : "IN")} ({string.Join(",", parts)})");

                            break;
                        }
                    case ConditionOperator.GreaterThan:
                    case ConditionOperator.GreaterThanOrEqualTo:
                    case ConditionOperator.LessThan:
                    case ConditionOperator.LessThanOrEqualTo:
                        {
                            var @operator = operatorSymbols[condition.Operator];
                            var parts = new List<string>(condition.Values.Count);
                            foreach (string value in condition.Values)
                            {
                                object clrValue = SwiftConvert.ChangeType(value, column.ClrType);
                                var parameter = parameters.AddWithValue($"@p{parameters.Count}", clrValue);
                                parts.Add($"{columnSql} {@operator} {parameter.ParameterName}");
                            }
                            where.Add($"({string.Join(" OR ", parts)})");
                            break;
                        }
                    default:
                        throw new NotSupportedException($"The '{condition.Operator}' condition operator is not supported.");
                }
            }

            return where;
        }
    }
}
