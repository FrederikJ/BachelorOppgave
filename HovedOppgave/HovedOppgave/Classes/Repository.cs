using HovedOppgave.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

//for info om skrive query - http://www.codeproject.com/Articles/43438/Connect-C-to-MySQL

namespace HovedOppgave.Models
{
    public class Repository : IRepository
    {
        MySqlConnection conn;
        HttpContext http = HttpContext.Current;

        public Repository()
        {
            this.Initialize();    
        }

        #region DB klasser
        private void Initialize()
        {
            string myConnectionString = "server=82.164.4.64;user id=boppg;persistsecurityinfo=True;database=mydb;port=3300";
            conn = new MySqlConnection(myConnectionString);  
        }
        private bool OpenConnection()
        {
            try
            {
                conn.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //To meste vanlige problemene
                switch (ex.Number)
                {
                    case 0:
                        http.Session["flashMessage"] = "Kunne ikke åpne forbindelse";
                        http.Session["flashStatus"] = Constants.NotificationType.danger.ToString();
                        break;
                    case 1045:
                        http.Session["flashMessage"] = "Feil brukernavn/passord, prøv igjen";
                        http.Session["flashStatus"] = Constants.NotificationType.danger.ToString();
                        break;
                }
                return false;
            }
        }
        private bool CloseConnection()
        {
            try
            {
                conn.Close();
                return true;
            }
            catch(MySqlException ex)
            {
                http.Session["flashMessage"] = "Kunne ikke stenge forbindelse - " + ex.Message;
                http.Session["flashStatus"] = Constants.NotificationType.danger.ToString();
                return false;
            }
        }
        #endregion

        #region Building Queries
        #endregion
        #region Cable Queries
        #endregion
        #region CableType Queries
        #endregion
        #region Company Queries
        #endregion
        #region Connection Queries
        #endregion
        #region ConnectorType Queries
        #endregion
        #region ConnectorTypeHasPin Queries
        #endregion
        #region Contact Queries
        #endregion
        #region ContactInfo Queries
        #endregion
        #region ContactInfoType Queries
        #endregion
        #region Device Queries
        #endregion
        #region DeviceConnector Queries
        #endregion
        #region DeviceType Queries
        #endregion
        #region EventType Queries
        #endregion
        #region LogEvent Queries
        #endregion
        #region NetworkInfo Queries
        #endregion
        #region Pin Queries
        #endregion
        #region PostCode Queries
        #endregion
        #region Rights Queries
        public Rights GetRights(int userID)
        {
            return null;
        }
        #endregion
        #region Room Queries
        #endregion
        #region SignalStandard Queries
        #endregion
        #region User Queries
        public User GetUserWithRights(int userID, Constants.Rights rights)
        {
            
            return null;
        }

        public User GetUser(int userID)
        {
            return null;
        }
        #endregion
    }
}