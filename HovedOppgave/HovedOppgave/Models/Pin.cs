using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/**
 * Model klasse for pinner til en konnektor
*/

namespace HovedOppgave.Models
{
    public class Pin
    {
        public int PinID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Enum Direction { get; set; }
        public Enum Signal { get; set; }
        public Enum OpticalMode { get; set; }
        public Enum Frequency { get; set; }
        public decimal Impedance { get; set; }
        public decimal MaxVolt { get; set; }
        public decimal MinVolt { get; set; }

        //Foreignkey
        public virtual int SignalStandardID { get; set; }
    }
}