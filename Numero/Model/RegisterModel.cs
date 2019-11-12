﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Numero.Model
{
    public class RegisterModel
    {
        [Required]
        public string FirstName  { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
