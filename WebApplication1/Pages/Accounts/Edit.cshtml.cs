using eCoinAccountingApp.Data;
using eCoinAccountingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace eCoinAccountingApp.Pages.Account
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public eCoinAccountingApp.Models.Account Account { get; set; }

        public string successMessage = string.Empty;
        public string errorMessage = string.Empty;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Load the account based on the ID
            Account = await _context.Accounts.FindAsync(id);

            if (Account == null)
            {
                errorMessage = "Account not found.";
                return RedirectToPage("/Accounts/Index"); // Redirect back to the list page
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
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

            // Update the account fields except for Id and DateAdded
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
            existingAccount.UserId = Account.UserId;
            existingAccount.Order = Account.Order;
            existingAccount.Statement = Account.Statement;
            existingAccount.Comment = Account.Comment;

            // Save the changes to the database
            await _context.SaveChangesAsync();

            successMessage = "Account updated successfully.";
            return RedirectToPage("/Accounts/Index");
        }
    }
}
