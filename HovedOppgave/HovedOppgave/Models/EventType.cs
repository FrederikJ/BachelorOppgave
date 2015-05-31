using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/**
 * Model klasse for log event typer
*/

namespace HovedOppgave.Models
{
    public class EventType
    {
        public int EventTypeID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}