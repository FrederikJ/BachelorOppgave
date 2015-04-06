using HovedOppgave.Models;
using HovedOppgave.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HovedOppgave.Controllers
{
    public class GlobalViewsController : Controller
    {
        // GET: GlobalViews
        public ActionResult UnitDetails(Device device)
        {
            return View();
        }

        // GET: GlobalViews
        public ActionResult UnitHistory(Device device)
        {
            return View();
        }
    }
}