using Microsoft.AspNetCore.Mvc;
using SwiftFox.Data;

namespace SwiftFox.Api
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DataController : ControllerBase
    {
        private readonly Database database;

        public DataController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        public async Task<ActionResult> Query([FromJsonQueryParameter] TableQuery query)
        {
            TableQueryResult result = await database.QueryAsync(query);
            return Ok(result);
        }
    }
}
