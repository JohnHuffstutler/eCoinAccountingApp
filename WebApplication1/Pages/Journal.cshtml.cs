using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eCoinAccountingApp.Pages
{
    public class JournalModel : PageModel
    {
        private readonly ILogger<JournalModel> _logger;

        public JournalModel(ILogger<JournalModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {
        }
    }
}
