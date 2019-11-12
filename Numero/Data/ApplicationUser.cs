using System;
using Microsoft.AspNetCore.Identity;

namespace Numero.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }
        public string VerificationToken { get; internal set; }
    }
}
