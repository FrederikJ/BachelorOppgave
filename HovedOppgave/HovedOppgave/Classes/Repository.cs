using HovedOppgave.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Models
{
    public class Repository : IRepository
    {
        public Rights GetRights(int userID)
        {
            return null;
        }
        public User GetUserWithRights(int userID, Constants.Rights rights)
        {
            return null;
        }
    }
}