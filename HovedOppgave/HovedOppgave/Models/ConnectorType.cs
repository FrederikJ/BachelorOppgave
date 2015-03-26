using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Models
{
    public class ConnectorType
    {
        public int ConnectorTypeID { get; set; }
        public string Type { get; set; }
        public int Pins { get; set; }
        public Enum Signal { get; set; }
        public Enum Gender { get; set; }
        public Enum DefaultPinNames { get; set; }
        public bool Shielded { get; set; }
    }
}