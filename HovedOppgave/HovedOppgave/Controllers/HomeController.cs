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
        public ActionResult Index()
        {
            string master = SessionCheck.FindMaster();
            return View("Index", master);
        }

        public ActionResult Contact()
        {
            string master = "~/Views/Shared/_LoggedOut.cshtml";
            if(Session["UserID"] != null)
                master = SessionCheck.FindMaster();
            return View("Contact", master);
        }
    }
}