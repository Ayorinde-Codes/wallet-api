using System;
using System.ComponentModel.DataAnnotations;

namespace Numero.Model
{
    public class BusinessDetailsUpdate
    {
        public string BusinessSector { get; set; }

        public string RcNumber { get; set; }


        public string BusinessEmail { get; set; }

        public string Address { get; set; }

        public string BusinesssName { get; set; }

        public int EmployeeNumber { get; set; }

        public int DirectorsNumber { get; set; }
    }
}
