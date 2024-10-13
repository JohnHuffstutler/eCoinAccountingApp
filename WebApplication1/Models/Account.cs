namespace eCoinAccountingApp.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountDescription { get; set; }
        public string NormalSide { get; set; }
        public string AccountCategory { get; set; }
        public string AccountSubcategory { get; set; }
        public decimal InitialBalance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public DateTime DateAdded { get; set; }
        public string UserId { get; set; }
        public int Order { get; set; }
        public string Statement { get; set; }
        public string Comment { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; }
    }
}
