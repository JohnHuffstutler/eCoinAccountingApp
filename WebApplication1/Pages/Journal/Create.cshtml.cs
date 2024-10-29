using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using eCoinAccountingApp.Data;
using eCoinAccountingApp.Models;
using eCoinAccountingApp.Services;
using System.ComponentModel;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace eCoinAccountingApp.Pages.Journal
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly EventLogger _eventLogger;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(ApplicationDbContext context, EventLogger eventLogger, IEmailSender emailSender, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _eventLogger = eventLogger;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        // Property for dropdown list of accounts
        public List<Models.Account> Accounts { get; set; }

        [BindProperty]
        public Models.Journal Journal { get; set; } = default!;

        [BindProperty]
        public int SelectedAccountId { get; set; }  // Stores selected account ID from dropdown

        [BindProperty]
        public IFormFile UploadedDocument { get; set; } // For file uploads


        public async Task<IActionResult> OnGetAsync()
        {
            // Populate the dropdown with available accounts from chart of accounts
            Accounts = await _context.Accounts.ToListAsync();
            Journal = new Models.Journal
            {
                DateAdded = DateTime.Now  // Set the date automatically to the current date
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Repopulate the dropdown list on post
            Accounts = await _context.Accounts.ToListAsync();

            // Validate that an account is selected and either Debit or Credit is filled (but not both)
            if (SelectedAccountId == 0)
            {
                ModelState.AddModelError("SelectedAccountId", "Please select an account.");
            }
            if (Journal.Debit == 0 && Journal.Credit == 0)
            {
                ModelState.AddModelError("Journal.Debit", "Enter a value for either Debit or Credit.");
            }
            if (Journal.Debit != 0 && Journal.Credit != 0)
            {
                ModelState.AddModelError("Journal.Debit", "Only one of Debit or Credit can be filled.");
            }
                  
            ModelState.Remove("Journal.Account");
            ModelState.Remove("Journal.Description");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Set the account and user data
            Journal.AccountId = SelectedAccountId;
            Journal.DateAdded = DateTime.Now;

            // File upload handling
            if (UploadedDocument != null && UploadedDocument.Length > 0)
            {
                string fileName = Path.GetFileName(UploadedDocument.FileName);
                string filePath = Path.Combine("wwwroot/uploads", fileName); // Update this path as needed

                // Ensure the uploads directory exists
                if (!Directory.Exists(Path.Combine("wwwroot", "uploads")))
                {
                    Directory.CreateDirectory(Path.Combine("wwwroot", "uploads"));
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadedDocument.CopyToAsync(stream);
                }

                // Save only the relative path (without wwwroot)
                Journal.DocumentPath = "uploads/" + fileName; 
                
            }

            // Save the journal entry
            _context.Journals.Add(Journal);
            await _context.SaveChangesAsync();  

            // Log the event with a description of the action
            var description = $"Created journal entry for account {Journal.AccountId} with {(Journal.Debit != 0 ? $"Debit: {Journal.Debit}" : $"Credit: {Journal.Credit}")}";
            await _eventLogger.LogEventAsync(description, User?.Identity?.Name);

            var adminUsers = await _userManager.Users.Where(u => u.Role == "Admin").ToListAsync();
            foreach (var admin in adminUsers)
            {
                var subject = "Journal Entry Submitted for Approval";
                var message = $"Journal Entry: \"{Journal.Description}\" has been submitted for approval.";
                await _emailSender.SendEmailAsync(admin.Email, subject, message);
            }

            return RedirectToPage("/Journal/Index");
        }
    }
}
    