using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace SwiftFox.Data.Schema
{
    /// <summary>
    /// A singleton service that is the <see cref="DbSchema"/>
    /// of the <see cref="SwiftFoxOptions.MainConnectionString"/> database.
    /// Call <see cref="RefreshAsync"/> to refresh this schema.
    /// </summary>
    [Service(ServiceLifetime.Singleton)]
    public class DatabaseSchema : DbSchema
    {
        private readonly IOptions<SwiftFoxOptions> options;
        private readonly DbSchemaBuilder schemaBuilder;

        public DatabaseSchema(IOptions<SwiftFoxOptions> options, DbSchemaBuilder schemaBuilder)
        {
            this.options = options;
            this.schemaBuilder = schemaBuilder;

            RefreshAsync().Wait();
        }

        /// <summary>
        /// Refresh this schema.
        /// </summary>
        /// <returns></returns>
        public async Task RefreshAsync()
        {
            Tables.Clear();
            Relationships.Clear();
            using var conn = new SqlConnection(options.Value.MainConnectionString);
            await schemaBuilder.BuildAsync(this, conn);
        }
    }
}
