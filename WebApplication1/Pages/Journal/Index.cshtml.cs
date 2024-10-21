using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using eCoinAccountingApp.Data;
using eCoinAccountingApp.Models;

namespace eCoinAccountingApp.Pages.Journal
{
    public class IndexModel : PageModel
    {
        private readonly eCoinAccountingApp.Data.ApplicationDbContext _context;

        public IndexModel(eCoinAccountingApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<eCoinAccountingApp.Models.Journal> Journals { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Journals = (IList<eCoinAccountingApp.Models.Journal>)await _context.Journals.ToListAsync();
        }
    }
}
