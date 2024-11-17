namespace eCoinAccountingApp.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }

        public List<Account> Accounts { get; set; }
    }
}
