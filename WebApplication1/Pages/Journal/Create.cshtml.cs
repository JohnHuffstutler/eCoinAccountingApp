using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eCoinAccountingApp.Data;
using eCoinAccountingApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace eCoinAccountingApp.Pages.Journal
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Models.Account> Accounts { get; set; }

        [BindProperty]
        public Models.Journal JournalEntry { get; set; } = new Models.Journal();

        [BindProperty]
        public List<JournalTransaction> Transactions { get; set; } = new List<JournalTransaction>();

        [BindProperty]
        public IFormFile UploadedDocument { get; set; } // For file uploads

        public async Task<IActionResult> OnGetAsync()
        {
            Accounts = await _context.Accounts.ToListAsync();
            JournalEntry.DateAdded = DateTime.Now;
            // Initialize with one empty transaction
            Transactions.Add(new JournalTransaction());
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Accounts = await _context.Accounts.ToListAsync();

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
            if (UploadedDocument != null && UploadedDocument.Length > 0)
            {
                string fileName = Path.GetFileName(UploadedDocument.FileName);
                string uploadsFolder = Path.Combine("wwwroot", "uploads");

                // Ensure the uploads directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadedDocument.CopyToAsync(stream);
                }

                // Save only the relative path (without wwwroot)
                JournalEntry.DocumentPath = Path.Combine("uploads", fileName);
            }
            ModelState.Remove("JournalEntry.DocumentPath");
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Handle file upload
            

            // Save the journal entry and transactions
            _context.Journals.Add(JournalEntry);
            await _context.SaveChangesAsync();

            foreach (var transaction in Transactions)
            {
                transaction.JournalEntryId = JournalEntry.JournalEntryId;
                _context.JournalTransactions.Add(transaction);
            }
            await _context.SaveChangesAsync();

            return RedirectToPage("/Journal/Index");
        }
    }
}
