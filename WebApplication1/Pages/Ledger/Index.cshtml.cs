using System;
using System.Collections.Generic;
using System.Linq;
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

        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "asc"; 

        public async Task OnGetAsync()
        {
            var query = _context.Journals
                .Include(j => j.Account)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                query = query.Where(j =>
                    j.JournalNum.ToString().Contains(SearchQuery) ||         
                    j.DateAdded.ToString().Contains(SearchQuery) ||          
                    j.Account.AccountNumber.Contains(SearchQuery) ||        
                    j.Account.AccountName.Contains(SearchQuery) ||           
                    j.Account.NormalSide.Contains(SearchQuery) ||            
                    j.Description.Contains(SearchQuery) ||                   
                    j.Debit.ToString().Contains(SearchQuery) ||             
                    j.Credit.ToString().Contains(SearchQuery));              
            }

            query = SortJournals(query, SortColumn, SortOrder);

            Journals = await query.ToListAsync();
        }

        private IQueryable<Models.Journal> SortJournals(IQueryable<Models.Journal> query, string sortColumn, string sortOrder)
        {
            return sortColumn switch
            {
                "JournalNum" => sortOrder == "asc" ? query.OrderBy(j => j.JournalNum) : query.OrderByDescending(j => j.JournalNum),
                "DateAdded" => sortOrder == "asc" ? query.OrderBy(j => j.DateAdded) : query.OrderByDescending(j => j.DateAdded),
                "AccountNumber" => sortOrder == "asc" ? query.OrderBy(j => j.Account.AccountNumber) : query.OrderByDescending(j => j.Account.AccountNumber),
                "AccountName" => sortOrder == "asc" ? query.OrderBy(j => j.Account.AccountName) : query.OrderByDescending(j => j.Account.AccountName),
                "NormalSide" => sortOrder == "asc" ? query.OrderBy(j => j.Account.NormalSide) : query.OrderByDescending(j => j.Account.NormalSide),
                "Description" => sortOrder == "asc" ? query.OrderBy(j => j.Description) : query.OrderByDescending(j => j.Description),
                "Debit" => sortOrder == "asc" ? query.OrderBy(j => j.Debit) : query.OrderByDescending(j => j.Debit),
                "Credit" => sortOrder == "asc" ? query.OrderBy(j => j.Credit) : query.OrderByDescending(j => j.Credit),
                _ => query.OrderBy(j => j.JournalNum), // Default sort
            };
        }
    }
}
