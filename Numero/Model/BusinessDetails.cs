using System;
using System.ComponentModel.DataAnnotations;
using Numero.Data;
using Numero.Model;

namespace Numero.Model
{
    public class BusinessDetails : BaseEntity
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Required]
        public string BusinessSector { get; set; }

        [Required]
        public string RcNumber { get; set; }

        public string Email { get; set; }

        [Required]
        [EmailAddress]
        public string BusinessEmail { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string BusinesssName { get; set; }

        [Required]
        public int EmployeeNumber { get; set; }

        [Required]
        public int DirectorsNumber { get; set; }

    }

}
