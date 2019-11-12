using System;
using System.ComponentModel.DataAnnotations;

namespace Numero.Model
{
    public class ResendOtp
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
