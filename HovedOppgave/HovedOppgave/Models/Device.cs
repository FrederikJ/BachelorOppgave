using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Models
{
    public class Device
    {
        public int DeviceID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SerialNum { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public bool IsRackMountable { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public decimal InputVoltage { get; set; }

        //Foreignkeys
        public virtual int DeviceTypeID { get; set; }
        public virtual int RoomID { get; set; }
    }
}