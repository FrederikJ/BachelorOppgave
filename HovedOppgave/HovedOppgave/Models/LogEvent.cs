using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Models
{
    public class LogEvent
    {
        public int LogEventID { get; set; }
        public DateTime RegisteredDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DataFilePath { get; set; }
        public string Data1 { get; set; }
        public string Data2 { get; set; }
        
        //Foreignkeys
        public virtual int EventTypeID { get; set; }
        public virtual int DeviceID { get; set; }
        public virtual int RoomID { get; set; }
        public virtual int ContactID { get; set; }
    }
}