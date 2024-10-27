using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using eCoinAccountingApp.Data;
using eCoinAccountingApp.Models;

namespace eCoinAccountingApp.Pages.Ledger
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.Journal> Journals { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Include Account details for each Journal entry for Ledger display
            Journals = await _context.Journals
                .Include(j => j.Account)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var journal = await _context.Journals.FindAsync(id);
            if (journal == null)
            {
                return NotFound();
            }

            _context.Journals.Remove(journal);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
