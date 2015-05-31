using HovedOppgave.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;

namespace HovedOppgave.Classes
{
    /// <summary>
    /// En default epost klasse som brukes for å sende en epost til en
    /// eller flere.
    /// </summary>

    public class SendEmail : System.Web.UI.Page
    {
        public bool SendEpost(string email, string message, string subject)
        {
            try
            {
                MailMessage msg = new MailMessage();
                SmtpClient smtp = new SmtpClient();

                msg.From = new MailAddress("Admin@gmail.com"); // Avsender
                msg.To.Add(email); // Sendes til
                msg.Subject = subject; // Tittelen
                msg.Body = message; // Beskjeden
                msg.IsBodyHtml = true;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("bacheloroppgavehingr6@gmail.com", "BacOpp06");
                smtp.Send(msg);
                return true;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Message", "alert('Error occured : " + ex.Message.ToString() + "');", true);
                return false;
            }
        }
        public bool SendEpost(string message, string subject, List<User> users)
        {
            try
            {
                MailMessage msg = new MailMessage();
                SmtpClient smtp = new SmtpClient();

                //bruker gruppe eposten som avsender
                msg.From = new MailAddress("Admin@gmail.com"); //Avsender
                foreach(var user in users)
                    msg.To.Add(user.Email); // Sendes til
                msg.Subject = subject; // Tittelen
                msg.Body = message; // Beskjeden
                msg.IsBodyHtml = true;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("bacheloroppgavehingr6@gmail.com", "BacOpp06");
                smtp.Send(msg);
                return true;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Message", "alert('Error occured : " + ex.Message.ToString() + "');", true);
                return false;
            }
        }
    }
}