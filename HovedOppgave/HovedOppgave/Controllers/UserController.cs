using HovedOppgave.Classes;
using HovedOppgave.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HovedOppgave.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult List()
        {
            User method = new User("Frederik Johnsen", "Heia@hin.no", 1);
            User method1 = new User("Kjartan Horpestad", "Ohla@hin.no", 2);
            User method2 = new User("Vladimir Putin", "Dracula@hin.no", 3);

            List<User> list = new List<User>();
            list.Add(method);
            list.Add(method1);
            list.Add(method2);
            return View(list);
        }

        public ActionResult ListUsers()
        {
            User method = new User("Frederik Johnsen", "Heia@hin.no", 1);
            User method1 = new User("Kjartan Horpestad", "Ohla@hin.no", 2);
            User method2 = new User("Vladimir Putin", "Dracula@hin.no", 3);

            List<User> list = new List<User>();
            list.Add(method);
            list.Add(method1);
            list.Add(method2);
            return View(list);
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(User user)
        {
            try
            {
                User method = new User(user);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
