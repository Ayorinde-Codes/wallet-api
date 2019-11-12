using System;
using System.ComponentModel.DataAnnotations;

namespace Numero.Model
{
    public class ChangePasswordBindingModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        //[Display(Name = "New password")]
      
    }
}
