using eCoinAccountingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eCoinAccountingApp.Pages.User
{
    public class EditModel : PageModel
    {
        public Models.Users user = new Models.Users();
        public string successMessage = String.Empty;
        public string errorMessage = String.Empty;

        private readonly IConfiguration config;
        public EditModel(IConfiguration config)
        {
            this.config = config;
        }

        public void OnGet()
        {
            string id = Request.Query["id"];

            try
            {
                DAL dal = new DAL();
                user = dal.GetUser(id, config);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        
    }
}
