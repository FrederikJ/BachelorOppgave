using HovedOppgave.Classes;
using HovedOppgave.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace HovedOppgave.Controllers
{
    public class HomeController : Controller
    {
        SessionCheck check = new SessionCheck();

        /**
         * front siden 
        */
        public ActionResult Index()
        {
            //string master = check.FindMaster();
            return View();//"Index", master);
        }

        /**
         * kotakt info til narom 
        */
        public ActionResult Contact()
        {
            string master = "~/Views/Shared/_LoggedOut.cshtml";
            if(Session["UserID"] != null)
                master = check.FindMaster();
            return View("Contact", master);
        }
    }
}