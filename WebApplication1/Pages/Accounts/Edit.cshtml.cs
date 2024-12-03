using eCoinAccountingApp.Data;
using eCoinAccountingApp.Models;
using eCoinAccountingApp.Services; // Assuming EventLogger is in the Services folder
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eCoinAccountingApp.Pages.Account
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EventLogger _eventLogger; // Inject the EventLogger

        [BindProperty]
        public eCoinAccountingApp.Models.Account Account { get; set; }

        public string CompanyName { get; set; } // For displaying the company name
        public string successMessage = string.Empty;
        public string errorMessage = string.Empty;

        public EditModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, EventLogger eventLogger)
        {
            _context = context;
            _userManager = userManager;
            _eventLogger = eventLogger; // Initialize the EventLogger
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Load the account based on the ID
            Account = await _context.Accounts.Include(a => a.Company).FirstOrDefaultAsync(a => a.Id == id);

            if (Account == null)
            {
                errorMessage = "Account not found.";
                return RedirectToPage("/Accounts/Index");
            }

            // Store the company name to display in the UI
            CompanyName = Account.Company?.CompanyName ?? "Unknown";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Account.UserId");
            ModelState.Remove("Account.Company");
            ModelState.Remove("Account.Journals");

            if (!ModelState.IsValid)
            {
                return Page(); // Reload page with validation errors
            }

            // Find the existing account from the database
            var existingAccount = await _context.Accounts.FindAsync(Account.Id);

            if (existingAccount == null)
            {
                errorMessage = "Account not found.";
                return RedirectToPage("/Accounts/Index");
            }

            // Get the current logged-in user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                errorMessage = "User is not logged in.";
                return Page();
            }

            // Track changes to fields for logging
            var changes = new List<string>();

            if (existingAccount.AccountName != Account.AccountName)
                changes.Add($"Account Name changed from '{existingAccount.AccountName}' to '{Account.AccountName}'");

            if (existingAccount.AccountNumber != Account.AccountNumber)
                changes.Add($"Account Number changed from '{existingAccount.AccountNumber}' to '{Account.AccountNumber}'");

            if (existingAccount.AccountDescription != Account.AccountDescription)
                changes.Add($"Description changed from '{existingAccount.AccountDescription}' to '{Account.AccountDescription}'");

            if (existingAccount.NormalSide != Account.NormalSide)
                changes.Add($"Normal Side changed from '{existingAccount.NormalSide}' to '{Account.NormalSide}'");

            if (existingAccount.AccountCategory != Account.AccountCategory)
                changes.Add($"Category changed from '{existingAccount.AccountCategory}' to '{Account.AccountCategory}'");

            if (existingAccount.AccountSubcategory != Account.AccountSubcategory)
                changes.Add($"Subcategory changed from '{existingAccount.AccountSubcategory}' to '{Account.AccountSubcategory}'");

            if (existingAccount.InitialBalance != Account.InitialBalance)
                changes.Add($"Initial Balance changed from '{existingAccount.InitialBalance}' to '{Account.InitialBalance}'");

            if (existingAccount.Debit != Account.Debit)
                changes.Add($"Debit changed from '{existingAccount.Debit}' to '{Account.Debit}'");

            if (existingAccount.Credit != Account.Credit)
                changes.Add($"Credit changed from '{existingAccount.Credit}' to '{Account.Credit}'");

            if (existingAccount.Balance != Account.Balance)
                changes.Add($"Balance changed from '{existingAccount.Balance}' to '{Account.Balance}'");

            if (existingAccount.Order != Account.Order)
                changes.Add($"Order changed from '{existingAccount.Order}' to '{Account.Order}'");

            if (existingAccount.Statement != Account.Statement)
                changes.Add($"Statement changed from '{existingAccount.Statement}' to '{Account.Statement}'");

            if (existingAccount.Comment != Account.Comment)
                changes.Add($"Comment changed from '{existingAccount.Comment}' to '{Account.Comment}'");

            // Update the account fields except for Id, DateAdded, Company, and UserId
            existingAccount.AccountName = Account.AccountName;
            existingAccount.AccountNumber = Account.AccountNumber;
            existingAccount.AccountDescription = Account.AccountDescription;
            existingAccount.NormalSide = Account.NormalSide;
            existingAccount.AccountCategory = Account.AccountCategory;
            existingAccount.AccountSubcategory = Account.AccountSubcategory;
            existingAccount.InitialBalance = Account.InitialBalance;
            existingAccount.Debit = Account.Debit;
            existingAccount.Credit = Account.Credit;
            existingAccount.Balance = Account.Balance;
            existingAccount.Order = Account.Order;
            existingAccount.Statement = Account.Statement;
            existingAccount.Comment = Account.Comment;

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Log the changes to the event log
            if (changes.Count > 0)
            {
                var description = $"User {currentUser.UserName} edited Account ID {existingAccount.Id}. Changes: {string.Join(", ", changes)}.";
                await _eventLogger.LogEventAsync(description, currentUser.Id);
            }

            successMessage = "Account updated successfully.";
            return RedirectToPage("/Accounts/Index");
        }
    }
}
