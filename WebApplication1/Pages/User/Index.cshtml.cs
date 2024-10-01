using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eCoinAccountingApp.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration config;
        public List<Models.Users> listUsers = new List<Models.Users>();

        public IndexModel(IConfiguration config)
        {
            this.config = config;
        }

        public void OnGet()
        {
            Models.DAL dal = new Models.DAL();
            listUsers = dal.GetUsers(config);
        }
    }
}
