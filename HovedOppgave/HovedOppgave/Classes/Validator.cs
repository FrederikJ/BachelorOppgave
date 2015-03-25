using HovedOppgave.Models;
using System;
using System.Collections.Generic;
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

        public static bool CheckRights(int UserID, Constants.Rights rights)
        {
            IRepository queries = new Repository();
            User user = queries.GetUserWithRights(UserID, rights);
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
    }
}