using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/**
 * Model klasse for tilkoblinger
*/

namespace HovedOppgave.Models
{
    public class Connection
    {
        public int ConnectionID { get; set; }
        public int PinNumCon1 { get; set; }
        public int PinNumCon2 { get; set; }
        public int StrandNum { get; set; }

        //Foreignkeys
        public virtual int ConnectorID1 { get; set; }
        public virtual int ConnectorID2 { get; set; }
        public virtual int CableID { get; set; }
    }
}