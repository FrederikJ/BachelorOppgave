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

        public static bool IsDateTime(string date)
        {
            DateTime result;
            bool convert = DateTime.TryParse(date, out result);
            return convert;
        }

        public static bool CheckRights(int UserID, Constant.Rights rights)
        {
            UserRight user = myrep.GetUserWithRights(UserID, rights);
            
            if (user == null)
                return false;
            else
                return true;
        }

        //Forfatter: Frederik Johnsen
        public static string TextValditor(string input)
        {
            if (input == null)
                return "Vennligst fyll inn feltet";

            if (Regex.IsMatch(input,"^[0-9a-åA-Å'.\t\n\f\t]$"))
                return null;
            else
                return "Du har skrevet inn ugyldige tegn";
        }

        public static bool IsValidFile(HttpPostedFileBase file, double maxFileSize)
        {
            HttpContext http = HttpContext.Current;
            if(file == null)
            {
                http.Session["flashMessage"] = "Filen eksisterer ikke";
                http.Session["flashStatus"] = Constant.NotificationType.danger.ToString();
                return false;
            }

            //parametere man setter for max fil størrelse i MB. 
            var max = maxFileSize * 1024 * 1024;

            if (file.ContentLength > max)
            {
                http.Session["flashMessage"] = "Filstørrelse er for stor";
                http.Session["flashStatus"] = Constant.NotificationType.danger.ToString();
                return false;
            }

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