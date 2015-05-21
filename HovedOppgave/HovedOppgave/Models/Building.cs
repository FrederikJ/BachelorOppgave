using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/**
 * Model klasse for bygninger
*/
namespace HovedOppgave.Models
{
    public class Building
    {
        public int BuildingID { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //Foreignkeys
        public virtual int CompanyID { get; set; }
        public virtual int PostcodeID { get; set; }
    }
}