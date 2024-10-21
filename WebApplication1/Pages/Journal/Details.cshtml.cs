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
    public class DetailsModel : PageModel
    {
        private readonly eCoinAccountingApp.Data.ApplicationDbContext _context;

        public DetailsModel(eCoinAccountingApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public eCoinAccountingApp.Models.Journal Journals { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journal = await _context.Journals.FirstOrDefaultAsync(m => m.journalNum == id);
            if (journal == null)
            {
                return NotFound();
            }
            else
            {
                Journals = journal;
            }
            return Page();
        }
    }
}
