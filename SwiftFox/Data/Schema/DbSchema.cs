namespace SwiftFox.Data.Schema
{
    public class DbSchema
    {
        public static class Defaults
        {
            /// <summary>
            /// Maps built-in types to the corresponding C# keyword.
            /// See: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types
            /// </summary>
            public static readonly Dictionary<Type, string> BuiltInTypes = new()
            {
                [typeof(bool)] = "bool",
                [typeof(byte)] = "byte",
                [typeof(byte[])] = "byte[]",
                [typeof(sbyte)] = "sbyte",
                [typeof(char)] = "char",
                [typeof(decimal)] = "decimal",
                [typeof(double)] = "double",
                [typeof(float)] = "float",
                [typeof(int)] = "int",
                [typeof(uint)] = "uint",
                [typeof(long)] = "long",
                [typeof(ulong)] = "ulong",
                [typeof(short)] = "short",
                [typeof(ushort)] = "ushort",

                [typeof(object)] = "object",
                [typeof(string)] = "string",
            };

            /// <summary>
            /// Maps DbType, SqlDbType, OleDbType, OdbcType strings to a CLR type.
            /// See: https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/configuring-parameters-and-parameter-data-types#specifying-parameter-data-types
            /// </summary>
            public static readonly Dictionary<string, Type> ClrTypes = new(StringComparer.InvariantCultureIgnoreCase)
            {
                ["Boolean"] = typeof(bool),
                ["Bit"] = typeof(bool),

                ["Byte"] = typeof(byte),
                ["TinyInt"] = typeof(byte),
                ["UnsignedTinyInt"] = typeof(byte),

                ["Binary"] = typeof(byte[]),
                ["VarBinary"] = typeof(byte[]),

                ["Char"] = typeof(char),

                ["Date"] = typeof(DateTime),
                ["DBDate"] = typeof(DateTime),

                ["datetime"] = typeof(DateTime),
                ["DBTimeStamp"] = typeof(DateTime),

                ["DateTimeOffset"] = typeof(DateTimeOffset),

                ["Decimal"] = typeof(decimal),
                ["Numeric"] = typeof(decimal),

                ["Double"] = typeof(double),
                ["Float"] = typeof(double),

                ["Single"] = typeof(float),
                ["Real"] = typeof(float),

                ["Guid"] = typeof(Guid),
                ["UniqueIdentifier"] = typeof(Guid),

                ["Int16"] = typeof(short),
                ["SmallInt"] = typeof(short),

                ["Int"] = typeof(int),
                ["Int32"] = typeof(int),

                ["Int64"] = typeof(long),
                ["BigInt"] = typeof(long),

                ["Object"] = typeof(object),
                ["Variant"] = typeof(object),

                ["NChar"] = typeof(string),
                ["WChar"] = typeof(string),

                ["NVarChar"] = typeof(string),
                ["String"] = typeof(string),
                ["VarChar"] = typeof(string),
                ["VarWChar"] = typeof(string),

                ["DBTime"] = typeof(TimeSpan),
                ["Time"] = typeof(TimeSpan),

                ["UInt16"] = typeof(ushort),
                ["UnsignedSmallInt"] = typeof(ushort),

                ["UInt32"] = typeof(uint),
                ["UnsignedInt"] = typeof(uint),

                ["UInt64"] = typeof(ulong),
                ["UnsignedBigInt"] = typeof(ulong),
            };
        }

        public DbTable GetTable(string tableSchema, string tableName)
        {
            return
                Tables.SingleOrDefault(t => t.SchemaName.Equals(tableSchema, StringComparison.InvariantCultureIgnoreCase) && t.TableName.Equals(tableName, StringComparison.InvariantCultureIgnoreCase)) ??
                throw new ArgumentException($"Table not found: {tableSchema}.{tableName}");
        }

        public List<DbTable> Tables { get; } = new List<DbTable>();
        public List<DbRelationship> Relationships { get; } = new List<DbRelationship>();
    }
}
