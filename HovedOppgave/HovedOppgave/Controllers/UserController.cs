using HovedOppgave.Classes;
using HovedOppgave.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HovedOppgave.Controllers
{
    public class UserController : Controller
    {
        IRepository myrep = new Repository();
        // GET: User
        public ActionResult List()
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

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: User/Create
        public ActionResult Create()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach(Rights item in myrep.GetAllRights())
            {
                SelectListItem selectlist = new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.RightsID.ToString()
                };
                list.Add(selectlist);
            }

            CreatUserViewModel model = new CreatUserViewModel()
            {
                Rights = list
            };

            return View(model);
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(CreatUserViewModel user, IEnumerable<string> SelectedRight)
        {
            List<Rights> list = myrep.GetAllRights();
            User createUser = new User();

            if(user.Password.Equals(user.ConfirmPassword))
            {
                Hashtable table = Hash.GetHashAndSalt(user.Password);
                createUser.PassHash = (string)table["hash"];
                createUser.PassSalt = (string)table["salt"];
                createUser.Name = user.Name;
                createUser.Email = user.Email;

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Equals(SelectedRight))
                        createUser.RightsID = list[i].RightsID;
                }
            }

            try
            {
                myrep.CreateUser(createUser);
                return RedirectToAction("List");
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

                return RedirectToAction("List");
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

                return RedirectToAction("List");
            }
            catch
            {
                return View();
            }
        }
    }
}
