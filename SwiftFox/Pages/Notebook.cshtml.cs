using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace SwiftFox.Pages
{
    public class NotebookModel : PageModel
    {
        [FromRoute]
        [Required]
        public string Id { get; set; } = null!;

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
