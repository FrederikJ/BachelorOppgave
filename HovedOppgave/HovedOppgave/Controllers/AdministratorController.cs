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
    public class AdministratorController : Controller
    {
        IRepository myrep = new Repository();

        public AdministratorController()
        {
            //SessionCheck.CheckForRightsOnLogInUser(Constant.Rights.Administrator);
        }

        // GET: Administrator
        public ActionResult OverViewUsers()
        {
            List<User> users = myrep.GetAllUsers();
            List<Rights> rights = myrep.GetAllRights();
            AdminViews model = new AdminViews();
            model.Users = users;
            model.Rights = rights;

            //var view = View(model);
            //view.MasterName = SessionCheck.FindMaster();
            return View(model);
        }

        // GET: Administrator/CheckNewUsers/5
        public ActionResult CheckNewUsers()
        {
            List<User> users = myrep.GetAllUsersUnchecked();
            List<Rights> rights = myrep.GetAllRights();
            AdminViews model = new AdminViews();
            Rights right = new Rights()
            {
                RightsID = 3,
                Name = "Guest"
            };
            model.Users = users;
            model.Rights = rights;
            model.Right = right;

            //var view = View(model);
            //view.MasterName = SessionCheck.FindMaster();
            return View(model);
        }

        // POST: Administrator/CheckUser/5
        [HttpPost]
        public ActionResult CheckUser(int userId, int rightId)
        {
            User user = myrep.GetUser(userId);
            Rights right = myrep.GetRightToUser(user);
            user.RightsID = rightId;
            user.Checked = true;
            if (myrep.EditUser(user))
            {
                string message = "Administratoren har gitt deg rettigheten " + right.Name;
                SendEmail mail = new SendEmail();
                mail.SendEpost(user.Email, message, "Ny rettighet hos oss");
            }
            return RedirectToAction("CheckNewUsers");
        }

        // GET: Administrator/DeleteDeleteDiscarded/5
        public ActionResult DeleteDiscarded(int id)
        {
            return View();
        }

        // POST: Administrator/DeleteDeleteDiscarded/5
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
