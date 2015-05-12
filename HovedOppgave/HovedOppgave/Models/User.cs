using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PassHash { get; set; }
        public string PassSalt { get; set; }
        public bool Checked { get; set; }

        //ForeignKey
        public virtual int RightsID { get; set; }
    }
}