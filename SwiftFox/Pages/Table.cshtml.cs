using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SwiftFox.Data.Schema;

namespace SwiftFox.Pages
{
    public class TableModel : PageModel
    {
        private readonly ILogger<TableModel> logger;
        private readonly DatabaseSchema schema;

        public DbTable Table { get; set; } = null!;

        public TableModel(ILogger<TableModel> logger, DatabaseSchema schema)
        {
            this.logger = logger;
            this.schema = schema;
        }

        public ActionResult OnGet([FromRoute] string tableSchema, [FromRoute] string tableName)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            try
            {
                Table = schema.GetTable(tableSchema, tableName);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Suppressed {ExceptionType}: {ExceptionMessage}", ex.GetBaseException().GetType(), ex.GetBaseException().Message);
                if (!ModelState.IsValid)
                {
                    return NotFound();
                }
            }

            return Page();
        }
    }
}
