using Microsoft.AspNetCore.Mvc;
using SwiftFox.Data;

namespace SwiftFox.Api
{
    [ApiController]
    [Route("api/data")]
    public class DataController : ControllerBase
    {
        private readonly Database database;

        public DataController(Database database)
        {
            this.database = database;
        }

        [HttpGet("query")]
        public async Task<ActionResult<TableQueryResult>> Query([FromJsonQueryParameter] TableQuery query)
        {
            TableQueryResult result = await database.QueryAsync(query);
            return Ok(result);
        }
    }
}
