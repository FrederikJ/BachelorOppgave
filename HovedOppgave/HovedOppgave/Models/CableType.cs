using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Models
{
    public class CableType
    {
        public int CableTypeID { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
        public decimal Impdance { get; set; }
        public int Strands { get; set; }
        public bool Shielded { get; set; }
        public int MaxFrequency { get; set; }
        public bool Optical { get; set; }
    }
}