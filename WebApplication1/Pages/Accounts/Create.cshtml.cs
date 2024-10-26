    using eCoinAccountingApp.Data;
using eCoinAccountingApp.Models;
using eCoinAccountingApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace eCoinAccountingApp.Pages.Account
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EventLogger _eventLogger;   

        public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, EventLogger eventLogger)
        {
            _context = context;
            _userManager = userManager;
            _eventLogger = eventLogger;  // Initialize the EventLogger
        }

        public List<eCoinAccountingApp.Models.Company> Companies { get; set; }

        [BindProperty]
        public eCoinAccountingApp.Models.Account Account { get; set; }

        [BindProperty]
        public int SelectedCompanyId { get; set; }

        public string successMessage { get; set; } = string.Empty;
        public string errorMessage { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            Companies = await _context.Companies.ToListAsync();
            Account = new eCoinAccountingApp.Models.Account();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Companies = await _context.Companies.ToListAsync();

            if (string.IsNullOrWhiteSpace(Account.AccountName) || string.IsNullOrWhiteSpace(Account.AccountCategory))
            {
                errorMessage = "Please fill out all required fields.";
                return Page();
            }

            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    errorMessage = "User is not logged in.";
                    return Page();
                }

                Account.UserId = currentUser.Id;

                string prefix = Account.AccountCategory == "Asset" ? "1000" : "2000";
                string normalSide = Account.AccountCategory == "Asset" ? "Debit" : "Credit";
                Account.NormalSide = normalSide;

                var maxAccountNumber = await _context.Accounts
                    .Where(a => a.AccountNumber.StartsWith(prefix))
                    .OrderByDescending(a => a.AccountNumber)
                    .Select(a => a.AccountNumber)
                    .FirstOrDefaultAsync();

                int newAccountNumber = maxAccountNumber != null ? int.Parse(maxAccountNumber) + 10 : int.Parse(prefix);
                Account.AccountNumber = newAccountNumber.ToString();

                var duplicateAccount = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountNumber == Account.AccountNumber);

                if (duplicateAccount != null)
                {
                    errorMessage = "An account with this number already exists.";
                    return Page();
                }

                Account.DateAdded = System.DateTime.Now;

                var selectedCompany = await _context.Companies.FindAsync(SelectedCompanyId);
                if (selectedCompany == null)
                {
                    errorMessage = "Selected company not found.";
                    return Page();
                }

                Account.Company = selectedCompany;

                _context.Accounts.Add(Account);
                await _context.SaveChangesAsync();

                // Log the event for account creation
                await _eventLogger.LogEventAsync($"Account '{Account.AccountName}' created.", currentUser.Id);

                successMessage = "Account has been added successfully.";
                return RedirectToPage("/Accounts/Index");
            }
            catch (Exception ex)
            {
                errorMessage = $"An error occurred: {ex.Message}";
                return Page();
            }
        }
    }
}
