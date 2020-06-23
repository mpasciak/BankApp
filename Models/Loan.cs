using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BankApp.Models
{
    public class Loan
    {
        [Key]
        public int Id { get; set; }
        public int OwnerUserId { get; set; }
        public decimal ToPay { get; set; }

    }
}
