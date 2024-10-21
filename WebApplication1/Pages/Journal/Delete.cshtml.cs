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
    public class DeleteModel : PageModel
    {
        private readonly eCoinAccountingApp.Data.ApplicationDbContext _context;

        public DeleteModel(eCoinAccountingApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public eCoinAccountingApp.Models.Journal Journal { get; set; } = default!;

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
                Journal = journal;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journal = await _context.Journals.FindAsync(id);
            if (journal != null)
            {
                Journal = journal;
                _context.Journals.Remove(Journal);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
