using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace Swiftfox.Data.Schema
{
    /// <summary>
    /// A singleton service that is the <see cref="DbSchema"/>
    /// of the <see cref="SwiftfoxOptions.MainConnectionString"/> database.
    /// Refresh this schema with a call to <see cref="RefreshAsync"/>.
    /// </summary>
    [Service(ServiceLifetime.Singleton)]
    public class DatabaseSchema : DbSchema
    {
        private readonly IOptions<SwiftfoxOptions> options;
        private readonly DbSchemaBuilder schemaBuilder;

        public DatabaseSchema(IOptions<SwiftfoxOptions> options, DbSchemaBuilder schemaBuilder)
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
            DbSchema tempSchema = new();

            using (var conn = new SqlConnection(options.Value.MainConnectionString))
            {
                await schemaBuilder.BuildAsync(tempSchema, conn);
            }

            Tables.Clear();
            Tables.AddRange(tempSchema.Tables);
            Relationships.Clear();
            Relationships.AddRange(tempSchema.Relationships);
        }
    }
}
