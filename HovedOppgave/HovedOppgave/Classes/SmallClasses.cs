using HovedOppgave.Models;
using HovedOppgave.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace HovedOppgave.Classes
{
    public class SmallClasses
    {
        static IRepository myrep = new Repository();

        /**
         * oppretter et random passord med en gitt lengde 
        */
        public static string CreatePassword(int length)
        {
            string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string res = "";
            Random rnd = new Random();
            while (0 < length--)
                res += valid[rnd.Next(valid.Length)];
            return res;
        }

        /**
         * oppdaterer passordet til en bruker 
        */
        public static bool UpdatePassword(User user, string password)
        {
            if (user != null)
            {
                Hashtable table = Hash.GetHashAndSalt(password);
                user.PassHash = (string)table["hash"];
                user.PassSalt = (string)table["salt"];
                if (myrep.EditUser(user))
                    return true;
            }
            return false;
        }

        /**
         * endrer passordet til den innloggede brukeren 
        */
        public static bool ChangePassword(string password)
        {
            HttpContext http = HttpContext.Current;
            User user = myrep.GetUser(Validator.ConvertToNumbers(http.Session["UserID"].ToString()));
            Hashtable table = Hash.GetHashAndSalt(password);
            user.PassHash = (string)table["hash"];
            user.PassSalt = (string)table["salt"];
            if (myrep.EditUser(user))
                return true;
            else
                return false;
        }

        /**
         * sjekker om den innloggede brukere har et passord 
        */
        public static bool HasPassword()
        {
            HttpContext http = HttpContext.Current;
            User user = myrep.GetUser(Validator.ConvertToNumbers(http.Session["UserID"].ToString()));
            if (user.PassHash != null)
                return true;
            else
                return false;
        }

        /**
         * setter session objektene for innloggede bruker og finner master pagen 
        */
        public static void LoggingIn(User loggedIn)
        {
            HttpContext http = HttpContext.Current;
            //Denne som vil autentisere brukeren
            http.Session["UserID"] = loggedIn.UserId;
            http.Session["User"] = loggedIn;
            http.Session["Name"] = loggedIn.Name;
            //lagre authenticated i en session så den er tigjengelig i hele prosjektet
            http.Session["LoggedIn"] = true;
            
            // Sjekker rettigheter og sender brukeren videre til riktig hovedside
            if (Validator.CheckRights(loggedIn.UserId, Constant.Rights.Administrator))
                http.Session["Rights"] = Constant.Rights.Administrator.ToString();
            else if (Validator.CheckRights(loggedIn.UserId, Constant.Rights.User))
                http.Session["Rights"] = Constant.Rights.User.ToString();
            else if (Validator.CheckRights(loggedIn.UserId, Constant.Rights.Guest))
                http.Session["Rights"] = Constant.Rights.Guest.ToString();
        }

        /**
         * sletter alle session objekt ved utlogging 
        */
        public static void DeleteSessions()
        {
            HttpContext http = HttpContext.Current;
            http.Session["UserID"] = null;
            http.Session["User"] = null;
            http.Session["Name"] = null;
            http.Session["LoggedIn"] = null;
            http.Session["Rights"] = null;
        }
    }
}