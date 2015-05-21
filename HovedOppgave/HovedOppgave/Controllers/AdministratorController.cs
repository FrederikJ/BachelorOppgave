using HovedOppgave.Classes;
using HovedOppgave.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;

namespace HovedOppgave.Controllers
{
    public class AdministratorController : Controller
    {
        IRepository myrep = new Repository();
        SessionCheck sessionCheck = new SessionCheck();
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

            string master = sessionCheck.FindMaster();
            return View("OverViewUsers", master, model);
        }

        // GET: Administrator/CheckNewUsers/5
        public ActionResult CheckNewUsers()
        {
            List<User> users = myrep.GetAllUsersUnchecked();
            List<Rights> rights = myrep.GetAllRights();
            AdminViews model = new AdminViews();
            Rights right = new Rights() {
                RightsID = 3,
                Name = "Guest"
            };
            model.Users = users;
            model.Rights = rights;
            model.Right = right;

            string master = sessionCheck.FindMaster();
            return View("CheckNewUsers", master, model);
        }

        // POST: Administrator/CheckUser/5
        [HttpPost]
        public JsonResult CheckUser(User user)
        {
            User userDb = myrep.GetUser(user.UserId);
            userDb.RightsID = user.RightsID;
            userDb.Checked = true;
            Rights right = myrep.GetRightToUser(userDb);
            if (myrep.EditUser(userDb))
            {
                string message = "Administratoren har gitt deg rettigheten " + right.Name;
                SendEmail mail = new SendEmail();
                mail.SendEpost(userDb.Email, message, "Ny rettighet hos oss");
                return Json(true);
            }
            return Json(false);
        }

        // GET: Administrator/DeleteDeleteDiscarded/5
        public ActionResult DeleteDiscarded()
        {
            CalibrationViews model = new CalibrationViews();
            model.ExtraStringHelp = Url.Content("~/Sertifikat");
            model.Files = myrep.GetAllDiscardedFiles();
            List<LogEvent> list = myrep.GetAllLogEventToEventType(12);
            model.JoinQuery = new List<JoinLogEventWithNames>();
            foreach(var item in list)
            {
                model.JoinQuery.Add(myrep.JoinQuery(item));
            }
            //string master = SessionCheck.FindMaster();
            return View(/*"DeleteDiscarded", master,*/ model);
        }

        // POST: Administrator/DeleteDeleteDiscardedFile/5
        [HttpPost]
        public JsonResult DeleteDiscardedFile(Files file)
        {
            Files fileDB = myrep.GetFile(file.FileID);
            if (myrep.DeleteFile(fileDB))
            {
                DeleteFileFromDirectory(fileDB);
                return Json(true);
            }
            else
                return Json(false);
        }

        [HttpPost]
        public JsonResult DeleteDiscardedEvent(LogEvent logEvent)
        {
            logEvent = myrep.GetLogEvent(logEvent.LogEventID);
            if (myrep.DeleteLogEvent(logEvent))
                return Json(true);
            else
                return Json(false);
        }

        public void DeleteFileFromDirectory(Files file)
        {
            //Går igjennom alle filer vi har, sjekker om det er flere med samme fil navn
            List<Files> files = myrep.GetAllFiles();
            var tempList = new List<Files>();
            for (int i = 0; i < files.Count; i++)
                if (files[i].FileName.Equals(file.FileName))
                    tempList.Add(files[i]);

            //count vil være 1 vis det ikke er flere med samme navn så man kan slette filen
            // fra directory. for når man lagre filen, så lagre den ikke om det er en fil med samme navnet fra før av.
            if (tempList.Count == 1)
            {
                DirectoryInfo myDir = new DirectoryInfo(Server.MapPath("~/Sertifikat"));
                foreach (FileInfo fil in myDir.GetFiles())
                    if (fil.Name.Equals(file.FileName))
                        fil.Delete();
            }
        }
    }
}
