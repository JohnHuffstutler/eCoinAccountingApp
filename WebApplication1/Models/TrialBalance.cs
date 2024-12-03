namespace eCoinAccountingApp.Models
{
    public class TrialBalance
    {
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
    }
}
