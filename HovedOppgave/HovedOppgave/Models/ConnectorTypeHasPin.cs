using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Models
{
    public class ConnectorTypeHasPin
    {
        public int PinNrID { get; set; }
        
        //Foreignkeys
        public virtual int ConnectorTypeID { get; set; }
        public virtual int PinID { get; set; }
    }
}