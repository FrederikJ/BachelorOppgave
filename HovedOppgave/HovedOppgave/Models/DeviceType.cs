using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/**
 * Model klasse for enhet typer
*/

namespace HovedOppgave.Models
{
    public class DeviceType
    {
        public int DeviceTypeID { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        public virtual int CanCalibrateID { get; set; }
    }
}