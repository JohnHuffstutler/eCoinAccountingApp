using eCoinAccountingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eCoinAccountingApp.Pages.User
{
    public class CreateModel : PageModel
    {
        public Models.Users user = new Models.Users();
        public string successMessage = String.Empty;
        public string errorMessage = String.Empty;

        private readonly IConfiguration config;
        public CreateModel(IConfiguration config)
        {
            this.config = config;
        }
        public void OnGet()
        {
        }

        public void OnPost()
        {
            user.firstName = Request.Form["FirstName"];
            user.lastName = Request.Form["LastName"];
            user.userName = Request.Form["Username"];
            user.password = Request.Form["Password"];
            user.role = Request.Form["Role"];
            user.email = Request.Form["Email"];
            user.address = Request.Form["Address"];
            user.dateOfBirth = Request.Form["DateOfBirth"];

            if(user.firstName.Length == 0 || user.lastName.Length == 0 || user.userName.Length == 0 || user.password.Length == 0)
            {
                errorMessage = "Fill required fields.";
                return;
            }

            try
            {
                DAL dal = new DAL();
                int i = dal.AddUser(user, config);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            user.firstName = ""; user.lastName = ""; user.password = ""; user.role = ""; user.email = ""; user.address = ""; 
            successMessage = "User has been added.";
            Response.Redirect("/User/Index");
        }
    }
}
