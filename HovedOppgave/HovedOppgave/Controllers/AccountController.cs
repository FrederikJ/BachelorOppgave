using HovedOppgave.Classes;
using HovedOppgave.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Net.Mail;
using System.Collections;
using System.Web.UI.WebControls;

namespace HovedOppgave.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        IRepository myrep = new Repository();

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            string master = "~/Views/Shared/_LoggedOut.cshtml";
            return View("Login", master);
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, AuthenticateEventArgs e)
        {
            if (ModelState.IsValid)
            {
                User loggingIn = myrep.GetUser(model.Email);
                if(loggingIn.UserId != 0 && Hash.CheckPassword(model.Password, loggingIn.PassHash, loggingIn.PassSalt))
                {
                    e.Authenticated = true;
                    SmallClasses.LoggingIn(loggingIn);
                    if (model.RememberMe)
                    {
                        // viss brukeren vil at systemet skal huske han 
                        // http://stackoverflow.com/questions/3140341/how-to-create-persistent-cookies-in-asp-net
                        HttpCookie persist = new HttpCookie("persist");
                        persist.Values.Add("UserID", loggingIn.UserId.ToString());
                        persist.Expires = DateTime.Now.AddMonths(1); // Husker bruker i 1 månde
                        Response.Cookies.Add(persist);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    e.Authenticated = false;
                    ModelState.AddModelError("", "Feil innloggings legitimasjon, vennligst prøv på nytt.");
                }
            }
            // kommer vi så langt, noe har failet, vise formen igjen
            Session["flashMelding"] = "Feil innloggings legitimasjon, vennligst prøv på nytt.";
            Session["flashStatus"] = Constant.NotificationType.danger.ToString();
            SmallClasses.DeleteSessions();
            return View(model);
        }
        
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            string master = null;
            CreatUserViewModel model = new CreatUserViewModel();
            if(Session["UserID"] != null && Validator.CheckRights(Constant.Rights.Administrator))
            {
                List<Rights> list = myrep.GetAllRights();
                model.Rights = list;
                master = "~/Views/Shared/_AdminLayout.cshtml";
                return View("Register", master, model);
            }
            master = "~/Views/Shared/_LoggedOut.cshtml";
            return View("Register", master, model);
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(CreatUserViewModel model, AuthenticateEventArgs e)
        {
            if (ModelState.IsValid && Validator.ValidateEmail(model.Email))
            {
                List<User> userList = myrep.GetAllUsers();
                foreach(var item in userList)
                {
                    if(item.Email.Equals(model.Email))
                    {
                        Session["flashMelding"] = "Emailen eksisterer fra før";
                        Session["flashStatus"] = Constant.NotificationType.danger.ToString();
                        SmallClasses.DeleteSessions();
                        return View(model);
                    }
                }
                
                Hashtable table = Hash.GetHashAndSalt(model.Password);
                User user = null;
                string message = null;
                if (model.Right != null)
                {
                    user = new User()
                    {
                        Email = model.Email,
                        Name = model.Name,
                        PassHash = (string)table["hash"],
                        PassSalt = (string)table["salt"],
                        RightsID = model.Right.RightsID,
                        Checked = false
                    };
                    message = "Velkommen til vårres system. Du har nu registrert deg her hos oss. Administrator har gitt deg rettigheten " + model.Right.Name + " og med passord " + model.Password + ". Du kan logger deg bare inn med eposten din og evt. endre på bruker detaljene dine";
                }
                else
                {
                    user = new User()
                    {
                        Email = model.Email,
                        Name = model.Name,
                        PassHash = (string)table["hash"],
                        PassSalt = (string)table["salt"],
                        RightsID = 3,
                        Checked = false
                    };
                    message = "Velkommen til vårres system. Du har nu registrert deg her hos oss. Du vil foreløbig bare få bruker rettighet gjest til administrator har evt gitt deg en annen rettighet";
                }

                int id = myrep.CreateUser(user);

                SendEmail send = new SendEmail();
                send.SendEpost(model.Email, message, "Opprettelse av konto");

                if (id != 0)
                {
                    user.UserId = id;
                    SmallClasses.LoggingIn(user);
                    e.Authenticated = true;
                    return RedirectToAction("Index", "Home");
                }
                else
                    e.Authenticated = false;
            }
            
            // kommer vi så langt, noe har failet, vise formen igjen
            Session["flashMelding"] = "Emailen din er ikke godkjent";
            Session["flashStatus"] = Constant.NotificationType.danger.ToString();
            SmallClasses.DeleteSessions();
            return View(model);
        }

        // GET: /Account/LostPassword
        [AllowAnonymous]
        public ActionResult LostPassword()
        {
            string master = "~/Views/Shared/_LoggedOut.cshtml";
            return View("LostPassword", master);
        }

        // POST: /Account/LostPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LostPassword(LostPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = myrep.GetUser(model.Email);
                if(user.UserId == 0)
                {
                    Session["flashMessage"] = "Eposten eksisterer ikke!";
                    Session["flashStatus"] = Classes.Constant.NotificationType.danger.ToString();
                    return View(model);
                }

                MailMessage msg = new MailMessage();
                SendEmail sendMsg = new SendEmail();
                string newPassword = SmallClasses.CreatePassword(10);

                if (SmallClasses.UpdatePassword(user, newPassword))
                {
                    msg.Subject = "Tilsendt nytt passord";
                    msg.Body = "Hei " + user.Name + "!\n" + "Her har du et nytt passord for din bruker " + newPassword + ".\nVi vil anbefale deg å å skifte passord når du får logget deg inn til noe som er mer personlig";
                    if (sendMsg.SendEpost(model.Email, msg.Body, msg.Subject))
                    {
                        Session["flashMessage"] = "Du har nu fått en epost om ditt nye passord";
                        Session["flashStatus"] = Classes.Constant.NotificationType.success.ToString();
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        Session["flashMessage"] = "Klarte ikke å sende en epost";
                        Session["flashStatus"] = Classes.Constant.NotificationType.danger.ToString();
                    }
                }
                else
                {
                    Session["flashMessage"] = "Ville ikke oppdateres i databasen";
                    Session["flashStatus"] = Classes.Constant.NotificationType.danger.ToString();
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/Manage
        public ActionResult Manage(int id)
        {
            ManageUserViewModel model = new ManageUserViewModel();
            User loggedIn = null;
            //User loggedIn = myrep.GetUser(SessionCheck.CheckForUserID());
            User change = null;

            if (id != 0)
                change = myrep.GetUser(id);
            else
                change = loggedIn;

            if(loggedIn.RightsID == 1)
            {
                List<Rights> rights = myrep.GetAllRights();
                model.Rights = rights;
                model.AdminUser = loggedIn;
            }
            model.ChangeUser = change;

            string master = SessionCheck.FindMaster();
            return View("Manage", master, model);
        }
        
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(ManageUserViewModel model)
        {
            if (ModelState.IsValid && Validator.ValidateEmail(model.ChangeUser.Email))
            {
                User edit = model.ChangeUser;
                string message = null;
                if (model.Right.RightsID != 0)
                {
                    edit.RightsID = model.Right.RightsID;
                    if (model.AdminUser.UserId != 0 && model.AdminUser.UserId != model.ChangeUser.UserId)
                    {
                        message = "Administrator har endret på kontoen din. bruker informasjonen din er nu... Navn: " + edit.Name + ", Epost: " + edit.Email + ", Rettighet: " + edit.RightsID +  ".";
                    }
                }

                if (model.OldPassword != "" &&  model.NewPassword != "" &&
                    Hash.CheckPassword(model.OldPassword, model.ChangeUser.PassHash, model.ChangeUser.PassSalt))
                {
                    Hashtable table = Hash.GetHashAndSalt(model.NewPassword);
                    edit.PassHash = (string)table["hash"];
                    edit.PassSalt = (string)table["salt"];
                    if (model.AdminUser.UserId != 0 && model.AdminUser.UserId != model.ChangeUser.UserId)
                    {
                        message = "Administrator har endret på kontoen din. bruker informasjonen din er nu... Navn: " + edit.Name + ", Epost: " + edit.Email + ", Rettighet: " + edit.RightsID + ", Passord: " + model.NewPassword + ".";
                    }
                }
                
                if(message != null)
                {
                    SendEmail send = new SendEmail();
                    send.SendEpost(edit.Email, message, "Bruker detaljer endret");
                }

                if(myrep.EditUser(edit))
                    if (model.Right.RightsID != 0)
                    {
                        Session["flashMelding"] = "Suksessfult";
                        Session["flashStatus"] = Constant.NotificationType.success.ToString();
                        return RedirectToAction("OverViewUsers", "Administrator");
                    }
                    else
                    {
                        Session["flashMelding"] = "Ville ikke oppdatere db";
                        Session["flashStatus"] = Constant.NotificationType.danger.ToString();
                        return RedirectToAction("Manage");
                    }
            }

            // kommer vi så langt, noe har failet, vise formen igjen
            Session["flashMelding"] = "Emailen din er ikke godkjent";
            Session["flashStatus"] = Constant.NotificationType.danger.ToString();
            return View(model);
        }

        // Get: /Account/LogOff
        public ActionResult LogOff(AuthenticateEventArgs e)
        {
            SmallClasses.DeleteSessions();
            e.Authenticated = false;
            return RedirectToAction("Login");
        }
    }
}