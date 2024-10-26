using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using eCoinAccountingApp.Data;
using eCoinAccountingApp.Models;

namespace eCoinAccountingApp.Pages.Journal
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Journal Journal { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Journal = await _context.Journals
                .Include(j => j.Account) // Include account if needed for display
                .FirstOrDefaultAsync(m => m.JournalNum == id);

            if (Journal == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Ignore validation for `Journal.Account` as it's not being edited
            ModelState.Remove("Journal.Account");
            ModelState.Remove("Journal.Account.AccountName");
            ModelState.Remove("Journal.Account.AccountNumber");
            ModelState.Remove("Journal.Account.NormalSide");
            ModelState.Remove("Journal.Account.AccountCategory");
            ModelState.Remove("Journal.Account.AccountSubcategory");
            ModelState.Remove("Journal.Account.UserId");
            ModelState.Remove("Journal.Account.Comment");
            ModelState.Remove("Journal.Account.Company");
            ModelState.Remove("Journal.Account.Journals");
            ModelState.Remove("Journal.Account.Statement");
            ModelState.Remove("Journal.Account.AccountSubcategory");
            ModelState.Remove("Journal.Account.AccountDescription");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch the existing Journal from the database
            var existingJournal = await _context.Journals.FindAsync(Journal.JournalNum);
            if (existingJournal == null)
            {
                return NotFound();
            }

            // Update only the fields that are allowed to be modified
            existingJournal.Description = Journal.Description;
            existingJournal.Debit = Journal.Debit;
            existingJournal.Credit = Journal.Credit;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JournalExists(Journal.JournalNum))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Journal/Index");
        }

        private bool JournalExists(int id)
        {
            return _context.Journals.Any(e => e.JournalNum == id);
        }
    }
}
