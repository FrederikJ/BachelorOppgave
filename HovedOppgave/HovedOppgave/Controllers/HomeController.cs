﻿using HovedOppgave.Classes;
using HovedOppgave.Models;
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
            var view = View();
            SessionCheck.FindMaster();
            //view.MasterNam e= "_GuestLayout";
            return view;
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
            User method = new User("Frederik Johnsen", "Heia@hin.no");
            User method1 = new User("Kjartan Horpestad", "Ohla@hin.no");
            User method2 = new User("Vladimir Putin", "Dracula@hin.no");

            List<User> list = new List<User>();
            list.Add(method);
            list.Add(method1);
            list.Add(method2);
            return View(list);
        }

        public ActionResult SearchEngine(string searchString)
        {
            return null;
        }
    }
}