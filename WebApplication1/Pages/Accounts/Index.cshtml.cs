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
        public async Task<IActionResult> OnPostPrintTrialBalanceAsync(DateTime StartDate, DateTime EndDate)
        {
            // Validate the date range
            if (StartDate > EndDate)
            {
                ModelState.AddModelError(string.Empty, "Start Date cannot be greater than End Date.");
                return Page();
            }

            // Check if company is selected
            if (!SelectedCompanyId.HasValue)
            {
                TempData["Message"] = "Please select a company.";
                return RedirectToPage();
            }

            // Fetch all accounts for the selected company
            var accounts = await _context.Accounts
                .Where(a => a.CompanyId == SelectedCompanyId.Value)
                .ToListAsync();

            // If no accounts exist for the selected company, return an error
            if (accounts.Count == 0)
            {
                TempData["Message"] = "No accounts found for the selected company.";
                return RedirectToPage();
            }

            // Fetch journal entries for the selected accounts within the date range
            var journalEntries = await _context.Journals
                .Where(j => j.DateAdded >= StartDate && j.DateAdded <= EndDate &&
                            accounts.Select(a => a.Id).Contains(j.AccountId))
                .Include(j => j.Account)
                .OrderBy(j => j.DateAdded)
                .ToListAsync();

            // If no journal entries exist, show a message
            if (!journalEntries.Any())
            {
                TempData["Message"] = "No journal entries found for the selected date range.";
                return RedirectToPage();
            }

            // Calculate trial balance data
            var trialBalance = accounts.Select(account => new TrialBalance
            {
                AccountNumber = account.AccountNumber,
                AccountName = account.AccountName,
                Debit = journalEntries.Where(j => j.AccountId == account.Id && j.Debit > 0).Sum(j => j.Debit),
                Credit = journalEntries.Where(j => j.AccountId == account.Id && j.Credit > 0).Sum(j => j.Credit)
            }).ToList();

            // Generate the PDF with the trial balance data
            string htmlContent = GenerateHtmlForTrialBalancePdf(trialBalance, StartDate, EndDate);

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

            return File(pdfBytes, "application/pdf", "TrialBalance.pdf");
        }

        public async Task<IActionResult> OnPostPrintBalanceSheetAsync(DateTime StartDate, DateTime EndDate)
        {
            // Validate the date range
            if (StartDate > EndDate)
            {
                ModelState.AddModelError(string.Empty, "Start Date cannot be greater than End Date.");
                return Page();
            }

            // Check if company is selected
            if (!SelectedCompanyId.HasValue)
            {
                TempData["Message"] = "Please select a company.";
                return RedirectToPage();
            }

            // Fetch all accounts for the selected company
            var accounts = await _context.Accounts
                .Where(a => a.CompanyId == SelectedCompanyId.Value)
                .ToListAsync();

            // If no accounts exist for the selected company, return an error
            if (accounts.Count == 0)
            {
                TempData["Message"] = "No accounts found for the selected company.";
                return RedirectToPage();
            }

            // Fetch journal entries for the selected accounts within the date range
            var journalEntries = await _context.Journals
                .Where(j => j.DateAdded >= StartDate && j.DateAdded <= EndDate &&
                            accounts.Select(a => a.Id).Contains(j.AccountId))
                .Include(j => j.Account)
                .OrderBy(j => j.DateAdded)
                .ToListAsync();

            // If no journal entries exist, show a message
            if (!journalEntries.Any())
            {
                TempData["Message"] = "No journal entries found for the selected date range.";
                return RedirectToPage();
            }

            // Calculate balance sheet data by grouping accounts based on their category
            var balanceSheet = accounts.Select(account => new BalanceSheet
            {
                AccountName = account.AccountName,
                AccountNumber = account.AccountNumber,
                AccountCategory = account.AccountCategory,
                Debit = journalEntries.Where(j => j.AccountId == account.Id && j.Debit > 0).Sum(j => j.Debit),
                Credit = journalEntries.Where(j => j.AccountId == account.Id && j.Credit > 0).Sum(j => j.Credit)
            }).ToList();

            // Group accounts by category (Assets, Liabilities, Equity)
            var assets = balanceSheet.Where(b => b.AccountCategory == "Asset").ToList();
            var liabilitiesAndEquity = balanceSheet.Where(b => b.AccountCategory != "Asset").ToList();

            // Generate the PDF with the balance sheet data
            string htmlContent = GenerateHtmlForBalanceSheetPdf(assets, liabilitiesAndEquity, StartDate, EndDate);

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

            return File(pdfBytes, "application/pdf", "BalanceSheet.pdf");
        }




        private string GenerateHtmlForTrialBalancePdf(List<TrialBalance> trialBalance, DateTime startDate, DateTime endDate)
        {
            // Calculate the total debits and credits
            decimal totalDebits = trialBalance.Sum(item => item.Debit);
            decimal totalCredits = trialBalance.Sum(item => item.Credit);

            var html = $@"
    <html>
    <head>
        <style>
            body {{ font-family: Arial, sans-serif; }}
            table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
            th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
            th {{ background-color: #f2f2f2; }}
            .total-row td {{ font-weight: bold; }}
        </style>
    </head>
    <body>
        <h1>Trial Balance Report</h1>
        <p><strong>Date Range:</strong> {startDate:MM/dd/yyyy} - {endDate:MM/dd/yyyy}</p>
        <table>
            <thead>
                <tr>
                    <th>Account Number</th>
                    <th>Account Name</th>
                    <th>Debit</th>
                    <th>Credit</th>
                </tr>
            </thead>
            <tbody>";

            // Populate the table with the trial balance data
            foreach (var item in trialBalance)
            {
                html += $@"
            <tr>
                <td>{item.AccountNumber}</td>
                <td>{item.AccountName}</td>
                <td>{item.Debit:C}</td>
                <td>{item.Credit:C}</td>
            </tr>";
            }

            // Add a total row at the bottom
            html += $@"
            <tr class='total-row'>
                <td colspan='2'>Total</td>
                <td>{totalDebits:C}</td>
                <td>{totalCredits:C}</td>
            </tr>
        </tbody>
    </table>
</body>
</html>";

            return html;
        }

        private string GenerateHtmlForBalanceSheetPdf(List<BalanceSheet> assets, List<BalanceSheet> liabilitiesAndEquity, DateTime startDate, DateTime endDate)
        {
            // Calculate totals for assets and liabilities/equity
            decimal totalAssets = assets.Sum(item => item.Debit - item.Credit);
            decimal totalLiabilitiesAndEquity = liabilitiesAndEquity.Sum(item => item.Debit - item.Credit);

            var html = $@"
    <html>
    <head>
        <style>
            body {{ font-family: Arial, sans-serif; }}
            table {{ width: 48%; border-collapse: collapse; float: left; margin-right: 2%; }}
            th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
            th {{ background-color: #f2f2f2; }}
            .total-row td {{ font-weight: bold; }}
            .balance-sheet-section {{ margin-top: 20px; }}
            .section-title {{ font-size: 16px; font-weight: bold; }}
            .clear {{ clear: both; }}
        </style>
    </head>
    <body>
        <h1>Balance Sheet Report</h1>
        <p><strong>Date Range:</strong> {startDate:MM/dd/yyyy} - {endDate:MM/dd/yyyy}</p>

        <!-- Liabilities and Equity Section -->
        <div class='balance-sheet-section'>
            <div class='section-title'>Liabilities & Equity</div>
            <table>
                <thead>
                    <tr>
                        <th>Account Number</th>
                        <th>Account Name</th>
                        <th>Debit</th>
                        <th>Credit</th>
                    </tr>
                </thead>
                <tbody>";

            // Add rows for Liabilities & Equity
            foreach (var item in liabilitiesAndEquity)
            {
                html += $@"
            <tr>
                <td>{item.AccountNumber}</td>
                <td>{item.AccountName}</td>
                <td>{item.Debit:C}</td>
                <td>{item.Credit:C}</td>
            </tr>";
            }

            html += $@"
            <tr class='total-row'>
                <td colspan='2'>Total Liabilities & Equity</td>
                <td>{totalLiabilitiesAndEquity:C}</td>
                <td></td>
            </tr>
        </tbody>
    </table>
</div>

<!-- Clear floats between sections -->
<div class='clear'></div>

<!-- Assets Section -->
<div class='balance-sheet-section'>
    <div class='section-title'>Assets</div>
    <table>
        <thead>
            <tr>
                <th>Account Number</th>
                <th>Account Name</th>
                <th>Debit</th>
                <th>Credit</th>
            </tr>
        </thead>
        <tbody>";

            // Add rows for Assets
            foreach (var item in assets)
            {
                html += $@"
            <tr>
                <td>{item.AccountNumber}</td>
                <td>{item.AccountName}</td>
                <td>{item.Debit:C}</td>
                <td>{item.Credit:C}</td>
            </tr>";
            }

            html += $@"
            <tr class='total-row'>
                <td colspan='2'>Total Assets</td>
                <td>{totalAssets:C}</td>
                <td></td>
            </tr>
        </tbody>
    </table>
</div>

</body>
</html>";

            return html;
        }


    }
}

