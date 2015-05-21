using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/**
 * Model klasse for kotakt personer i et firma
*/

namespace HovedOppgave.Models
{
    public class Contact
    {
        public int ContactID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }

        //Foreignkeys
        public virtual int CompanyID { get; set; }
    }
}