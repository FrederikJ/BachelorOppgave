using HovedOppgave.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Classes
{
    public class CreatRandomPassword
    {
        public static string CreatePassword(int length)
        {
            string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string res = "";
            Random rnd = new Random();
            while (0 < length--)
                res += valid[rnd.Next(valid.Length)];
            return res;
        }
    }

    public class UpdatePassword
    {
        public static bool updatePassword(string email, string password)
        {
            if (email != null)
            {
                using (var context = new Context())
                {
                    User user = context.Brukere.Where(b => b.Epost == email).FirstOrDefault();
                    Hashtable table = Hash.GetHashAndSalt(password);
                    user.Password = table["hash"].ToString();
                    user.Salt = table["salt"].ToString();

                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }
    }
}