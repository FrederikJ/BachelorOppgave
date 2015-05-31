using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/**
 * Model klasse for post kode til bygning adresse
*/

namespace HovedOppgave.Models
{
    public class PostCode
    {
        public int PostCodeID { get; set; }
        public string City { get; set; }
    }
}