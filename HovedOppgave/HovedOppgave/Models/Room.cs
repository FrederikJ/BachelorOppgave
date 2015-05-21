using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/**
 * Model klasse for rom i bygninger
*/

namespace HovedOppgave.Models
{
    public class Room
    {
        public int RoomID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //Foreignkeys
        public virtual int BuildingID { get; set; }
    }
}