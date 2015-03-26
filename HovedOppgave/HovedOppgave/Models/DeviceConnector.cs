using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Models
{
    public class DeviceConnector
    {
        public int DeviceConnectorID { get; set; }

        //ForeignKeys
        public virtual int DeviceID { get; set; }
        public virtual int ConnectorTypeID { get; set; }
    }
}