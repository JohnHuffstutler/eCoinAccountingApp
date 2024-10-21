using System.ComponentModel.DataAnnotations;

namespace eCoinAccountingApp.Models
{
    public class Journal
    {
        [Key]
        public int journalNum { get; set; }
        public DateTime DateAdded { get; set; }
        public string AccountName { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public Account Account { get; set; }

    }
}
