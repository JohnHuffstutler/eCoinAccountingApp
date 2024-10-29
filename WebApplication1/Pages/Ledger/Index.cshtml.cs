using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eCoinAccountingApp.Data;
using eCoinAccountingApp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eCoinAccountingApp.Pages.Ledger
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<JournalTransaction> LedgerTransactions { get; set; } = new List<JournalTransaction>();

        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "asc";

        public async Task OnGetAsync()
        {
            var query = _context.JournalTransactions
                .Include(t => t.JournalEntry)
                .Include(t => t.Account)
                .Where(t => t.JournalEntry.Status == "Approved")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                query = query.Where(t =>
                    t.JournalEntry.JournalEntryId.ToString().Contains(SearchQuery) ||
                    t.JournalEntry.DateAdded.ToString().Contains(SearchQuery) ||
                    t.Account.AccountNumber.Contains(SearchQuery) ||
                    t.Account.AccountName.Contains(SearchQuery) ||
                    t.Account.NormalSide.Contains(SearchQuery) ||
                    t.Description.Contains(SearchQuery) ||
                    (t.Debit.HasValue && t.Debit.Value.ToString().Contains(SearchQuery)) ||
                    (t.Credit.HasValue && t.Credit.Value.ToString().Contains(SearchQuery))
                );
            }

            query = SortTransactions(query, SortColumn, SortOrder);

            LedgerTransactions = await query.ToListAsync();
        }

        private IQueryable<JournalTransaction> SortTransactions(IQueryable<JournalTransaction> query, string sortColumn, string sortOrder)
        {
            return sortColumn switch
            {
                "JournalEntryId" => sortOrder == "asc" ? query.OrderBy(t => t.JournalEntry.JournalEntryId) : query.OrderByDescending(t => t.JournalEntry.JournalEntryId),
                "DateAdded" => sortOrder == "asc" ? query.OrderBy(t => t.JournalEntry.DateAdded) : query.OrderByDescending(t => t.JournalEntry.DateAdded),
                "AccountNumber" => sortOrder == "asc" ? query.OrderBy(t => t.Account.AccountNumber) : query.OrderByDescending(t => t.Account.AccountNumber),
                "AccountName" => sortOrder == "asc" ? query.OrderBy(t => t.Account.AccountName) : query.OrderByDescending(t => t.Account.AccountName),
                "NormalSide" => sortOrder == "asc" ? query.OrderBy(t => t.Account.NormalSide) : query.OrderByDescending(t => t.Account.NormalSide),
                "Description" => sortOrder == "asc" ? query.OrderBy(t => t.Description) : query.OrderByDescending(t => t.Description),
                "Debit" => sortOrder == "asc" ? query.OrderBy(t => t.Debit) : query.OrderByDescending(t => t.Debit),
                "Credit" => sortOrder == "asc" ? query.OrderBy(t => t.Credit) : query.OrderByDescending(t => t.Credit),
                _ => query.OrderBy(t => t.JournalEntry.JournalEntryId),
            };
        }
    }
}
