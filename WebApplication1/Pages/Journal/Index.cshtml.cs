using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using eCoinAccountingApp.Data;
using eCoinAccountingApp.Models;

namespace eCoinAccountingApp.Pages.Journal
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.Journal> Journals { get; set; } = new List<Models.Journal>();

        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "asc";

        public async Task OnGetAsync()
        {
            var query = _context.Journals
                .Include(j => j.Transactions)
                    .ThenInclude(t => t.Account)
                .AsQueryable();

            // Search Logic
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                query = query.Where(j =>
                    j.JournalEntryId.ToString().Contains(SearchQuery) ||
                    j.DateAdded.ToString().Contains(SearchQuery) ||
                    j.Description.Contains(SearchQuery) ||
                    j.Status.Contains(SearchQuery) ||
                    j.Transactions.Any(t =>
                        t.Account.AccountName.Contains(SearchQuery) ||
                        t.Description.Contains(SearchQuery) ||
                        t.Debit.ToString().Contains(SearchQuery) ||
                        t.Credit.ToString().Contains(SearchQuery)
                    )
                );
            }

            // Sorting Logic
            query = SortJournals(query, SortColumn, SortOrder);

            Journals = await query.ToListAsync();
        }

        private IQueryable<Models.Journal> SortJournals(IQueryable<Models.Journal> query, string sortColumn, string sortOrder)
        {
            switch (sortColumn)
            {
                case "JournalEntryId":
                    query = sortOrder == "asc" ? query.OrderBy(j => j.JournalEntryId) : query.OrderByDescending(j => j.JournalEntryId);
                    break;
                case "DateAdded":
                    query = sortOrder == "asc" ? query.OrderBy(j => j.DateAdded) : query.OrderByDescending(j => j.DateAdded);
                    break;
                case "Description":
                    query = sortOrder == "asc" ? query.OrderBy(j => j.Description) : query.OrderByDescending(j => j.Description);
                    break;
                case "Status":
                    query = sortOrder == "asc" ? query.OrderBy(j => j.Status) : query.OrderByDescending(j => j.Status);
                    break;
                default:
                    query = query.OrderBy(j => j.JournalEntryId);
                    break;
            }
            return query;
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var journalEntry = await _context.Journals
                .Include(j => j.Transactions)
                .FirstOrDefaultAsync(j => j.JournalEntryId == id);

            if (journalEntry == null)
            {
                return NotFound();
            }

            _context.Journals.Remove(journalEntry);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
