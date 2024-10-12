using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eCoinAccountingApp.Pages
{
    public class LedgerModel : PageModel
    {
        private readonly ILogger<LedgerModel> _logger;

        public LedgerModel(ILogger<LedgerModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
