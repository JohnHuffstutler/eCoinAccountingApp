using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using eCoinAccountingApp.Data;
using eCoinAccountingApp.Models;
using Microsoft.AspNetCore.Identity;

namespace eCoinAccountingApp.Pages.Journal
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Models.Journal JournalEntry { get; set; }

        [BindProperty]
        public List<JournalTransaction> Transactions { get; set; } = new List<JournalTransaction>();

        public List<Models.Account> Accounts { get; set; }

        public bool IsAdminOrManager { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            IsAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");

            Accounts = await _context.Accounts.ToListAsync();

            JournalEntry = await _context.Journals
                .Include(j => j.Transactions)
                    .ThenInclude(t => t.Account)
                .FirstOrDefaultAsync(m => m.JournalEntryId == id);

            if (JournalEntry == null)
            {
                return NotFound();
            }

            // Populate Transactions for binding
            Transactions = JournalEntry.Transactions.ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Accounts = await _context.Accounts.ToListAsync();

            var currentUser = await _userManager.GetUserAsync(User);
            IsAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");

            // Remove empty transactions
            Transactions = Transactions
                .Where(t => t.AccountId != 0 && (t.Debit != null || t.Credit != null))
                .ToList();

            // Validate that total debits equal total credits
            decimal totalDebit = Transactions.Sum(t => t.Debit ?? 0);
            decimal totalCredit = Transactions.Sum(t => t.Credit ?? 0);

            if (totalDebit != totalCredit)
            {
                ModelState.AddModelError(string.Empty, "Total debits must equal total credits.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch the existing JournalEntry from the database
            var existingJournalEntry = await _context.Journals
                .Include(j => j.Transactions)
                .FirstOrDefaultAsync(j => j.JournalEntryId == JournalEntry.JournalEntryId);

            if (existingJournalEntry == null)
            {
                return NotFound();
            }

            // Update JournalEntry fields
            existingJournalEntry.Description = JournalEntry.Description;

            // Only Admin or Manager can update the Status
            if (IsAdminOrManager)
            {
                existingJournalEntry.Status = JournalEntry.Status;
            }

            // Update Transactions
            // Remove existing transactions not in the updated list
            var updatedTransactionIds = Transactions.Where(t => t.JournalTransactionId != 0).Select(t => t.JournalTransactionId).ToList();
            var transactionsToRemove = existingJournalEntry.Transactions
                .Where(t => !updatedTransactionIds.Contains(t.JournalTransactionId))
                .ToList();

            _context.JournalTransactions.RemoveRange(transactionsToRemove);

            // Update or Add Transactions
            foreach (var transaction in Transactions)
            {
                if (transaction.JournalTransactionId == 0)
                {
                    // New transaction
                    transaction.JournalEntryId = existingJournalEntry.JournalEntryId;
                    _context.JournalTransactions.Add(transaction);
                }
                else
                {
                    // Existing transaction
                    var existingTransaction = existingJournalEntry.Transactions
                        .FirstOrDefault(t => t.JournalTransactionId == transaction.JournalTransactionId);

                    if (existingTransaction != null)
                    {
                        existingTransaction.AccountId = transaction.AccountId;
                        existingTransaction.Debit = transaction.Debit;
                        existingTransaction.Credit = transaction.Credit;
                        existingTransaction.Description = transaction.Description;
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JournalEntryExists(JournalEntry.JournalEntryId))
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

        private bool JournalEntryExists(int id)
        {
            return _context.Journals.Any(e => e.JournalEntryId == id);
        }
    }
}
