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
        static IRepository myrep = new Repository();

        /**
         * sjekker om en bruker er innlogget med bruker id 
        */
        public static void CheckForUserID()
        {
            HttpContext http = HttpContext.Current;
            if (http.Session["UserID"] == null || http.Session.Keys.Count == 0)
            {
                http.Session["flashMessage"] = "Du må logge deg inn";
                http.Session["flashStatus"] = Constant.NotificationType.danger.ToString();
                http.Response.Redirect("~/Account/LogOff", true);
            }
        }

        /**
         * sjekker rettighet til innlogget bruker 
        */
        public static void CheckForRightsOnLogInUser(Constant.Rights rettighet)
        {
            CheckForUserID();

            if (!Validator.CheckRights(rettighet))
            {
                HttpContext http = HttpContext.Current;
                http.Session["flashMessage"] = "Du har ikke korrekte rettighet for aksessere siden du prøvde å nå";
                http.Session["flashStatus"] = Constant.NotificationType.danger.ToString();
                http.Response.Redirect("~/Account/LogOff", true);
            }
        }

        //sjekker hvilken masterpage den skal velge etter hvilken rettighet man har
        public static string FindMaster()
        {
            HttpContext http = HttpContext.Current;
            
            string master = "";
            CheckForUserID();
            int userId = Validator.ConvertToNumbers(http.Session["UserID"].ToString());
            User user = myrep.GetUser(userId);
            Rights rights = myrep.GetRightToUser(user);

            if (rights.Name == Constant.Rights.Administrator.ToString())
                master = "~/Views/Shared/_AdminLayout.cshtml";
            else if (rights.Name == Constant.Rights.User.ToString())
                master = "~/Views/Shared/_UserLayout.cshtml";
            else if (rights.Name == Constant.Rights.Guest.ToString())
                master = "~/Views/Shared/_GuestLayout.cshtml";

            return master;
        }
    }
}