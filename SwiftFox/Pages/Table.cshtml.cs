using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace SwiftFox.Pages
{
    public class TableModel : PageModel
    {
        [FromRoute]
        [Required]
        public string Schema { get; set; } = null!;

        [FromRoute]
        [Required]
        public string Table { get; set; } = null!;

        public ActionResult OnGet()
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
