using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/**
 * Model klasse for firmaer
*/

namespace HovedOppgave.Models
{
    public class Company
    {
        public int CompanyID { get; set; }
        public string Name { get; set; }
        public int OrganisationNum { get; set; }
    }
}