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
using DinkToPdf;
using DinkToPdf.Contracts;

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
        public async Task<IActionResult> OnPostPrintJournalEntriesAsync(int AccountId, DateTime StartDate, DateTime EndDate)
        {
            if (StartDate > EndDate)
            {
                ModelState.AddModelError(string.Empty, "Start Date cannot be greater than End Date.");
                return Page();
            }

            var journalEntries = await _context.Journals
                .Include(j => j.Account)
                .Where(j => j.AccountId == AccountId && j.DateAdded >= StartDate && j.DateAdded <= EndDate)
                .OrderBy(j => j.DateAdded)
                .ToListAsync();

            if (!journalEntries.Any())
            {
                TempData["Message"] = "No journal entries found for the selected date range.";
                return RedirectToPage();
            }

            string htmlContent = GenerateHtmlForPdf(journalEntries, StartDate, EndDate);

            var converter = new SynchronizedConverter(new PdfTools());
            var pdfDocument = new HtmlToPdfDocument
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 }
                }
            };

            pdfDocument.Objects.Add(new ObjectSettings
            {
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" }
            });

            var pdfBytes = converter.Convert(pdfDocument);

            return File(pdfBytes, "application/pdf", "JournalEntries.pdf");
        }

        private string GenerateHtmlForPdf(List<Models.Journal> journalEntries, DateTime startDate, DateTime endDate)
        {
            var html = $@"
        <html>
        <head>
            <style>
                body {{ font-family: Arial, sans-serif; }}
                table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
                th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
                th {{ background-color: #f2f2f2; }}
            </style>
        </head>
        <body>
            <h1>{journalEntries.First().Account.Statement}</h1>
            <p><strong>Account:</strong> {journalEntries.First().Account.AccountName}</p>
            <p><strong>Date Range:</strong> {startDate:MM/dd/yyyy} - {endDate:MM/dd/yyyy}</p>
            <table>
                <thead>
                    <tr>
                        <th>Date</th>
                        <th>Account</th>
                        <th>Debit</th>
                        <th>Credit</th>
                        <th>Description</th>
                    </tr>
                </thead>
                <tbody>";

            foreach (var entry in journalEntries)
            {
                html += $@"
                    <tr>
                        <td>{entry.DateAdded:MM/dd/yyyy}</td>
                        <td>{entry.Account.AccountName}</td>
                        <td>{entry.Debit:C}</td>
                        <td>{entry.Credit:C}</td>
                        <td>{entry.Description}</td>
                    </tr>";
            }

            html += @"
                </tbody>
            </table>
        </body>
        </html>";

            return html;
        }

    }
}

