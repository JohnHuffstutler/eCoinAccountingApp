using eCoinAccountingApp.Data;
using eCoinAccountingApp.Models;
using eCoinAccountingApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCoinAccountingApp.Pages.Account
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EventLogger _eventLogger;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, EventLogger eventLogger)
        {
            _context = context;
            _userManager = userManager;
            _eventLogger = eventLogger;
        }

        // List of companies for the dropdown
        public List<eCoinAccountingApp.Models.Company> Companies { get; set; } = new List<eCoinAccountingApp.Models.Company>();

        // Accounts for the selected company
        public List<eCoinAccountingApp.Models.Account> SelectedCompanyAccounts { get; set; } = new List<eCoinAccountingApp.Models.Account>();

        // Property to store the selected company ID
        [BindProperty(SupportsGet = true)]
        public int? SelectedCompanyId { get; set; }

        // New property for search functionality
        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; }

        // Sort properties
        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "asc"; // Default to ascending

        public async Task OnGetAsync()
        {
            // Load all companies
            Companies = await _context.Companies.ToListAsync();

            // If a company is selected, load its accounts
            if (SelectedCompanyId.HasValue)
            {
                var query = _context.Accounts
                    .Where(a => a.CompanyId == SelectedCompanyId.Value)
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(SearchQuery))
                {
                    query = query.Where(a => a.AccountName.Contains(SearchQuery) || a.AccountNumber.Contains(SearchQuery));
                }

                // Apply sorting
                query = SortAccounts(query, SortColumn, SortOrder);

                SelectedCompanyAccounts = await query.ToListAsync();
            }
        }

        // Method to apply sorting based on the column and order
        private IQueryable<eCoinAccountingApp.Models.Account> SortAccounts(IQueryable<eCoinAccountingApp.Models.Account> query, string sortColumn, string sortOrder)
        {
            switch (sortColumn)
            {
                case "AccountName":
                    query = sortOrder == "asc" ? query.OrderBy(a => a.AccountName) : query.OrderByDescending(a => a.AccountName);
                    break;
                case "AccountNumber":
                    query = sortOrder == "asc" ? query.OrderBy(a => a.AccountNumber) : query.OrderByDescending(a => a.AccountNumber);
                    break;
                case "InitialBalance":
                    query = sortOrder == "asc" ? query.OrderBy(a => a.InitialBalance) : query.OrderByDescending(a => a.InitialBalance);
                    break;
                case "Debit":
                    query = sortOrder == "asc" ? query.OrderBy(a => a.Debit) : query.OrderByDescending(a => a.Debit);
                    break;
                case "Credit":
                    query = sortOrder == "asc" ? query.OrderBy(a => a.Credit) : query.OrderByDescending(a => a.Credit);
                    break;
                case "Balance":
                    query = sortOrder == "asc" ? query.OrderBy(a => a.Balance) : query.OrderByDescending(a => a.Balance);
                    break;
                case "DateAdded":
                    query = sortOrder == "asc" ? query.OrderBy(a => a.DateAdded) : query.OrderByDescending(a => a.DateAdded);
                    break;
                case "Statement":
                    query = sortOrder == "asc" ? query.OrderBy(a => a.Statement) : query.OrderByDescending(a => a.Statement);
                    break;
                case "Order":
                    query = sortOrder == "asc" ? query.OrderBy(a => a.Order) : query.OrderByDescending(a => a.Order);
                    break;
                default:
                    query = query.OrderBy(a => a.Id); // Default sort by Id
                    break;
            }
            return query;
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var accountToDelete = await _context.Accounts.FindAsync(id);

            if (accountToDelete == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Page();
            }

            _context.Accounts.Remove(accountToDelete);
            await _context.SaveChangesAsync();

            await _eventLogger.LogEventAsync($"Account '{accountToDelete.AccountName}' deactivated.", currentUser.Id);

            // Reload the page after deletion
            return RedirectToPage(new { SelectedCompanyId });
        }
    }
}
