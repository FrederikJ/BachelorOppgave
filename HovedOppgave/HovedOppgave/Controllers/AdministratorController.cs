using HovedOppgave.Classes;
using HovedOppgave.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;

/**
 * Alle sidene for administrator, som han kan vedlike holde systemet. oversikt over alt
 * 
 * Forfatter: Frederik Johnsen
*/
namespace HovedOppgave.Controllers
{
    public class AdministratorController : Controller
    {
        IRepository myrep = new Repository();
        SessionCheck sessionCheck = new SessionCheck();

        /**
         * Sjekker rettigheten til brukeren
        */
        public AdministratorController()
        {
            //SessionCheck.CheckForRightsOnLogInUser(Constant.Rights.Administrator);
        }

        /**
         * Bruker oversikt til alle brukerene i hele systemet. Kan endre på dem som han vil
        */
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

        /**
         * En ny bruker har registrert seg på systemet, så admin må sette rettigheten på brukeren
         * default rettighet som har blitt satt er gjest
        */
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

        /**
         * får inn bruker id, henter brukeren, setter rettighet på brukeren, sjekker han ut av denne
         * delen av systemet, oppdatere brukeren og sende email til brukeren om rettigheten han har fått
         * når man kommer tilbake til viewet, så slettes brukeren fra den eksisterende listen så man
         * har den oppdatert
        */
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

        /**
         * får inn alle filer og event som har blitt slettet av bruker(kassert) i systemet
         * så kan admin slette dem fra db viss han vil det
        */
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

        /**
         *  sletter filen fra db og fra directory
        */
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

        /**
         * sletter event fra db 
        */
        [HttpPost]
        public JsonResult DeleteDiscardedEvent(LogEvent logEvent)
        {
            logEvent = myrep.GetLogEvent(logEvent.LogEventID);
            if (myrep.DeleteLogEvent(logEvent))
                return Json(true);
            else
                return Json(false);
        }

        /**
         * sletter filer fra directory
        */
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
