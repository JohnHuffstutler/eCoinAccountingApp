using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eCoinAccountingApp.Models
{
    public class Journal
    {
        [Key]
        public int JournalEntryId { get; set; }

        [Required]
        public DateTime DateAdded { get; set; }

        public string Description { get; set; }

        [Required]
        public string Status { get; set; }

        public string DocumentPath { get; set; }

        public virtual ICollection<JournalTransaction> Transactions { get; set; } = new List<JournalTransaction>();
    }
}
