using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using eCoinAccountingApp.Models;
using eCoinAccountingApp.Data;
using System.Linq;

namespace eCoinAccountingApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Properties to hold the values for the view
        public decimal TotalAssets { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal DebtRatio { get; set; }
        public decimal CurrentRatio { get; set; }

        public void OnGet()
        {
            // Fetch all accounts (adjust for filtering by company or user if needed)
            var accounts = _context.Accounts.ToList();

            // Calculate the total assets and liabilities
            TotalAssets = accounts
                .Where(a => a.AccountCategory == "Asset")
                .Sum(a => a.Balance);

            TotalLiabilities = accounts
                .Where(a => a.AccountCategory == "Liability")
                .Sum(a => a.Balance);

            // Calculate the Debt Ratio (handle division by zero)
            CurrentRatio = TotalAssets != 0 ? TotalLiabilities / TotalAssets : 0;
            DebtRatio = TotalLiabilities != 0 ? TotalAssets / TotalLiabilities : 0;
        }
    }
}
