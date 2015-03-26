﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Models
{
    public class Contact
    {
        public int ContactID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }

        //Foreignkeys
        public virtual int CompanyID { get; set; }
    }
}