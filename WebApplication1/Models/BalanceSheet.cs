namespace eCoinAccountingApp.Models
{
    public class BalanceSheet
    {
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountCategory { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
