using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCoinAccountingApp.Models
{
    public class JournalTransaction
    {
        [Key]
        public int JournalTransactionId { get; set; }

        [Required]
        [ForeignKey("JournalEntry")]
        public int JournalEntryId { get; set; }

        public Journal JournalEntry{ get; set; }

        [Required]
        [ForeignKey("Account")]
        public int AccountId { get; set; }

        public Account Account { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Debit { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Credit { get; set; }

        public string Description { get; set; }
    }
}
