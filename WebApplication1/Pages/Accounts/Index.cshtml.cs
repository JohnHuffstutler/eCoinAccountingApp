using eCoinAccountingApp.Data;
using eCoinAccountingApp.Models;
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

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // List of companies for the dropdown
        public List<eCoinAccountingApp.Models.Company> Companies { get; set; } = new List<eCoinAccountingApp.Models.Company>();

        // Accounts for the selected company
        public List<eCoinAccountingApp.Models.Account> SelectedCompanyAccounts { get; set; } = new List<eCoinAccountingApp.Models.Account>();

        // Property to store the selected company ID
        [BindProperty(SupportsGet = true)]
        public int? SelectedCompanyId { get; set; }

        public async Task OnGetAsync()
        {
            // Load all companies
            Companies = await _context.Companies.ToListAsync();

            // If a company is selected, load its accounts
            if (SelectedCompanyId.HasValue)
            {
                SelectedCompanyAccounts = await _context.Accounts
                    .Where(a => a.CompanyId == SelectedCompanyId.Value)
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var accountToDelete = await _context.Accounts.FindAsync(id);

            if (accountToDelete == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(accountToDelete);
            await _context.SaveChangesAsync();

            // Reload the page after deletion
            return RedirectToPage(new { SelectedCompanyId });
        }
    }
}
