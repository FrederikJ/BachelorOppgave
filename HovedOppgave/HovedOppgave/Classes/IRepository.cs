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
        List<Company> GetAllCompanys();
        Company GetCompany(int companyId);
        #endregion
        #region Connection Queries
        #endregion
        #region ConnectorType Queries
        #endregion
        #region ConnectorTypeHasPin Queries
        #endregion
        #region Contact Queries
        List<Contact> GetAllContacts();
        Contact GetContact(int contactId);
        #endregion
        #region ContactInfo Queries
        #endregion
        #region ContactInfoType Queries
        #endregion
        #region Device Queries
        List<Device> GetAllDevices();
        Device GetDevice(int deviceId);
        #endregion
        #region DeviceConnector Queries
        #endregion
        #region DeviceType Queries
        List<DeviceType> GetAllDeviceTypes();
        DeviceType GetDeviceType(int deviceTypeId);
        #endregion
        #region EventType Queries
        List<EventType> GetAllEventTypes();
        EventType GetEventType(int eventTypeId);
        #endregion
        #region File Queries
        int CreateFile(Files file);
        List<Files> GetAllFiles();
        Files GetFile(int fileId);
        bool DeleteFile(Files file);
        Files GetLastInsertedFile();
        bool EditFile(Files file);
        #endregion
        #region LogEvent Queries
        bool CreateLogEvent(LogEvent logEvent);
        List<LogEvent> GetAllLogEvent();
        LogEvent GetLogEvent(int logEventId);
        bool EditLogEvent(LogEvent logEvent);
        LogEvent GetLastEventForDevice(int deviceId);
        LogEvent GetLogEventByFileId(int fileId);
        List<LogEvent> GetAllLogEventToEventType(int eventTypeId);
        #endregion
        #region NetworkInfo Queries
        #endregion
        #region Pin Queries
        #endregion
        #region PostCode Queries
        #endregion
        #region Rights Queries
        Rights GetRightToUser(User user);
        List<Rights> GetAllRights();
        #endregion
        #region Room Queries
        List<Room> GetAllRooms();
        Room GetRoom(int roomId);
        #endregion
        #region SignalStandard Queries
        #endregion
        #region User Queries
        UserRight GetUserWithRights(int userID, Constant.Rights rights);
        User GetUser(int UserID);
        User GetUser(string email);
        int CreateUser(User user);
        List<User> GetAllUsers();
        List<User> GetAllUsersUnchecked();
        bool EditUser(User user);
        #endregion
    }
}
