using eCoinAccountingApp.Data;
using eCoinAccountingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace eCoinAccountingApp.Pages.Account
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public eCoinAccountingApp.Models.Account Account { get; set; }

        public string successMessage { get; set; } = string.Empty;
        public string errorMessage { get; set; } = string.Empty;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            Account = new eCoinAccountingApp.Models.Account();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Account.AccountName) || string.IsNullOrWhiteSpace(Account.AccountCategory))
            {
                errorMessage = "Please fill out all required fields.";
                return Page();
            }

            try
            {
                // Auto-generate account number based on the category (1xxx for Asset, 2xxx for Liability)
                string prefix = Account.AccountCategory == "Asset" ? "1000" : "2000";
                string normalSide = Account.AccountCategory == "Asset" ? "Debit" : "Credit";

                Account.NormalSide = normalSide;

                // Find the highest existing account number in the current category
                var maxAccountNumber = await _context.Accounts
                    .Where(a => a.AccountNumber.StartsWith(prefix))
                    .OrderByDescending(a => a.AccountNumber)
                    .Select(a => a.AccountNumber)
                    .FirstOrDefaultAsync();

                int newAccountNumber;

                if (maxAccountNumber != null)
                {
                    newAccountNumber = int.Parse(maxAccountNumber) + 10;
                }
                else
                {
                    newAccountNumber = int.Parse(prefix);
                }

                // Format the new account number as a four-digit string (e.g., 1000, 2000)
                Account.AccountNumber = newAccountNumber.ToString();

                // Ensure no duplicate account number exists
                var duplicateAccount = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountNumber == Account.AccountNumber);

                if (duplicateAccount != null)
                {
                    errorMessage = "An account with this number already exists.";
                    return Page();
                }

                // Set the date the account was added
                Account.DateAdded = System.DateTime.Now;

                // Add the new account to the database
                _context.Accounts.Add(Account);
                await _context.SaveChangesAsync();

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
