using HovedOppgave.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HovedOppgave.Models
{
    interface IRepository
    {
        Rights GetRights(int userID);
        User GetUserWithRights(int userID, Constants.Rights rights);
    }
}
