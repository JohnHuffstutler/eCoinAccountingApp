using eCoinAccountingApp.Data;
using eCoinAccountingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace eCoinAccountingApp.Pages.Company
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public eCoinAccountingApp.Models.Company Company { get; set; }

        public string successMessage { get; set; } = string.Empty;
        public string errorMessage { get; set; } = string.Empty;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            Company = new eCoinAccountingApp.Models.Company();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Company.CompanyName))
            {
                errorMessage = "Please fill out the company name.";
                return Page();
            }

            try
            {
                // Add the new company to the database
                _context.Companies.Add(Company);
                await _context.SaveChangesAsync();

                successMessage = "Company has been added successfully.";
                return RedirectToPage("/Accounts/Index"); // Redirect to a list of companies
            }
            catch (Exception ex)
            {
                errorMessage = $"An error occurred: {ex.Message}";
                return Page();
            }
        }
    }
}
