using HovedOppgave.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HovedOppgave.Models
{
    interface IRepository
    {
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
        Rights GetRights(int userID);
        #endregion
        #region Room Queries
        #endregion
        #region SignalStandard Queries
        #endregion
        #region User Queries
        User GetUserWithRights(int userID, Constants.Rights rights);
        User GetUser(int UserID);
        #endregion
    }
}
