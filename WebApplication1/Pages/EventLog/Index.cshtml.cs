using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using eCoinAccountingApp.Models;
using eCoinAccountingApp.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eCoinAccountingApp.Pages.EventLog
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<eCoinAccountingApp.Models.EventLog> EventLogs { get; set; }

        public async Task OnGetAsync()
        {
            EventLogs = await _context.EventLogs.ToListAsync();
        }
    }
}
