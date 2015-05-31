using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/**
 * Model klasse for kabler
*/

namespace HovedOppgave.Models
{
    public class Cable
    {
        public int CableID { get; set; }
        public int Length { get; set; }
        public string Note { get; set; }

        //Foreignkey
        public virtual int CableTypeID { get; set; }
    }
}