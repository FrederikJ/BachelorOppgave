using HovedOppgave.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace HovedOppgave.Models
{
    public class Repository : IRepository
    {
        MySqlConnection conn;

        public Repository()
        {
            conn = new MySqlConnection();
            string myConnectionString = "server=82.164.4.64;user id=boppg;persistsecurityinfo=True;database=mydb;port=3300";
            try
            {
                conn,
            }
        }
        
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