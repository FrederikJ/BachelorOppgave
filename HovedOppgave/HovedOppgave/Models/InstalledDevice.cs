using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Models
{
    public class InstalledDevice
    {
        public int InstalledDeviceID { get; set; }
        public int PositionFromTop { get; set; }
        public bool FacingFront { get; set; }
        public string InstalledLabel { get; set; }

        //Foreignkeys
        public virtual int DeviceID { get; set; }
        public virtual int MountID { get; set; }
    }
}