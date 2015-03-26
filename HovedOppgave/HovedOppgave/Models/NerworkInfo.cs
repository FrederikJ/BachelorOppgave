using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HovedOppgave.Models
{
    public class NerworkInfo
    {
        public byte MAC { get; set; }
        public byte IP { get; set; }
        public byte Subnet { get; set; }
        public string NetworkInfocol { get; set; }

        //Foreignkeys
        [Key]
        public virtual int DeviceID { get; set; }
    }
}