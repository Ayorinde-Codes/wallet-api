using System;
using System.ComponentModel.DataAnnotations;

namespace Numero.Model
{
    public class BankVerifyModel
    {
        [Required]
        public string AccountNumber { get; set; }

        [Required]
        public string BankCode { get; set; }
    }
}
