using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using eCoinAccountingApp.Data;
using eCoinAccountingApp.Models;

namespace eCoinAccountingApp.Pages.Journal
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Journal JournalEntry { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            JournalEntry = await _context.Journals
                .Include(j => j.Transactions)
                    .ThenInclude(t => t.Account)
                .FirstOrDefaultAsync(m => m.JournalEntryId == id);

            if (JournalEntry == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            JournalEntry = await _context.Journals
                .Include(j => j.Transactions)
                .FirstOrDefaultAsync(j => j.JournalEntryId == id);

            if (JournalEntry != null)
            {
                // Remove associated transactions
                _context.JournalTransactions.RemoveRange(JournalEntry.Transactions);

                // Remove the journal entry
                _context.Journals.Remove(JournalEntry);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Journal/Index");
        }
    }
}
