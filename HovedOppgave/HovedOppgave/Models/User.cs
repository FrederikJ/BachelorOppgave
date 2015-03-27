﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public int AccessLevel { get; set; }
        public string Email { get; set; }
        public string PassHash { get; set; }
        public string PassSalt { get; set; }

        public User(User user)
        {
            this.Name = user.Name;
            this.AccessLevel = user.AccessLevel;
            this.Email = user.Email;
            this.PassHash = user.PassHash;
            this.PassSalt = user.PassSalt;
        }

        public User(string name, string email, int accessLevel)
        {
            this.Name = name;
            this.AccessLevel = accessLevel;
            this.Email = email;
        }
    }
}