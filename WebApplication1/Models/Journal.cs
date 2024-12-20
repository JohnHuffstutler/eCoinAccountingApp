﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCoinAccountingApp.Models
{
    public class Journal
    {
        [Key]
        public int JournalNum { get; set; }

        [Required]
        public DateTime DateAdded { get; set; }

        [Required]
        [ForeignKey("Account")]
        public int AccountId { get; set; }

        public Account Account { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Debit { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Credit { get; set; }

        public string? Description { get; set; }

        public string Status { get; set; }

        public string? DocumentPath { get; set; } // To store the file path
    }
}
