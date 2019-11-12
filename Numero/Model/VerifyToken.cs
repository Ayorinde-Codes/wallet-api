using System;
using System.ComponentModel.DataAnnotations;

namespace Numero.Model
{
    public class VerifyToken
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
