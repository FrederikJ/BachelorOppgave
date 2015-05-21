using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

/**
 * Model klasse for konnektor typer som har pinner (mellomledd)
*/

namespace HovedOppgave.Models
{
    public class ConnectorTypeHasPin
    {
        public int PinNr { get; set; }
        
        //Foreignkeys
        public virtual int ConnectorTypeID { get; set; }
        [Key]
        public virtual int PinID { get; set; }
    }
}