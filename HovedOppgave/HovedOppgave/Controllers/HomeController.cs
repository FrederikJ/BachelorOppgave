using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HovedOppgave.Models;
using System.Data;

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
            User method = new User("Frederik", "Johnsen", "Heia@hin.no", "48864032", "Heiaveien 34", "8515", "Narvik");
            User method1 = new User("Kjartan", "Horpestad", "Ohla@hin.no", "65349532", "Ohlaveien 24", "8516", "Larvik");
            User method2 = new User("Vladimir", "Putin", "Dracula@hin.no", "58368532", "Draculaveien 24", "8514", "Karvik");

            List<User> list = new List<User>();
            list.Add(method);
            list.Add(method1);
            list.Add(method2);
            return View(list);
        }

        public ActionResult SearchEngine(string searchString)
        {
            User user = new User();
            if(searchString.Equals(user.FirstName))
                return null;

            return null;
        }
    }
}