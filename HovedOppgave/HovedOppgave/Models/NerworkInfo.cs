using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;

/**
 * Model klasse for netwerk informasjon
*/

namespace HovedOppgave.Models
{
    public class NerworkInfo
    {
        public string MAC { get; set; }
        public string IP { get; set; } //byte[]
        public string Subnet { get; set; }
        public string NetworkInfocol { get; set; }

        //Foreignkeys
        [Key]
        public virtual int DeviceID { get; set; }
    }
}