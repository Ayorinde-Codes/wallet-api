using System;
using System.ComponentModel.DataAnnotations;

namespace Numero.Model
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Your Email is Required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Your Password is Required.")]
        public string Password { get; set; }
    }
}
