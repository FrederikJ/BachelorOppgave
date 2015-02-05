using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HovedOppgave.Models;

namespace HovedOppgave.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddUser(User bruker)
        {
            User method = new User(bruker);
            return View(bruker);
        }

        public ActionResult ListUsers()
        {
            User method = new User("Frederik", "Johnsen", "heia@hin.no", "48864032", "heiaveien 34", "8515", "Narvik");
            User method1 = new User("Kjartan", "Horpestad", "hade@hin.no", "45349532", "hadeveien 24", "8516", "Narvik");

            List<User> list = new List<User>();
            list.Add(method);
            list.Add(method1);
            return View(list);
        }
    }
}