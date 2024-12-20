using eCoinAccountingApp.Data;
using eCoinAccountingApp.Models;
using eCoinAccountingApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly IEmailSender _emailSender;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, EventLogger eventLogger, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _eventLogger = eventLogger;
            _emailSender = emailSender;
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
        public string SortOrder { get; set; } = "asc";

        [BindProperty]
        public string Subject { get; set; }

        [BindProperty]
        public string Body { get; set; }

        public async Task OnGetAsync()
        {
            Companies = await _context.Companies.ToListAsync();

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

                query = SortAccounts(query, SortColumn, SortOrder);

                SelectedCompanyAccounts = await query.ToListAsync();
            }
        }

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
                    query = query.OrderBy(a => a.Id); 
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

            return RedirectToPage(new { SelectedCompanyId });
        }
        public async Task<IActionResult> OnPostSendEmailAsync()
        {
            if (string.IsNullOrWhiteSpace(Subject) || string.IsNullOrWhiteSpace(Body))
            {
                ModelState.AddModelError(string.Empty, "Subject and Body are required.");
                return Page();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            List<ApplicationUser> recipients;
            if (currentUser.Role == "Admin" || currentUser.Role == "Manager")
            {
                recipients = await _userManager.Users
                    .Where(user => user.Role == "User")
                    .ToListAsync();
            }
            else
            {
                recipients = await _userManager.Users
                    .Where(user => user.Role == "Admin" || user.Role == "Manager")
                    .ToListAsync();
            }

            foreach (var user in recipients)
            {
                await _emailSender.SendEmailAsync(user.Email, Subject, Body);
            }

            await _eventLogger.LogEventAsync($"Sent email with subject: '{Subject}'", currentUser.Id);

            return RedirectToPage(new { SelectedCompanyId = SelectedCompanyId });
        }
    }
}

