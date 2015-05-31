using HovedOppgave.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace HovedOppgave.Classes
{
    /// <summary>
    /// Forskjellige validatorer som sjekker at ikke noe ulovlig blir
    /// skrevet inn i inputfeltene i prosjeket
    /// 
    /// Gjeste forfatter: Eivind Skreddernes og Dag-Andre Ivarsøy
    /// </summary>

    public class Validator
    {
        static IRepository myrep = new Repository();

        /**
         * konventerer en string til tall 
        */
        public static int ConvertToNumbers(string tekst)
        {
            string onlyNumber = Regex.Replace(tekst, @"\D", "");
            int result;
            bool convert = int.TryParse(onlyNumber, out result);
            if (convert)
                return result;
            else 
                return -1;
        }

        /**
         * sjekker om en string er en dato 
        */
        public static bool IsDateTime(string date)
        {
            DateTime result;
            bool convert = DateTime.TryParse(date, out result);
            return convert;
        }

        /**
         * sjekker rettigheten til innlogget bruker 
        */
        public static bool CheckRights(Constant.Rights rights)
        {
            HttpContext http = HttpContext.Current;
            int UserID = Validator.ConvertToNumbers(http.Session["UserID"].ToString());
            UserRight model = myrep.GetUserWithRights(UserID, rights);
            
            if (model.Right == null || model.User == null)
                return false;
            else
                return true;
        }

        /**
         * sjekker en brukers rettighet 
        */
        public static bool CheckRights(int UserID, Constant.Rights rights)
        {
            UserRight model = myrep.GetUserWithRights(UserID, rights);

            if (model.Right == null || model.User == null)
                return false;
            else
                return true;
        }

        /**
         * sjekker om en email faktisk er en email
         * forfatter: frederik
        */
        public static bool ValidateEmail(string input)
        {
            //Hentet regex uttrykket her i fra http://stackoverflow.com/questions/5342375/c-sharp-regex-email-validation
            if (input != "" && Regex.IsMatch(input, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                return true;
            
            return false;
        }

        /**
         * sjekker om en fil er gyldig 
        */
        public static bool IsValidFile(HttpPostedFileBase file, double maxFileSize)
        {
            //om filen eksisterer
            HttpContext http = HttpContext.Current;
            if(file == null)
            {
                http.Session["flashMessage"] = "Filen eksisterer ikke";
                http.Session["flashStatus"] = Constant.NotificationType.danger.ToString();
                return false;
            }

            //parametere man setter for max fil størrelse i MB. 
            var max = maxFileSize * 1024 * 1024;

            //om den er liten nok
            if (file.ContentLength > max)
            {
                http.Session["flashMessage"] = "Filstørrelse er for stor";
                http.Session["flashStatus"] = Constant.NotificationType.danger.ToString();
                return false;
            }

            //og formatet på filen
            var format = Path.GetExtension(file.FileName);

            var validExtensions = new [] { ".csv", ".pdf" };

            if (!validExtensions.Contains(format.ToLower()))
            {
                http.Session["flashMessage"] = "Filformatet er ikke gyldig";
                http.Session["flashStatus"] = Constant.NotificationType.danger.ToString();
                return false;
            }  
            else
                return true;
        }
    }
}