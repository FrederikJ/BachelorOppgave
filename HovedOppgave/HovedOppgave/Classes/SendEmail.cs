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
        public void SendEpost(string email, string message, string subject)
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
                smtp.Send(msg);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Message", "alert('Error occured : " + ex.Message.ToString() + "');", true);
                return;
            }
        }
        public void SendEpost(string message, string subject, List<User> users)
        {
            try
            {
                for (int i = 0; i < users.Count; i++)
                {
                    MailMessage msg = new MailMessage();
                    SmtpClient smtp = new SmtpClient();

                    //bruker gruppe eposten som avsender
                    msg.From = new MailAddress("Admin@gmail.com"); //Avsender
                    msg.To.Add(users[i].Email); // Sendes til
                    msg.Subject = subject;  //Tittelen
                    msg.Body = message; //Beskjeden
                    msg.IsBodyHtml = true;
                    smtp.Port = 587;
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    smtp.Send(msg);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Message", "alert('Error occured : " + ex.Message.ToString() + "');", true);
                return;
            }
        }
    }
}