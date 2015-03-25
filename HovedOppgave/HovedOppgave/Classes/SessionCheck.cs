using HovedOppgave.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Classes
{
    /// <summary>
    /// Forskjellige sjekk på rettigheter til innlogget bruker så ingen
    /// er på siden dem ikke skal være
    /// 
    /// Gjeste forfatter: Dag-Andre Ivasøy
    /// </summary>
    
    public class SessionCheck : System.Web.HttpApplication
    {
        public static void CheckForUserID()
        {
            HttpContext http = HttpContext.Current;
            if (http.Session["UserID"] == null)
            {
                http.Session["flashMessage"] = "Du må logge deg inn";
                http.Session["flashStatus"] = Constants.NotificationType.danger.ToString();
                http.Response.Redirect("~/login", true);
            }
        }

        public static void CheckForRightsOnLogInUser(Constants.Rights rettighet)
        {
            HttpContext http = HttpContext.Current;
            CheckForUserID();
            int UserID = Validator.ConvertToNumbers(http.Session["UserID"].ToString());

            if (!Validator.CheckRights(UserID, rettighet))
            {
                http.Session["flashMessage"] = "Du har ikke korrekte rettighet for aksessere siden du prøvde å nå";
                http.Session["flashStatus"] = Constants.NotificationType.danger.ToString();
                http.Response.Redirect(("~/Loggut"), true);
            }
        }

        //sjekker hvilken masterpage den skal velge etter hvilken rettighet man har
        public static string FindMaster()
        {
            IRepository queries = new Repository();
            HttpContext http = HttpContext.Current;
            CheckForUserID();
            int UserID = Validator.ConvertToNumbers(http.Session["UserID"].ToString());
            Rights rights = queries.GetRights(UserID);

            string master = "";


            if (rights.RightsName == Constants.Rights.Administrator.ToString())
                master = "~/Views/Shared/_AdminLayout";
            else if (rights.RightsName == Constants.Rights.User.ToString())
                master = "~/Views/Shared/_UserLayout";
            else if (rights.RightsName == Constants.Rights.Guest.ToString())
                master = "~/Views/Shared/_GuestLayout";

            return master;
        }

        public static void LogOutWrongRights()
        {
            HttpContext http = HttpContext.Current;
            http.Session["UserID"] = null;
            http.Session["User"] = null;
            http.Session["FirstName"] = null;
            http.Session["UserName"] = null;
            http.Session["loggedIn"] = null;
            http.Session["flashMessage"] = "Du har ikke korrekte rettighet for aksessere siden du prøvde å nå";
            http.Session["flashStatus"] = Constants.NotificationType.danger.ToString();
            http.Response.Redirect(("~/Login.aspx"), true);
        }
    }
}