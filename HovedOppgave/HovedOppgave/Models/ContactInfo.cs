using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/**
 * Model klasse for kontakt info
*/

namespace HovedOppgave.Models
{
    public class ContactInfo
    {
        public int ContactInfoID { get; set; }
        public string Value { get; set; }

        //Foreignkeys
        public virtual int ContactInfoTypeID { get; set; }
        public virtual int ContactID { get; set; }
    }
}