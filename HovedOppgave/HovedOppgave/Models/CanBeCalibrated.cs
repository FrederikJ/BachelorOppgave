using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/**
 * Model klasse for device som kan kalibreres
*/

namespace HovedOppgave.Models
{
    public class CanBeCalibrated
    {
        public int CanCalibrateID { get; set; }
        public string Description { get; set; }
    }
}