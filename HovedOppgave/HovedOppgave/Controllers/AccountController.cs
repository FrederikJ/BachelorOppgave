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

/**
 * Alle sider som har med innlogging er her. 
 * 
 * Forfatter: Frederik Johnsen
*/

namespace HovedOppgave.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //Få tilgang til alle spørringer opp i mot db
        IRepository myrep = new Repository();

        /**
         * Get metoden til logg inn siden. setter master
        */
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            string master = "~/Views/Shared/_LoggedOut.cshtml";
            return View("Login", master);
        }

        /**
         *  får inn login modellen med en autentiserings event args. modellen består simpelt av 
         *  brukernavn passord og en bolsk verdi om den skal huske brukeren eller ikke.
        */
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, AuthenticateEventArgs e)
        {
            if (ModelState.IsValid)
            {
                //henter brukeren
                User loggingIn = myrep.GetUser(model.Email);
                //Sjekkker om brukeren eksisterer og om det innskrevene passord stemmer med db
                if(loggingIn.UserId != 0 && Hash.CheckPassword(model.Password, loggingIn.PassHash, loggingIn.PassSalt))
                {
                    //autensiserer og setter diverse session objekt som blir brukt senere med diverse bruker sjekk
                    e.Authenticated = true;
                    SmallClasses.LoggingIn(loggingIn);
                    //viss den boslke verdien er true, lagrer han en cookie i browseren som skal vare en månde
                    if (model.RememberMe)
                    {
                        // viss brukeren vil at systemet skal huske han 
                        // http://stackoverflow.com/questions/3140341/how-to-create-persistent-cookies-in-asp-net
                        HttpCookie persist = new HttpCookie("persist");
                        persist.Values.Add("UserID", loggingIn.UserId.ToString());
                        persist.Expires = DateTime.Now.AddMonths(1); // Husker bruker i 1 månde
                        Response.Cookies.Add(persist);
                    }
                    //forflytte seg til hjemme siden
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //emailen eller passordet var feil
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

        /**
         * folk kan selv registrere seg om dem har lyst eller admmin kan gjøre det for dem.
         * er det admin vil det også bli tatt med bruker rettigheter som admin kan sette
        */
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

        /**
         *  får inn register modellen med en autentiserings event args. modellen består simpelt av 
         *  brukernavn, epost, passord, bekreft passord og evt en rettighet
        */
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(CreatUserViewModel model, AuthenticateEventArgs e)
        {
            //om alt stemer i henhold til modellen og om emailen faktisk er en email
            if (ModelState.IsValid && Validator.ValidateEmail(model.Email))
            {
                //henter alle brukere som eksisterer og ser om eposten allerede eksisterer
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
                
                //hasher og sette salt på passordet før det blir lagret i db
                Hashtable table = Hash.GetHashAndSalt(model.Password);
                //setter div. bruker preferanser
                User user = null;
                //melding som blir sent i en epost som kommer til den registrerte brukeren
                string message = null;
                //viss dette er en admin eller ikke
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

                //oppretter brukeren og får sent tilbake id til brukeren
                int id = myrep.CreateUser(user);

                //sende eposten
                SendEmail send = new SendEmail();
                send.SendEpost(model.Email, message, "Opprettelse av konto");

                //en sjekk på at brukeren faktisk ble opprettet, setter id til brukeren, 
                //setter div. session objekt, autentiserer login og redirekte til hovedsiden
                //blir ikke godkjent, så sendes du tilbake til der du kom fra
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

        /**
         * Get metoden til mistet passord siden. setter master
        */
        // GET: /Account/LostPassword
        [AllowAnonymous]
        public ActionResult LostPassword()
        {
            string master = "~/Views/Shared/_LoggedOut.cshtml";
            return View("LostPassword", master);
        }

        /**
         * om brukere har mistet passordet sitt, så skriver dem inn emailen, også blir det opprettet
         * et nytt passord og sent som epost til brukeren
        */
        // POST: /Account/LostPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LostPassword(LostPasswordViewModel model)
        {
            //om alt stemer i henhold til modellen og om emailen faktisk er en email
            if (ModelState.IsValid && Validator.ValidateEmail(model.Email))
            {
                //henter brukeren og sjekker om den eksisterer
                User user = myrep.GetUser(model.Email);
                if(user.UserId == 0)
                {
                    Session["flashMessage"] = "Eposten eksisterer ikke!";
                    Session["flashStatus"] = Classes.Constant.NotificationType.danger.ToString();
                    return View(model);
                }

                MailMessage msg = new MailMessage();
                SendEmail sendMsg = new SendEmail();
                //lager et nytt passord med en lengde 10
                string newPassword = SmallClasses.CreatePassword(10);

                //oppdatere brukeren, viss det blir sukksessfult sender han en epost med nytt passord til brukeren og redirekte brukeren til login siden
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

        /**
        * Get metoden til endring av bruker info siden. setter master setter 
        */
        // GET: /Account/Manage
        [AllowAnonymous]
        public ActionResult Manage(int id)
        {
            ManageUserViewModel model = new ManageUserViewModel();
                        
            //henter personen som er logget inn
            User loggedIn = null;
            User change = null;
            if (Session["UserID"] != null)
                loggedIn = myrep.GetUser(Validator.ConvertToNumbers(Session["UserID"].ToString()));
            //viss id = 0 (altså ikke en admin) så settes brukeren som skal endres til innlogget bruker
            if (id != 0)
                change = myrep.GetUser(id);
            else
                change = loggedIn;

            //brukeren er en admin og får med rettigheter som man kan endre på brukeren
            if(loggedIn.RightsID == 1)
            {
                List<Rights> rights = myrep.GetAllRights();
                model.Rights = rights;
                model.AdminUser = loggedIn;
            }
            model.ChangeUser = change;

            //setter master page og sender deg til viewet
            string master = SessionCheck.FindMaster();
            return View("Manage", master, model);
        }

        /**
         *  får inn register modellen. modellen består simpelt av 
         *  brukernavn, epost, passord, bekreft passord og evt en rettighet
        */
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(ManageUserViewModel model)
        {
            //sjekker at modellen er godkjent og emailen faktisk er en email
            if (ModelState.IsValid && Validator.ValidateEmail(model.ChangeUser.Email))
            {
                //setter brukeren som skal endres på.
                User edit = model.ChangeUser;
                //melding som sendes til brukerens epost
                string message = null;
                //viss innlogget bruker er admin, så blir rettigheten satt også
                if (model.Right.RightsID != 0)
                {
                    edit.RightsID = model.Right.RightsID;
                    if (model.AdminUser.UserId != 0 && model.AdminUser.UserId != model.ChangeUser.UserId)
                    {
                        message = "Administrator har endret på kontoen din. bruker informasjonen din er nu... Navn: " + edit.Name + ", Epost: " + edit.Email + ", Rettighet: " + edit.RightsID +  ".";
                    }
                }
                // sjekker at det gamle passordet og det nye passordet ikke er tom(input) og sjekker at dem stemmer over ens
                //stemmer dem så legges det til hash og salt på passordet ogh setter det til brukeren
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
                
                //sender eposten
                if(message != null)
                {
                    SendEmail send = new SendEmail();
                    send.SendEpost(edit.Email, message, "Bruker detaljer endret");
                }

                //bruekren blir oppdatert i db. viss det er en admin blir man sent tilbake til 
                //oversikt over alle brukere, mens vanlige brukere blir sent over til samme side
                //bare at alt er begynt på nytt igjen
                if(myrep.EditUser(edit))
                    if (model.Right.RightsID != 0)
                    {
                        Session["flashMelding"] = "Suksessfult";
                        Session["flashStatus"] = Constant.NotificationType.success.ToString();
                        return RedirectToAction("OverViewUsers", "Administrator");
                    }
                    else
                    {
                        Session["flashMelding"] = "Suksessfult";
                        Session["flashStatus"] = Constant.NotificationType.danger.ToString();
                        return RedirectToAction("Manage");
                    }
            }

            // kommer vi så langt, noe har failet, vise formen igjen
            Session["flashMelding"] = "Emailen din er ikke godkjent";
            Session["flashStatus"] = Constant.NotificationType.danger.ToString();
            return View(model);
        }

        /**
         *  logger av brukeren og sletter alle session objekt, og setter autentiseringen til false
        */
        // Get: /Account/LogOff
        public ActionResult LogOff(AuthenticateEventArgs e)
        {
            SmallClasses.DeleteSessions();
            e.Authenticated = false;
            return RedirectToAction("Login");
        }
    }
}